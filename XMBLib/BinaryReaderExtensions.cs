using System.IO;
using System.Runtime.InteropServices;

namespace SSBHLib.IO
{
    public static class BinaryReaderExtensions
    {
        public static T[] ReadStructs<T>(this BinaryReader reader, int count) where T : struct
        {
            int sizeOfT = Marshal.SizeOf(typeof(T));

            var buffer = reader.ReadBytes(sizeOfT * count);

            T[] result = new T[count];

            var pinnedHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
            Marshal.Copy(buffer, 0, pinnedHandle.AddrOfPinnedObject(), buffer.Length);
            pinnedHandle.Free();

            return result;
        }

        public static T ReadStruct<T>(this BinaryReader reader) where T : struct
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public static string ReadAscii(this BinaryReader reader, uint offset)
        {
            var originalPosition = reader.BaseStream.Position;

            reader.BaseStream.Position = offset;

            var stringValue = new System.Text.StringBuilder();

            byte b = reader.ReadByte();
            while (b != 0)
            {
                stringValue.Append((char)b);
                b = reader.ReadByte();
            }

            reader.BaseStream.Position = originalPosition;
            return stringValue.ToString();
        }
    }
}
