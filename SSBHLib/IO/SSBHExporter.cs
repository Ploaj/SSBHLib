using SSBHLib.Formats.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SSBHLib.IO
{
    public class SsbhExporter : BinaryWriter
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

        // Necessary for objects with offsets (strings, arrays, and matl data objects).
        private Queue<object> ssbhObjectWriteQueue = new Queue<object>();

        private readonly Dictionary<uint, object> objectByPositionBeforeRelativeOffset = new Dictionary<uint, object>();

        public SsbhExporter(Stream stream) : base(stream)
        {

        }

        public static void WriteSsbhFile(string fileName, SsbhFile file, bool writeHeader = true)
        {
            using (SsbhExporter exporter = new SsbhExporter(new FileStream(fileName, FileMode.Create)))
            {
                // write ssbh header
                if (writeHeader)
                {
                    // The header is 16-byte aligned.
                    exporter.Write(new char[] { 'H', 'B', 'S', 'S'});
                    // TODO: This value is always present.
                    exporter.Write(0x40);
                    exporter.Pad(0x10);
                }

                // write file contents
                exporter.AddSsbhFile(file);
                exporter.Pad(4);
            }
        }

        private void AddSsbhFile(SsbhFile file)
        {
            ssbhObjectWriteQueue.Enqueue(file);

            while (ssbhObjectWriteQueue.Count > 0)
            {
                var obj = ssbhObjectWriteQueue.Dequeue();
                if (obj == null)
                    continue;

                // 8-byte alignment for arrays and matl data objects.
                if (obj is Array || (obj is MaterialEntry entry && entry.Object is MatlAttribute.MatlString))
                    Pad(0x8);

                // 4-byte alignment for strings.
                if (obj is string)
                    Pad(0x4);

                // Fill in the temporary relative offset with the correct value.
                if (objectByPositionBeforeRelativeOffset.ContainsValue(obj))
                    WriteRelativeOffset(obj);

                if (obj is Array array)
                    WriteArray(array);
                else
                    WriteProperty(obj);
            }
        }

        private void WriteArray(Array array)
        {
            if (array.GetType() == typeof(byte[]))
            {
                foreach (byte o in array)
                {
                    Write(o);
                }
            }
            else
            {
                // Ensure the elements of this array get written before anything already in the queue.
                Queue<object> objectQueueTemp = ssbhObjectWriteQueue;
                ssbhObjectWriteQueue = new Queue<object>();
                foreach (object o in array)
                {
                    WriteProperty(o);
                }
                foreach (object o in objectQueueTemp)
                    ssbhObjectWriteQueue.Enqueue(o);
            }
        }

        private void WriteRelativeOffset(object obj)
        {
            long relativeOffsetStartPosition = objectByPositionBeforeRelativeOffset.FirstOrDefault(x => x.Value == obj).Key;
            if (relativeOffsetStartPosition != 0)
            {
                long currentPosition = Position;
                BaseStream.Position = relativeOffsetStartPosition;

                // Calculate a relative offset based on the previous position.
                Write(currentPosition - relativeOffsetStartPosition);

                BaseStream.Position = currentPosition;
                objectByPositionBeforeRelativeOffset.Remove((uint)relativeOffsetStartPosition);
            }
        }

        private void WriteSsbhFile(SsbhFile file)
        {
            foreach (var prop in file.GetType().GetProperties())
            {
                // Some more matl hacks.
                if (ParseTag.ShouldSkipProperty(prop))
                    continue;

                // Check for types that use offsets.
                if (prop.PropertyType == typeof(string))
                {
                    // TODO: Is this check necessary?
                    if (prop.GetValue(file) == null)
                    {
                        // Write placeholder relative offset.
                        Write(0L);
                        continue;
                    }
                    ssbhObjectWriteQueue.Enqueue(prop.GetValue(file));
                    objectByPositionBeforeRelativeOffset.Add((uint)Position, prop.GetValue(file));
                    // Write placeholder relative offset.
                    Write(0L);
                }
                else if (prop.PropertyType.IsArray)
                {
                    var array = (prop.GetValue(file) as Array);
                    if (array.Length > 0)
                        objectByPositionBeforeRelativeOffset.Add((uint)Position, array);
                    ssbhObjectWriteQueue.Enqueue(array);
                    // Write placeholder relative offset.
                    Write(0L);
                    Write((long)array.Length);
                }
                else if (prop.PropertyType == typeof(SsbhOffset)) 
                {
                    // HACK: for materials
                    var dataObject = file.GetType().GetProperty("DataObject").GetValue(file);
                    var matentry = new MaterialEntry(dataObject);
                    objectByPositionBeforeRelativeOffset.Add((uint)Position, matentry);
                    ssbhObjectWriteQueue.Enqueue(matentry);
                    // Write placeholder relative offset.
                    Write(0L);
                }
                else
                {
                    WriteProperty(prop.GetValue(file));
                }
            }
        }

        private void WriteProperty(object value)
        {
            Type t = value.GetType();
            if (value is MaterialEntry entry)
            {
                WriteProperty(entry.Object);
                // Floats are 8-byte aligned?
                if (entry.Object is float)
                    Pad(0x8);
            }
            else if (value is MatlAttribute.MatlString matlString)
            {
                // special write function for matl string
                // TODO: matl strings always have a relative offset of 8?
                Write((long)8);
                WriteString(matlString.Text);
            }
            else if (value is SsbhFile v)
            {
                WriteSsbhFile(v);
                Pad(0x8);
            }
            else if (t.IsEnum)
                WriteEnum(t, value);
            else if (t == typeof(bool))
                Write((bool)value ? 1L : 0L);
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
                WriteString((string)value);
            }
            else
                throw new NotSupportedException($"{t} is not a supported type.");
        }

        private void WriteString(string text)
        {
            // 4-byte aligned c string
            Write(text.ToCharArray());
            Write((byte)0);
            Pad(0x4);
        }

        private void WriteEnum(Type enumType, object value)
        {
            if (enumType.GetEnumUnderlyingType() == typeof(int))
                Write((int)value);
            else if (enumType.GetEnumUnderlyingType() == typeof(uint))
                Write((uint)value);
            else if (enumType.GetEnumUnderlyingType() == typeof(ulong))
                Write((ulong)value);
            else
                throw new NotImplementedException();
        }

        private void Pad(int toSize, byte paddingValue = 0)
        {
            while (BaseStream.Position % toSize != 0)
            {
                Write(paddingValue);
            }
        }
    }
}
