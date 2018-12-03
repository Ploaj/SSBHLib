using System;
using System.IO;
using SSBHLib.Formats;

namespace SSBHLib.Tools
{
    public struct SSBHVertexAttribute
    {
        public float X, Y, Z, W;
    }

    public class SSBHVertexAccessor : IDisposable
    {
        private BinaryReader[] Buffers;
        private BinaryReader IndexBuffer;
        
        private MESH MeshFile;

        public SSBHVertexAccessor(MESH MeshFile)
        {
            if (MeshFile == null) return;

            this.MeshFile = MeshFile;

            Buffers = new BinaryReader[MeshFile.VertexBuffers.Length];
            for(int i = 0; i < MeshFile.VertexBuffers.Length; i++)
            {
                Buffers[i] = new BinaryReader(new MemoryStream(MeshFile.VertexBuffers[i].Buffer));
            }
            IndexBuffer = new BinaryReader(new MemoryStream(MeshFile.PolygonBuffer));
        }

        private MESH_Attribute GetAttribute(string AttributeName, MESH_Object MeshObject)
        {
            foreach (MESH_Attribute a in MeshObject.Attributes)
            {
                foreach (MESH_AttributeString s in a.AttributeStrings)
                {
                    if (s.Name.Equals(AttributeName))
                    {
                        return a;
                    }
                }
            }
            return null;
        }

        public uint[] ReadIndices(int Position, int Count, MESH_Object MeshObject)
        {
            uint[] indicies = new uint[Count];

            IndexBuffer.BaseStream.Position = MeshObject.ElementOffset + Position * (MeshObject.DrawElementType == 1 ? 4 : 2);
            for(int i = 0; i < Count; i++)
            {
                indicies[i] = MeshObject.DrawElementType == 1 ? IndexBuffer.ReadUInt32() : IndexBuffer.ReadUInt16();
            }
            return indicies;
        }

        public SSBHVertexAttribute[] ReadAttribute(string AttributeName, int Position, int Count, MESH_Object MeshObject)
        {
            MESH_Attribute attr = GetAttribute(AttributeName, MeshObject);

            if(attr == null)
            {
                return new SSBHVertexAttribute[0];
            }
            BinaryReader SelectedBuffer = Buffers[attr.BufferIndex];

            int Offset = MeshObject.VertexOffset;
            int Stride = MeshObject.Stride;
            if(attr.BufferIndex == 1)
            {
                Offset = MeshObject.VertexOffset2;
                Stride = MeshObject.Stride2;
            }

            int Size = 3;
            if (AttributeName.Contains("colorSet"))
                Size = 4;
            if (AttributeName.Equals("map1") || AttributeName.Contains("uvSet"))
                Size = 2;

            SSBHVertexAttribute[] a = new SSBHVertexAttribute[Count];
            for(int i = 0; i < Count; i++)
            {
                SelectedBuffer.BaseStream.Position = Offset + attr.BufferOffset + Stride * (Position + i);
                a[i] = new SSBHVertexAttribute();

                if (Size > 0)
                    a[i].X = ReadAttribute(SelectedBuffer, (SSBVertexAttribFormat)attr.DataType);
                if (Size > 1)
                    a[i].Y = ReadAttribute(SelectedBuffer, (SSBVertexAttribFormat)attr.DataType);
                if (Size > 2)
                    a[i].Z = ReadAttribute(SelectedBuffer, (SSBVertexAttribFormat)attr.DataType);
                if (Size > 3)
                    a[i].W = ReadAttribute(SelectedBuffer, (SSBVertexAttribFormat)attr.DataType);
            }

            return a;
        }

        private float ReadAttribute(BinaryReader buffer, SSBVertexAttribFormat format)
        {
            switch (format)
            {
                case SSBVertexAttribFormat.Byte: return buffer.ReadByte();
                case SSBVertexAttribFormat.Float: return buffer.ReadSingle();
                case SSBVertexAttribFormat.HalfFloat: return ReadHalfFloat(buffer);
                case SSBVertexAttribFormat.HalfFloat2: return ReadHalfFloat(buffer);
                default: return buffer.ReadByte();
            }
        }

        private float ReadHalfFloat(BinaryReader Reader)
        {
            int hbits = Reader.ReadInt16();

            int mant = hbits & 0x03ff;            // 10 bits mantissa
            int exp = hbits & 0x7c00;            // 5 bits exponent
            if (exp == 0x7c00)                   // NaN/Inf
                exp = 0x3fc00;                    // -> NaN/Inf
            else if (exp != 0)                   // normalized value
            {
                exp += 0x1c000;                   // exp - 15 + 127
                if (mant == 0 && exp > 0x1c400)  // smooth transition
                    return BitConverter.ToSingle(BitConverter.GetBytes((hbits & 0x8000) << 16
                        | exp << 13 | 0x3ff), 0);
            }
            else if (mant != 0)                  // && exp==0 -> subnormal
            {
                exp = 0x1c400;                    // make it normal
                do
                {
                    mant <<= 1;                   // mantissa * 2
                    exp -= 0x400;                 // decrease exp by 1
                } while ((mant & 0x400) == 0); // while not normal
                mant &= 0x3ff;                    // discard subnormal bit
            }                                     // else +/-0 -> +/-0
            return BitConverter.ToSingle(BitConverter.GetBytes(          // combine all parts
                (hbits & 0x8000) << 16          // sign  << ( 31 - 15 )
                | (exp | mant) << 13), 0);         // value << ( 23 - 10 )
        }

        public void Dispose()
        {
            foreach(BinaryReader R in Buffers)
            {
                R.Close();
                R.Dispose();
            }
            IndexBuffer.Close();
            IndexBuffer.Dispose();
        }
    }
}
