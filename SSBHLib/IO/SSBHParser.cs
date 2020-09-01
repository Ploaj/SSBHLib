using SSBHLib.Formats;
using SSBHLib.Formats.Animation;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using SSBHLib.Formats.Rendering;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SSBHLib.IO
{
    public class SsbhParser : BinaryReader
    {
        public long Position => BaseStream.Position;
        public long FileSize => BaseStream.Length;

        private int bitPosition;

        private static readonly List<Type> issbhTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies() from assemblyType in domainAssembly.GetTypes()
                                                            where typeof(SsbhFile).IsAssignableFrom(assemblyType) select assemblyType).ToList();

        private static readonly Dictionary<Type, MethodInfo> genericParseMethodInfoByType = new Dictionary<Type, MethodInfo>();

        // Avoid reflection invoke overhead for known file magics.
        private static readonly Dictionary<string, Func<SsbhParser, SsbhFile>> parseMethodByMagic = new Dictionary<string, Func<SsbhParser, SsbhFile>>()
        {
            { "BPLH", (parser) => parser.ParseHlpb() },
            { "DPRN", (parser) => parser.ParseNrpd() },
            { "RDHS", (parser) => parser.ParseShdr() },
            { "LEKS", (parser) => parser.ParseSkel() },
            { "LDOM", (parser) => parser.ParseModl() },
            { "HSEM", (parser) => parser.ParseMesh() },
            { "LTAM", (parser) => parser.ParseMatl() },
            { "MINA", (parser) => parser.ParseAnim() }
        };

        // Avoid reflection invoke overhead for known types.
        private static readonly Dictionary<Type, Func<SsbhParser, SsbhFile>> parseMethodByType = new Dictionary<Type, Func<SsbhParser, SsbhFile>>()
        {
            { typeof(AnimGroup), (parser) => parser.ParseAnimGroup() },
            { typeof(AnimNode), (parser) => parser.ParseAnimNode() },
            { typeof(AnimTrack), (parser) => parser.ParseAnimTrack() },
            { typeof(MatlEntry), (parser) => parser.ParseMatlEntry() },
            { typeof(MatlAttribute), (parser) => parser.ParseMatlAttribute() },
            { typeof(ModlMaterialName), (parser) => parser.ParseModlMaterialName() },
            { typeof(ModlEntry), (parser) => parser.ParseModlEntry() },
            { typeof(MeshObject), (parser) => parser.ParseMeshObject() },
            { typeof(MeshAttribute), (parser) => parser.ParseMeshAttribute() },
            { typeof(MeshAttributeString), (parser) => parser.ParseMeshAttributeString() },
            { typeof(MeshBuffer), (parser) => parser.ParseMeshBuffer() },
            { typeof(MeshRiggingGroup), (parser) => parser.ParseMeshRiggingGroup() },
            { typeof(MeshBoneBuffer), (parser) => parser.ParseMeshBoneBuffer() },
            { typeof(SkelBoneEntry), (parser) => parser.ParseSkelBoneEntry() },
            { typeof(Matrix4x4), (parser) => parser.ParseMatrix4x4() }
        };

        public SsbhParser(Stream stream) : base(stream)
        {
            
        }

        public void Seek(long position)
        {
            BaseStream.Position = position;
        }

        public byte Peek()
        {
            byte b = ReadByte();
            BaseStream.Seek(-1, SeekOrigin.Current);
            return b;
        }

        public bool TryParse<T>(out T file) where T : SsbhFile
        {
            file = null;
            if (FileSize < 4)
                return false;
            
            string fileMagic = new string(ReadChars(4));
            Seek(Position - 4);
            if (fileMagic.Equals("HBSS"))
            {
                Seek(0x10);
                fileMagic = new string(ReadChars(4));
                Seek(0x10);
            }

            if (parseMethodByMagic.ContainsKey(fileMagic))
            {
                file = (T)parseMethodByMagic[fileMagic](this);
                return true;
            }

            // The type is not known, so do a very slow check to find it.
            foreach (var type in issbhTypes)
            {
                if (type.GetCustomAttributes(typeof(SsbhFileAttribute), true).FirstOrDefault() is SsbhFileAttribute attr)
                {
                    if (attr.Magic.Equals(fileMagic))
                    {
                        MethodInfo parseMethod = typeof(SsbhParser).GetMethod("Parse");
                        parseMethod = parseMethod.MakeGenericMethod(type);

                        file = (T)parseMethod.Invoke(this, null);
                        return true;
                    }
                }
            }

            return false;
        }

        public string ReadOffsetReadString()
        {
            long stringOffset = Position + ReadInt64();
            return ReadString(stringOffset);
        }

        public string ReadString(long offset)
        {
            long previousPosition = Position;

            var stringValue = new System.Text.StringBuilder();

            Seek(offset);
            if (Position >= FileSize)
            {
                Seek(previousPosition);
                return "";
            }

            byte b = ReadByte();
            while (b != 0)
            {
                stringValue.Append((char)b);
                b = ReadByte();
            }
            
            Seek(previousPosition);

            return stringValue.ToString();
        }

        public T Parse<T>() where T : SsbhFile
        {
            T tObject = Activator.CreateInstance<T>();

            ParseObjectProperties(tObject);

            if (tObject is MatlAttribute attribute)
            {
                long temp = Position;
                attribute.PostProcess(this);
                Seek(temp);
            }

            return tObject;
        }

        private void ParseObjectProperties<T>(T tObject) where T : SsbhFile
        {
            foreach (var prop in tObject.GetType().GetProperties())
            {
                if (ParseTag.ShouldSkipProperty(prop))
                    continue;

                if (prop.PropertyType == typeof(string))
                {
                    SetString(tObject, prop);
                }
                else if (prop.PropertyType.IsArray)
                {
                    SetArray(tObject, prop);
                }
                else
                {
                    prop.SetValue(tObject, ReadProperty(prop.PropertyType));
                }
            }
        }

        public static Dictionary<Type, long> countByType = new Dictionary<Type, long>();

        private void SetArray<T>(T tObject, PropertyInfo prop) where T : SsbhFile
        {
            long absoluteOffset = GetOffset();
            long elementCount = GetElementCount(tObject, prop);

            long previousPosition = Position;

            var propElementType = prop.PropertyType.GetElementType();

            Seek(absoluteOffset);

            // Check for the most frequent types first before using reflection.
            if (propElementType == typeof(byte))
                SetArrayPropertyByte(tObject, prop, elementCount);
            else
                SetArrayPropertyGeneric(tObject, prop, elementCount, propElementType);

            Seek(previousPosition);
        }

        private void SetArrayPropertyGeneric<T>(T tObject, PropertyInfo prop, long elementCount, Type propElementType) where T : SsbhFile
        {
            Array array = Array.CreateInstance(propElementType, elementCount);
            for (int i = 0; i < elementCount; i++)
            {
                // Check for primitive types first.
                // If the type is not primitive, check for an SSBH type.
                object obj = ReadProperty(propElementType);

                if (obj == null)
                {
                    if (parseMethodByType.ContainsKey(propElementType))
                    {
                        obj = parseMethodByType[propElementType](this);
                    }
                    else
                    {
                        // Only use reflection for types only known at runtime.
                        if (!genericParseMethodInfoByType.ContainsKey(propElementType))
                            AddParseMethod(prop, propElementType);

                        obj = genericParseMethodInfoByType[propElementType].Invoke(this, null);
                    }
                }
                array.SetValue(obj, i);
            }

            prop.SetValue(tObject, array);
        }

        private void SetArrayPropertyByte(object targetObject, PropertyInfo prop, long size)
        {
            prop.SetValue(targetObject, ReadBytes((int)size));
        }

        private long GetElementCount<T>(T tObject, PropertyInfo prop) where T : SsbhFile
        {
            return ReadInt64();
        }

        public long GetOffset()
        {
            return Position + ReadInt64();
        }

        private void SetString<T>(T tObject, PropertyInfo prop) where T : SsbhFile
        {
            long stringOffset = Position + ReadInt64();
            prop.SetValue(tObject, ReadString(stringOffset));
        }

        private static void AddParseMethod(PropertyInfo prop, Type propElementType)
        {
            MethodInfo parseMethod = typeof(SsbhParser).GetMethod("Parse");
            parseMethod = parseMethod.MakeGenericMethod(prop.PropertyType.GetElementType());
            genericParseMethodInfoByType.Add(propElementType, parseMethod);
        }

        public object ReadProperty(Type t)
        {
            // Check for enums last to improve performance.
            if (t == typeof(byte))
                return ReadByte();
            else if (t == typeof(uint))
                return ReadUInt32();
            else if (t == typeof(long))
                return ReadInt64();
            else if (t == typeof(SsbhOffset))
                return new SsbhOffset(Position + ReadInt64());
            else if (t == typeof(float))
                return ReadSingle();
            else if (t == typeof(int))
                return ReadInt32();
            else if (t == typeof(ushort))
                return ReadUInt16();
            else if (t == typeof(short))
                return ReadInt16();
            else if (t == typeof(ulong))
                return ReadUInt64();
            else if (t.IsEnum)
                return Enum.ToObject(t, ReadUInt64());
            else if (t == typeof(char))
                return ReadChar();
            else
                return null;
        }

        public int ReadBits(int bitCount)
        {
            byte b = Peek();
            int value = 0;
            int le = 0;
            int bitIndex = 0;
            for (int i = 0; i < bitCount; i++)
            {
                byte bit = (byte)((b & (0x1 << (bitPosition))) >> (bitPosition));
                value |= (bit << (le + bitIndex));
                bitPosition++;
                bitIndex++;
                if (bitPosition >= 8)
                {
                    bitPosition = 0;
                    b = ReadByte();
                    b = Peek();
                }
                if (bitIndex >= 8)
                {
                    bitIndex = 0;
                    if (le + 8 > bitCount)
                    {
                        le = bitCount - 1;
                    }
                    else
                        le += 8;
                }
            }
            
            return value;
        }
    }
}
