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
    public class SSBHParser : BinaryReader
    {
        public long Position => BaseStream.Position;
        public long FileSize => BaseStream.Length;

        private int bitPosition = 0;

        private static readonly List<Type> issbhTypes = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies() from assemblyType in domainAssembly.GetTypes()
                                                            where typeof(ISSBH_File).IsAssignableFrom(assemblyType) select assemblyType).ToList();

        private static readonly Dictionary<Type, MethodInfo> genericParseMethodByType = new Dictionary<Type, MethodInfo>();

        private static readonly Dictionary<Type, List<Action<ISSBH_File, Type>>> issbhSetters = new Dictionary<Type, List<Action<ISSBH_File, Type>>>();

        // Avoid reflection invoke overhead for known file magics.
        private static readonly Dictionary<string, Func<SSBHParser, ISSBH_File>> parseMethodByMagic = new Dictionary<string, Func<SSBHParser, ISSBH_File>>()
        {
            { "BPLH", (parser) => parser.Parse<HLPB>() },
            { "DPRN", (parser) => parser.Parse<NRPD>() },
            { "RDHS", (parser) => parser.Parse<SHDR>() },
            { "LEKS", (parser) => parser.Parse<SKEL>() },
            { "LDOM", (parser) => parser.Parse<MODL>() },
            { "HSEM", (parser) => parser.Parse<MESH>() },
            { "LTAM", (parser) => parser.Parse<MATL>() },
            { "MINA", (parser) => parser.Parse<ANIM>() }
        };

        // Avoid reflection invoke overhead for known types.
        private static readonly Dictionary<Type, Func<SSBHParser, ISSBH_File>> parseMethodByType = new Dictionary<Type, Func<SSBHParser, ISSBH_File>>()
        {
            { typeof(AnimGroup), (parser) => parser.Parse<AnimGroup>() },
            { typeof(AnimNode), (parser) => parser.Parse<AnimNode>() },
            { typeof(AnimTrack), (parser) => parser.Parse<AnimTrack>() },
            { typeof(MatlEntry), (parser) => parser.Parse<MatlEntry>() },
            { typeof(MatlAttribute), (parser) => parser.Parse<MatlAttribute>() },
            { typeof(MODL_MaterialName), (parser) => parser.Parse<MODL_MaterialName>() },
            { typeof(MODL_Entry), (parser) => parser.Parse<MODL_Entry>() },
            { typeof(MeshObject), (parser) => parser.Parse<MeshObject>() },
            { typeof(MeshAttribute), (parser) => parser.Parse<MeshAttribute>() },
            { typeof(MeshAttributeString), (parser) => parser.Parse<MeshAttributeString>() },
            { typeof(MeshBuffer), (parser) => parser.Parse<MeshBuffer>() },
            { typeof(MeshRiggingGroup), (parser) => parser.Parse<MeshRiggingGroup>() },
            { typeof(MeshBoneBuffer), (parser) => parser.Parse<MeshBoneBuffer>() },
            { typeof(SKEL_BoneEntry), (parser) => parser.Parse<SKEL_BoneEntry>() },
            { typeof(SKEL_Matrix), (parser) => parser.Parse<SKEL_Matrix>() }
        };

        public SSBHParser(Stream Stream) : base(Stream)
        {

        }

        public void Seek(long Position)
        {
            BaseStream.Position = Position;
        }

        public byte Peek()
        {
            byte b = ReadByte();
            BaseStream.Seek(-1, SeekOrigin.Current);
            return b;
        }

        public bool TryParse(out ISSBH_File file)
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
                if (type.GetCustomAttributes(typeof(SSBHFileAttribute), true).FirstOrDefault() is SSBHFileAttribute attr)
                {
                    if (attr.Magic.Equals(fileMagic))
                    {
                        MethodInfo parseMethod = typeof(SSBHParser).GetMethod("Parse");
                        parseMethod = parseMethod.MakeGenericMethod(type);

                        file = (ISSBH_File)parseMethod.Invoke(this, null);
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

        public T Parse<T>() where T : ISSBH_File
        {
            T tObject = Activator.CreateInstance<T>();

            ParseObjectProperties(tObject);

            long tempp = Position;
            tObject.PostProcess(this);
            Seek(tempp);

            return tObject;
        }

        private void ParseObjectProperties<T>(T tObject) where T : ISSBH_File
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

        private void SetArray<T>(T tObject, PropertyInfo prop) where T : ISSBH_File
        {
            bool inline = prop.GetValue(tObject) != null;
            long offset = GetOffset(inline);
            long size = GetSize(tObject, prop, inline);

            long temp = Position;

            var propElementType = prop.PropertyType.GetElementType();
            Array y1 = Array.CreateInstance(propElementType, size);

            Seek(offset);
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
                        if (!genericParseMethodByType.ContainsKey(propElementType))
                            AddParseMethod(prop, propElementType);

                        obj = genericParseMethodByType[propElementType].Invoke(this, null);
                    }
                }
                y1.SetValue(obj, i);
            }

            prop.SetValue(tObject, y1);

            if (!inline)
                Seek(temp);
        }

        private long GetSize<T>(T tObject, PropertyInfo prop, bool inline) where T : ISSBH_File
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

        private void SetString<T>(T tObject, PropertyInfo prop) where T : ISSBH_File
        {
            long stringOffset = Position + ReadInt64();
            prop.SetValue(tObject, ReadString(stringOffset));
        }

        private static bool ShouldSkipProperty<T>(T tObject, PropertyInfo prop) where T : ISSBH_File
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
            MethodInfo parseMethod = typeof(SSBHParser).GetMethod("Parse");
            parseMethod = parseMethod.MakeGenericMethod(prop.PropertyType.GetElementType());
            genericParseMethodByType.Add(propElementType, parseMethod);
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
            else if (t == typeof(SSBHOffset))
                return new SSBHOffset(Position + ReadInt64());
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
            int LE = 0;
            int bitIndex = 0;
            for (int i = 0; i < bitCount; i++)
            {
                byte bit = (byte)((b & (0x1 << (bitPosition))) >> (bitPosition));
                value |= (bit << (LE + bitIndex));
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
                    if (LE + 8 > bitCount)
                    {
                        LE = bitCount - 1;
                    }
                    else
                        LE += 8;
                }
            }
            
            return value;
        }

        public T[] ByteToType<T>(int Count)
        {
            T[] Items = new T[Count];

            for (int i = 0; i < Count; i++)
                Items[i] = ByteToType<T>();

            return Items;
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
