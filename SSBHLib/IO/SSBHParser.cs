using SSBHLib.Formats;
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
            
            string Mmagic = new string(ReadChars(4));
            Seek(Position - 4);
            if (Mmagic.Equals("HBSS"))
            {
                Seek(0x10);
                Mmagic = new string(ReadChars(4));
                Seek(0x10);
            }

            foreach (var type in issbhTypes)
            {
                if (type.GetCustomAttributes(typeof(SSBHFileAttribute), true).FirstOrDefault() is SSBHFileAttribute attr)
                {
                    if (attr.Magic.Equals(Mmagic))
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

            string stringValue = "";

            Seek(offset);
            if (Position >= FileSize)
            {
                Seek(temp);
                return stringValue;
            }

            byte b = ReadByte();
            while (b != 0)
            {
                stringValue += (char)b;
                if (Position >= FileSize)
                {
                    Seek(temp);
                    return stringValue;
                }
                b = ReadByte();
            }
            
            Seek(temp);

            return stringValue;
        }

        public T Parse<T>() where T : ISSBH_File
        {
            T tObject = Activator.CreateInstance<T>();

            //Reading Object
            foreach (var prop in tObject.GetType().GetProperties())
            {
                bool skip = false;
                foreach (var attribute in prop.GetCustomAttributes(true))
                {
                    if (attribute is ParseTag tag)
                    {
                        if (tag.Ignore)
                            skip = true;

                        if (!tag.IF.Equals(""))
                        {
                            string[] args = tag.IF.Split('>');
                            PropertyInfo checkprop = null;
                            foreach (PropertyInfo pi in tObject.GetType().GetProperties())
                            {
                                if (pi.Name.Equals(args[0]))
                                {
                                    checkprop = pi;
                                    break;
                                }
                            }
                            skip = true;
                            if (checkprop != null)
                            {
                                if ((ushort)checkprop.GetValue(tObject) > int.Parse(args[1]))
                                {
                                    skip = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (skip)
                    continue;

                if (prop.PropertyType == typeof(string))
                {
                    long StringOffset = Position + ReadInt64();
                    prop.SetValue(tObject, ReadString(StringOffset));
                }
                else if (prop.PropertyType.IsArray)
                {
                    bool Inline = prop.GetValue(tObject) != null;
                    long Offset = Position;
                    long Size = 0;
                    if (!Inline)
                    {
                        Offset = Position + ReadInt64();
                        Size = ReadInt64();
                    }
                    else
                    {
                        Size = (prop.GetValue(tObject) as Array).Length;
                    }
                    
                    long temp = Position;

                    var propElementType = prop.PropertyType.GetElementType();
                    Array y1 = Array.CreateInstance(propElementType, Size);

                    Seek(Offset);
                    for (int i = 0; i < Size; i++)
                    {
                        object obj = ReadProperty(propElementType);
                        if (obj == null)
                        {
                            if (!genericParseMethodByType.ContainsKey(propElementType))
                            {
                                AddParseMethod(prop, propElementType);
                            }

                            obj = genericParseMethodByType[propElementType].Invoke(this, null);
                        }
                        y1.SetValue(obj, i);
                    }

                    prop.SetValue(tObject, y1);
                    
                    if(!Inline)
                        Seek(temp);
                }
                else
                {
                    prop.SetValue(tObject, ReadProperty(prop.PropertyType));
                }
            }

            long tempp = Position;
            tObject.PostProcess(this);
            Seek(tempp);

            return tObject;
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
