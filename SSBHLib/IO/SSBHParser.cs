using SSBHLib.Formats;
using SSBHLib.Formats.Animation;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using SSBHLib.Formats.Rendering;
using System;
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
            { "BPLH", (parser) => parser.Parse<Hlpb>() },
            { "DPRN", (parser) => parser.Parse<Nrpd>() },
            { "RDHS", (parser) => parser.Parse<Shdr>() },
            { "LEKS", (parser) => parser.Parse<Skel>() },
            { "LDOM", (parser) => parser.Parse<Modl>() },
            { "HSEM", (parser) => parser.Parse<Mesh>() },
            { "LTAM", (parser) => parser.Parse<Matl>() },
            { "MINA", (parser) => parser.Parse<Anim>() }
        };

        // Avoid reflection invoke overhead for known types.
        private static readonly Dictionary<Type, Func<SsbhParser, SsbhFile>> parseMethodByType = new Dictionary<Type, Func<SsbhParser, SsbhFile>>()
        {
            { typeof(AnimGroup), (parser) => parser.Parse<AnimGroup>() },
            { typeof(AnimNode), (parser) => parser.Parse<AnimNode>() },
            { typeof(AnimTrack), (parser) => parser.Parse<AnimTrack>() },
            { typeof(MatlEntry), (parser) => parser.Parse<MatlEntry>() },
            { typeof(MatlAttribute), (parser) => parser.Parse<MatlAttribute>() },
            { typeof(ModlMaterialName), (parser) => parser.Parse<ModlMaterialName>() },
            { typeof(ModlEntry), (parser) => parser.Parse<ModlEntry>() },
            { typeof(MeshObject), (parser) => parser.Parse<MeshObject>() },
            { typeof(MeshAttribute), (parser) => parser.Parse<MeshAttribute>() },
            { typeof(MeshAttributeString), (parser) => parser.Parse<MeshAttributeString>() },
            { typeof(MeshBuffer), (parser) => parser.Parse<MeshBuffer>() },
            { typeof(MeshRiggingGroup), (parser) => parser.Parse<MeshRiggingGroup>() },
            { typeof(MeshBoneBuffer), (parser) => parser.Parse<MeshBoneBuffer>() },
            { typeof(SkelBoneEntry), (parser) => parser.Parse<SkelBoneEntry>() },
            { typeof(SkelMatrix), (parser) => parser.Parse<SkelMatrix>() }
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

        public bool TryParse(out SsbhFile file)
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
                file = parseMethodByMagic[fileMagic](this);
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

                        file = (SsbhFile)parseMethod.Invoke(this, null);
                        return true;
                    }
                }
            }

            return false;
        }

        public string ReadString(long offset)
        {
            long temp = Position;

            var stringValue = new System.Text.StringBuilder();

            Seek(offset);
            if (Position >= FileSize)
            {
                Seek(temp);
                return "";
            }

            byte b = ReadByte();
            while (b != 0)
            {
                stringValue.Append((char)b);
                b = ReadByte();
            }
            
            Seek(temp);

            return stringValue.ToString();
        }

        public T Parse<T>() where T : SsbhFile
        {
            T tObject = Activator.CreateInstance<T>();

            ParseObjectProperties(tObject);

            long temp = Position;
            tObject.PostProcess(this);
            Seek(temp);

            return tObject;
        }

        private void ParseObjectProperties<T>(T tObject) where T : SsbhFile
        {
            foreach (var prop in tObject.GetType().GetProperties())
            {
                if (ShouldSkipProperty(tObject, prop))
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
            bool inline = prop.GetValue(tObject) != null;
            long offset = GetOffset(inline);
            long size = GetSize(tObject, prop, inline);

            long temp = Position;

            var propElementType = prop.PropertyType.GetElementType();

            Seek(offset);

            // Check for the most frequent types first before using reflection.
            if (propElementType == typeof(byte))
                SetArrayPropertyByte(tObject, prop, size);
            else if (propElementType == typeof(AnimTrack))
                SetArrayPropertyIssbh<AnimTrack>(tObject, prop, size);
            else if (propElementType == typeof(AnimNode))
                SetArrayPropertyIssbh<AnimNode>(tObject, prop, size);
            else if (propElementType == typeof(AnimGroup))
                SetArrayPropertyIssbh<AnimGroup>(tObject, prop, size);
            else if (propElementType == typeof(MatlAttribute))
                SetArrayPropertyIssbh<MatlAttribute>(tObject, prop, size);
            else if (propElementType == typeof(SkelMatrix))
                SetArrayPropertyIssbh<SkelMatrix>(tObject, prop, size);
            else
                SetArrayPropertyGeneric(tObject, prop, size, propElementType);

            if (!inline)
                Seek(temp);
        }

        private void SetArrayPropertyGeneric<T>(T tObject, PropertyInfo prop, long size, Type propElementType) where T : SsbhFile
        {
            Array array = Array.CreateInstance(propElementType, size);
            for (int i = 0; i < size; i++)
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
            var array = new byte[size];
            for (int i = 0; i < size; i++)
                array[i] = ReadByte();

            prop.SetValue(targetObject, array);
        }

        private void SetArrayPropertyIssbh<T>(object targetObject, PropertyInfo prop, long size) where T : SsbhFile
        {
            var array = new T[size];
            for (int i = 0; i < size; i++)
                array[i] = Parse<T>();

            prop.SetValue(targetObject, array);
        }

        private long GetSize<T>(T tObject, PropertyInfo prop, bool inline) where T : SsbhFile
        {
            if (!inline)
                return ReadInt64();
            else
                return (prop.GetValue(tObject) as Array).Length;
        }

        private long GetOffset(bool inline)
        {
            if (!inline)
                return Position + ReadInt64();
            else
                return Position;
        }

        private void SetString<T>(T tObject, PropertyInfo prop) where T : SsbhFile
        {
            long stringOffset = Position + ReadInt64();
            prop.SetValue(tObject, ReadString(stringOffset));
        }

        private static bool ShouldSkipProperty<T>(T tObject, PropertyInfo prop) where T : SsbhFile
        {
            bool shouldSkip = false;

            foreach (var attribute in prop.GetCustomAttributes(true))
            {
                if (attribute is ParseTag tag)
                {
                    if (tag.Ignore)
                        shouldSkip = true;
                }
            }

            return shouldSkip;
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

        public T[] ByteToType<T>(int count)
        {
            T[] items = new T[count];

            for (int i = 0; i < count; i++)
                items[i] = ByteToType<T>();

            return items;
        }

        public T ByteToType<T>()
        {
            byte[] bytes = ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }
    }
}
