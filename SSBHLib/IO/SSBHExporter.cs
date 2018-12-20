using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSBHLib.IO
{
    public class SSBHExporter : BinaryWriter
    {
        public long Position => BaseStream.Position; 
        public long FileSize => BaseStream.Length;

        private LinkedList<object> objectQueue = new LinkedList<object>();
        private Dictionary<uint, object> objectOffset = new Dictionary<uint, object>();

        public SSBHExporter(Stream stream) : base(stream)
        {

        }

        public static void WriteSSBHFile(string fileName, ISSBH_File file, bool writeHeader = true)
        {
            using (SSBHExporter exporter = new SSBHExporter(new FileStream(fileName, FileMode.Create)))
            {
                // write ssbh header
                if (writeHeader)
                {
                    exporter.Write(new char[] { 'H', 'B', 'S', 'S'});
                    exporter.Write(0x40);
                    exporter.Pad(0x10);
                }

                // write file contents
                exporter.AddSSBHFile(file);
                exporter.Pad(8);
            }
        }

        private bool Skip(ISSBH_File File, PropertyInfo prop)
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

        private void AddSSBHFile(ISSBH_File file)
        {
            objectQueue.AddFirst(file);
            while (objectQueue.Count > 0)
            {
                var obj = objectQueue.First();
                objectQueue.RemoveFirst();

                // I guess?
                if (obj is Array)
                {
                    Pad(0x8);
                }

                if (objectOffset.ContainsValue(obj))
                {
                    long key = objectOffset.FirstOrDefault(x => x.Value == obj).Key;
                    if (key != 0)
                    {
                        long Temp = Position;
                        BaseStream.Position = key;
                        WriteProperty(Temp - key);
                        BaseStream.Position = Temp;
                        objectOffset.Remove((uint)key);
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
                        LinkedList<object> ObjectQueueTemp = objectQueue;// new LinkedList<object>();
                        objectQueue = new LinkedList<object>();
                        foreach (object o in Array)
                        {
                            WriteProperty(o);
                            //ObjectQueue.AddFirst(o);
                        }
                        foreach(object o in ObjectQueueTemp)
                            objectQueue.AddLast(o);
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
                    objectQueue.AddLast(prop.GetValue(File));
                    objectOffset.Add((uint)Position, prop.GetValue(File));
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
                            objectOffset.Add((uint)Position, Array);
                        objectQueue.AddLast(Array);
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
