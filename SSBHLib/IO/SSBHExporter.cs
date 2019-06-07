using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SSBHLib.IO
{
    public class SSBHExporter : BinaryWriter
    {
        private class MaterialEntry
        {
            public object Object;

            public MaterialEntry(object ob)
            {
                Object = ob;
            }
        }

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
                exporter.Pad(4);
            }
        }

        private bool Skip(ISSBH_File file, PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(true);
            bool skip = false;
            foreach (object attr in attrs)
            {
                if (attr is ParseTag tag)
                {
                    if (tag.Ignore)
                        return true;
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
                if (obj == null)
                    continue;

                // I guess?
                if (obj is Array || (obj is MaterialEntry &&((MaterialEntry)obj).Object is Formats.Materials.MatlAttribute.MatlString))
                    Pad(0x8);

                // not sure if 4 or 8
                if (obj is string)
                    Pad(0x4);

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
                        LinkedList<object> objectQueueTemp = objectQueue;
                        objectQueue = new LinkedList<object>();
                        foreach (object o in Array)
                        {
                            WriteProperty(o);
                        }
                        foreach(object o in objectQueueTemp)
                            objectQueue.AddLast(o);
                    }
                }
                else
                {
                    WriteProperty(obj);
                }
            }
        }

        private void WriteSSBHFile(ISSBH_File file)
        {
            foreach (var prop in file.GetType().GetProperties())
            {
                if (Skip(file, prop))
                    continue;

                if (prop.PropertyType == typeof(string))
                {
                    if (prop.GetValue(file) == null)
                    {
                        Write((long)0);
                        continue;
                    }
                    objectQueue.AddLast(prop.GetValue(file));
                    objectOffset.Add((uint)Position, prop.GetValue(file));
                    Write((long)0);
                }
                else if (prop.PropertyType.IsArray)
                {
                    var array = (prop.GetValue(file) as Array);
                    bool inline = false;
                    if (prop.GetCustomAttribute(typeof(ParseTag)) != null)
                        inline = ((ParseTag)prop.GetCustomAttribute(typeof(ParseTag))).InLine;

                    if (!inline)
                    {
                        if (array.Length > 0)
                            objectOffset.Add((uint)Position, array);
                        objectQueue.AddLast(array);
                        Write((long)0);
                        Write((long)array.Length);
                    }
                    else
                    {
                        // inline array
                        foreach (object o in array)
                        {
                            WriteProperty(o);
                        }
                    }
                }
                else if (prop.PropertyType == typeof(SSBHOffset)) 
                {
                    // HACK: for materials
                    var dataObject = file.GetType().GetProperty("DataObject").GetValue(file);
                    var matentry = new MaterialEntry(dataObject);
                    objectOffset.Add((uint)Position, matentry);
                    objectQueue.AddLast(matentry);
                    Write((long)0);
                }
                else
                {
                    WriteProperty(prop.GetValue(file));
                }
            }


            // TODO: Post Write is only used for materials.
            file.PostWrite(this);
        }

        public void WriteProperty(object value)
        {
            Type t = value.GetType();
            if (value is MaterialEntry)
            {
                WriteProperty(((MaterialEntry)value).Object);
                if(((MaterialEntry)value).Object is float)
                Pad(0x8);
            }
            else if (value is Formats.Materials.MatlAttribute.MatlString)
            {
                // special write function for matl string
                Write((long)8);
                value = ((Formats.Materials.MatlAttribute.MatlString)value).Text;
                Write(((string)value).ToCharArray());
                Write((byte)0);
                Pad(0x4);
            }
            else if (value is ISSBH_File v)
            {
                WriteSSBHFile(v);
                Pad(0x8);
            }
            else if (t.IsEnum)
                Write((long)((int)value));
            else if (t == typeof(bool))
                Write((bool)value ? (long)1 : (long)0);
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
                Write(((string)value).ToCharArray());
                Write((byte)0);
                Pad(0x4); 
            }
            else
                throw new NotSupportedException($"{t} is not a supported type.");
        }

        public void Pad(int toSize, byte paddingValue = 0)
        {
            while (BaseStream.Position % toSize != 0)
            {
                Write(paddingValue);
            }
        }
    }
}
