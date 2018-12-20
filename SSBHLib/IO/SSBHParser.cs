using SSBHLib.Formats;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSBHLib.IO
{
    public struct SSBHOffset
    {
        public long Value { get ; }

        public SSBHOffset(long value)
        {
            Value = value;
        }

        public static implicit operator SSBHOffset(long s)
        {
            return new SSBHOffset(s);
        }

        public static implicit operator long(SSBHOffset p)
        {
            return p.Value;
        }
    }

    public class SSBHParser : BinaryReader
    {
        public long Position => BaseStream.Position;
        public long FileSize => BaseStream.Length;

        private int bitPosition = 0;

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

        public bool TryParse(out ISSBH_File File)
        {
            File = null;
            if (FileSize < 4)
                return false;
            
            string Magic = new string(ReadChars(4));
            Seek(Position - 4);
            if (Magic.Equals("HBSS"))
            {
                Seek(0x10);
                Magic = new string(ReadChars(4));
                Seek(0x10);
            }

            var Types = (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in domainAssembly.GetTypes()
                            where typeof(ISSBH_File).IsAssignableFrom(assemblyType)
                            select assemblyType).ToArray();

            foreach (var c in Types)
            {
                SSBHFileAttribute attr = c.GetCustomAttributes(
                typeof(SSBHFileAttribute), true
                ).FirstOrDefault() as SSBHFileAttribute;
                if (attr != null)
                {
                    if (attr.Magic.Equals(Magic))
                    {
                        MethodInfo method = typeof(SSBHParser).GetMethod("Parse");

                        method = method.MakeGenericMethod(c);
                        
                        File = (ISSBH_File)method.Invoke(this, null);
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
                object[] attrs = prop.GetCustomAttributes(true);
                bool skip = false;
                foreach (object attr in attrs)
                {
                    if (attr is ParseTag tag)
                    {
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

                    Array y1 = Array.CreateInstance(prop.PropertyType.GetElementType(), Size);

                    Seek(Offset);
                    for (int i = 0; i < Size; i++)
                    {
                        object Obj = ReadProperty(prop.PropertyType.GetElementType());
                        if (Obj == null)
                        {
                            MethodInfo method = typeof(SSBHParser).GetMethod("Parse");

                            method = method.MakeGenericMethod(prop.PropertyType.GetElementType());

                            Obj = method.Invoke(this, null);
                        }
                        y1.SetValue(Obj, i);
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

        public object ReadProperty(Type t)
        {
            if (t == typeof(SSBHOffset))
                return new SSBHOffset(Position + ReadInt64());
            else if (t == typeof(ANIM_TRACKFLAGS))
                return (ANIM_TRACKFLAGS)ReadUInt32();
            else if (t.IsEnum)
                return Enum.ToObject(t, ReadUInt64());
            else if (t == typeof(byte))
                return ReadByte();
            else if (t == typeof(char))
                return ReadChar();
            else if (t == typeof(short))
                return ReadInt16();
            else if (t == typeof(ushort))
                return ReadUInt16();
            else if (t == typeof(int))
                return ReadInt32();
            else if (t == typeof(uint))
                return ReadUInt32();
            else if (t == typeof(long))
                return ReadInt64();
            else if (t == typeof(ulong))
                return ReadUInt64();
            else if (t == typeof(float))
                return ReadSingle();
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
    }
}
