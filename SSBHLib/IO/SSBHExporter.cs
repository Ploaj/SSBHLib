using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

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

        private class ObjectWriteInfo
        {
            public object Data { get; }
            public uint? RelativeOffsetStartPosition { get; }

            public ObjectWriteInfo(object data, uint? relativeOffsetStartPosition = null)
            {
                Data = data;
                RelativeOffsetStartPosition = relativeOffsetStartPosition;
            }
        }

        public long Position => BaseStream.Position; 
        public long FileSize => BaseStream.Length;

        private Queue<ObjectWriteInfo> objectWriteQueue = new Queue<ObjectWriteInfo>();

        public SsbhExporter(Stream stream) : base(stream)
        {

        }

        public SsbhExporter(string fileName) : this(new FileStream(fileName, FileMode.Create))
        {

        }

        public void WriteSsbhFile(SsbhFile file)
        {
            // The header is 16-byte aligned.
            Write(new char[] { 'H', 'B', 'S', 'S'});
            // TODO: This value is always present.
            Write(0x40);
            Pad(0x10);

            // write file contents
            AddSsbhFile(file);
            // TODO: padding?
            //exporter.Pad(0x4);
        }

        private void AddSsbhFile(SsbhFile file)
        {
            objectWriteQueue.Enqueue(new ObjectWriteInfo(file));

            while (objectWriteQueue.Count > 0)
            {
                var writeInfo = objectWriteQueue.Dequeue();
                if (writeInfo.Data == null)
                    continue;

                // 8-byte alignment for arrays and matl data objects.
                if (writeInfo.Data is Array || (writeInfo.Data is MaterialEntry entry && entry.Object is MatlAttribute.MatlString))
                    Pad(0x8);

                // 4-byte alignment for strings.
                if (writeInfo.Data is string)
                    Pad(0x4);

                // Fill in the temporary relative offset with the correct value.
                if (writeInfo.RelativeOffsetStartPosition.HasValue)
                    WriteRelativeOffset(writeInfo);

                var isLastWrite = objectWriteQueue.Count == 0;
                if (writeInfo.Data is Array array)
                    WriteArray(array, isLastWrite);
                else
                    WriteProperty(writeInfo.Data, isLastWrite);
            }
        }

        private void WriteArray(Array array, bool isLastWrite)
        {
            if (array.GetType() == typeof(byte[]))
            {
                Write((byte[])array);
            }
            else
            {
                // Ensure the elements of this array get written before anything already in the queue.
                var objectQueueTemp = objectWriteQueue;
                objectWriteQueue = new Queue<ObjectWriteInfo>();
                foreach (object o in array)
                {
                    WriteProperty(o, isLastWrite);
                }
                foreach (var o in objectQueueTemp)
                    objectWriteQueue.Enqueue(o);
            }
        }

        private void WriteRelativeOffset(ObjectWriteInfo obj)
        {
            long currentPosition = Position;
            BaseStream.Position = obj.RelativeOffsetStartPosition.Value;

            // Calculate a relative offset based on the previous position.
            Write(currentPosition - obj.RelativeOffsetStartPosition.Value);

            BaseStream.Position = currentPosition;
        }

        private void WriteSsbh(SsbhFile file, bool isLastWrite)
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
                    objectWriteQueue.Enqueue(new ObjectWriteInfo(prop.GetValue(file), (uint)Position));
                    // Write placeholder relative offset.
                    Write(0L);
                }
                else if (prop.PropertyType.IsArray)
                {
                    var array = (prop.GetValue(file) as Array);
                    if (array.Length > 0)
                        objectWriteQueue.Enqueue(new ObjectWriteInfo(array, (uint)Position));
                    else
                        objectWriteQueue.Enqueue(new ObjectWriteInfo(array));

                    // Write placeholder relative offset.
                    Write(0L);
                    Write((long)array.Length);
                }
                else if (prop.PropertyType == typeof(SsbhOffset)) 
                {
                    // HACK: for materials
                    var dataObject = file.GetType().GetProperty("DataObject").GetValue(file);
                    var matEntry = new MaterialEntry(dataObject);
                    objectWriteQueue.Enqueue(new ObjectWriteInfo(matEntry, (uint)Position));
                    // Write placeholder relative offset.
                    Write(0L);
                }
                else
                {
                    WriteProperty(prop.GetValue(file), isLastWrite);
                }
            }
        }

        private void WriteProperty(object value, bool isLastWrite)
        {
            Type t = value.GetType();
            if (value is MaterialEntry entry)
            {
                WriteProperty(entry.Object, isLastWrite);
                // Floats are 8-byte aligned for MATL?
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
                WriteSsbh(v, isLastWrite);
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
                WriteString((string)value, isLastWrite);
            else if (t == typeof(Vector3))
                WriteVector3((Vector3)value);
            else if (t == typeof(Vector4))
                WriteVector4((Vector4)value);
            else if (t == typeof(Matrix3x3))
                WriteMatrix3x3((Matrix3x3)value);
            else if (t == typeof(Matrix4x4))
                WriteMatrix4x4((Matrix4x4)value);
            else
                throw new NotSupportedException($"{t} is not a supported type.");
        }

        private void WriteMatrix3x3(Matrix3x3 value)
        {
            WriteVector3(value.Row1);
            WriteVector3(value.Row2);
            WriteVector3(value.Row3);
        }

        private void WriteMatrix4x4(Matrix4x4 value)
        {
            WriteVector4(value.Row1);
            WriteVector4(value.Row2);
            WriteVector4(value.Row3);
            WriteVector4(value.Row4);
        }

        private void WriteVector3(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        private void WriteVector4(Vector4 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        private void WriteString(string text, bool isLastWrite = false)
        {
            // 4-byte aligned c string
            Write(text.ToCharArray());
            Write((byte)0);
            if (!isLastWrite)
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
