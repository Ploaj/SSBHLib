using System.IO;
using SSBHLib.Formats;
using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;

namespace SSBHLib.IO
{
    public struct SSBHOffset
    {
        private readonly long value;

        public SSBHOffset(long value)
        {
            this.value = value;
        }

        public long Value { get { return value; } }

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
        public long Position { get { return BaseStream.Position; } }
        public long FileSize { get { return BaseStream.Length; } }
        private int BitPosition = 0;

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
                var attr = c.GetCustomAttributes(
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

        public string ReadString(long Offset)
        {
            long temp = Position;

            string String = "";

            Seek(Offset);
            if (Position >= FileSize)
            {
                Seek(temp);
                return String;
            }
            byte b = ReadByte();
            while (b != 0)
            {
                String += (char)b;
                if (Position >= FileSize)
                {
                    Seek(temp);
                    return String;
                }
                b = ReadByte();
            }
            
            Seek(temp);

            return String;
        }

        public T Parse<T>() where T : ISSBH_File
        {
            T Object = Activator.CreateInstance<T>();

            //Reading Object
            foreach (var prop in Object.GetType().GetProperties())
            {
                object[] attrs = prop.GetCustomAttributes(true);
                bool skip = false;
                foreach (object attr in attrs)
                {
                    ParseTag tag = attr as ParseTag;
                    if (tag != null)
                    {
                        if (!tag.IF.Equals(""))
                        {
                            string[] args = tag.IF.Split('>');
                            PropertyInfo checkprop = null;
                            foreach (PropertyInfo pi in Object.GetType().GetProperties())
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
                                if((ushort)checkprop.GetValue(Object) > int.Parse(args[1]))
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
                    prop.SetValue(Object, ReadString(StringOffset));
                } else
                if (prop.PropertyType.IsArray)
                {
                    bool Inline = prop.GetValue(Object) != null;
                    long Offset = Position;
                    long Size = 0;
                    if (!Inline)
                    {
                        Offset = Position + ReadInt64();
                        Size = ReadInt64();
                    } else
                    {
                        Size = (prop.GetValue(Object) as Array).Length;
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

                    prop.SetValue(Object, y1);
                    
                    if(!Inline)
                        Seek(temp);
                }
                else
                {
                    prop.SetValue(Object, ReadProperty(prop.PropertyType));
                }
            }

            long tempp = Position;
            Object.PostProcess(this);
            Seek(tempp);

            return Object;
        }

        public object ReadProperty(Type t)
        {
            if(t == typeof(SSBHOffset))
            {
                return new SSBHOffset(Position + ReadInt64());
            }
            if (t == typeof(ANIM_TRACKFLAGS))
                return (ANIM_TRACKFLAGS)ReadUInt32();
            else
            if (t.IsEnum)
                return Enum.ToObject(t, ReadUInt64());
            else
            if (t == typeof(byte))
                return ReadByte();
            else
            if (t == typeof(char))
                return ReadChar();
            else
            if (t == typeof(short))
                return ReadInt16();
            else
            if (t == typeof(ushort))
                return ReadUInt16();
            else
            if (t == typeof(int))
                return ReadInt32();
            else
            if (t == typeof(uint))
                return ReadUInt32();
            else
            if (t == typeof(long))
                return ReadInt64();
            else
            if (t == typeof(ulong))
                return ReadUInt64();
            else
            if (t == typeof(float))
                return ReadSingle();
            else
            {
            }
            return null;
        }

        public int ReadBits(int BitCount)
        {
            byte b = Peek();
            int Value = 0;
            int LE = 0;
            int bitindex = 0;
            for (int i = 0; i < BitCount; i++)
            {
                byte bit = (byte)((b & (0x1 << (BitPosition))) >> (BitPosition));
                Value |= (bit << (LE + bitindex));
                BitPosition++;
                bitindex++;
                if (BitPosition >= 8)
                {
                    BitPosition = 0;
                    b = ReadByte();
                    b = Peek();
                }
                if (bitindex >= 8)
                {
                    bitindex = 0;
                    if (LE + 8 > BitCount)
                    {
                        LE = BitCount - 1;
                    }
                    else
                        LE += 8;
                }
            }
            
            return Value;
        }
    }
}
