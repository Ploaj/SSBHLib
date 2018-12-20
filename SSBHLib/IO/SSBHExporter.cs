using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSBHLib.IO
{
    public class SSBHExporter : BinaryWriter
    {
        public long Position { get { return BaseStream.Position; } }
        public long FileSize { get { return BaseStream.Length; } }

        public SSBHExporter(Stream stream) : base(stream)
        {

        }

        public static void WriteSSBHFile(string FileName, ISSBH_File File, bool WriteHeader = true)
        {
            using (SSBHExporter exporter = new SSBHExporter(new FileStream(FileName, FileMode.Create)))
            {
                // write ssbh header
                if (WriteHeader)
                {
                    exporter.Write(new char[] { 'H', 'B', 'S', 'S'});
                    exporter.Write(0x40);
                    exporter.Pad(0x10);
                }

                // write file contents
                exporter.AddSSBHFile(File);
                exporter.Pad(8);
            }
        }

        private bool Skip(ISSBH_File File, PropertyInfo prop)
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
                        foreach (PropertyInfo pi in File.GetType().GetProperties())
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
                            if ((ushort)checkprop.GetValue(File) > int.Parse(args[1]))
                            {
                                skip = false;
                                break;
                            }
                        }
                    }
                }
            }
            return skip;
        }

        private LinkedList<object> ObjectQueue = new LinkedList<object>();
        private Dictionary<uint, object> ObjectOffset = new Dictionary<uint, object>();

        private void AddSSBHFile(ISSBH_File File)
        {
            ObjectQueue.AddFirst(File);
            while (ObjectQueue.Count > 0)
            {
                var obj = ObjectQueue.First();
                ObjectQueue.RemoveFirst();

                // I guess?
                if (obj is Array)
                {
                    Pad(0x8);
                }

                if (ObjectOffset.ContainsValue(obj))
                {
                    long key = ObjectOffset.FirstOrDefault(x => x.Value == obj).Key;
                    if (key != 0)
                    {
                        long Temp = Position;
                        BaseStream.Position = key;
                        WriteProperty(Temp - key);
                        BaseStream.Position = Temp;
                        ObjectOffset.Remove((uint)key);
                    }
                }

                if (obj is Array Array)
                {
                    if (Array.GetType() == typeof(byte[]))
                    {
                        foreach (byte o in Array)
                        {
                            Write(o);
                        }
                    }
                    else
                    {
                        LinkedList<object> ObjectQueueTemp = ObjectQueue;// new LinkedList<object>();
                        ObjectQueue = new LinkedList<object>();
                        foreach (object o in Array)
                        {
                            WriteProperty(o);
                            //ObjectQueue.AddFirst(o);
                        }
                        foreach(object o in ObjectQueueTemp)
                            ObjectQueue.AddLast(o);
                    }
                }
                else
                {
                    WriteProperty(obj);
                }
            }
        }

        private void WriteSSBHFile(ISSBH_File File)
        {
            foreach (var prop in File.GetType().GetProperties())
            {
                if (Skip(File, prop))
                    continue;

                if (prop.PropertyType == typeof(string))
                {
                    ObjectQueue.AddLast(prop.GetValue(File));
                    ObjectOffset.Add((uint)Position, prop.GetValue(File));
                    Write((long)0);
                }
                else if (prop.PropertyType.IsArray)
                {
                    var Array = (prop.GetValue(File) as Array);
                    bool Inline = false;
                    if (prop.GetCustomAttribute(typeof(ParseTag)) != null)
                        Inline = ((ParseTag)prop.GetCustomAttribute(typeof(ParseTag))).InLine;

                    if (!Inline)
                    {
                        if (Array.Length > 0)
                            ObjectOffset.Add((uint)Position, Array);
                        ObjectQueue.AddLast(Array);
                        Write((long)0);
                        Write((long)Array.Length);
                    }
                    else
                    {
                        // inline array
                        foreach (object o in Array)
                        {
                            WriteProperty(o);
                        }
                    }
                }
                else
                {
                    WriteProperty(prop.GetValue(File));
                }
            }

            // Post Write
            // TODO: mostly for materials....
        }

        public void WriteProperty(object value)
        {
            Type t = value.GetType();
            /* typof(SSBHOffset) */
            if (value is ISSBH_File v)
            {
                WriteSSBHFile(v);
                Pad(0x8);
            }
            else if (t == typeof(Formats.ANIM_TRACKFLAGS))
                Write((int)value);
            else if (t.IsEnum)
                Write((long)value);
            else if (t == typeof(byte))
                Write((byte)value);
            else if (t == typeof(char))
                Write((char)value);
            else if (t == typeof(short))
                Write((short)value);
            else if (t == typeof(ushort))
                Write((ushort)value);
            else if (t == typeof(int))
                Write((int)value);
            else if (t == typeof(uint))
                Write((uint)value);
            else if (t == typeof(long))
                Write((long)value);
            else if (t == typeof(ulong))
                Write((ulong)value);
            else if (t == typeof(float))
                Write((float)value);
            else if (t == typeof(string))
            {
                Pad(0x4); //this is just logical
                Write(((string)value).ToCharArray());
                Write((byte)0);
                Pad(0x4); //8 or 4?
            }
            else
                throw new Exception($"Invalid type {t.GetType()}");
        }

        public void Pad(int toSize, byte PaddingValue = 0)
        {
            while(BaseStream.Position % toSize != 0)
            {
                Write(PaddingValue);
            }
        }
    }
}
