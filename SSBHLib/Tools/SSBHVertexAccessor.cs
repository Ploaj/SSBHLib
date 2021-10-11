using SSBHLib.Formats.Meshes;
using SSBHLib.IO;
using System;
using System.IO;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Helper class for reading vertex information out of a MESH file
    /// </summary>
    public class SsbhVertexAccessor
    {
        private readonly Mesh meshFile;

        /// <summary>
        /// Creates a new vertex accessor for given mesh file
        /// </summary>
        /// <param name="meshFile"></param>
        public SsbhVertexAccessor(Mesh meshFile)
        {
            if (meshFile == null)
                return;

            this.meshFile = meshFile;
        }

        /// <summary>
        /// Returns the mesh attribute from the given attribute name or <c>null</c> if not found.
        /// </summary>
        /// <param name="attributeName">The name of the attribute</param>
        /// <param name="meshObject">The mesh to search</param>
        /// <returns>The mesh attribute for the given attribute name or <c>null</c> if not found.</returns>
        private MeshAttribute GetAttribute(string attributeName, MeshObject meshObject)
        {
            foreach (MeshAttribute a in meshObject.Attributes)
            {
                foreach (var s in a.AttributeStrings)
                {
                    if (s.Text.Equals(attributeName))
                    {
                        return a;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Reads all the triangle indices from the given mesh object.
        /// </summary>
        /// <param name="meshObject">The mesh containing the indices</param>
        /// <returns>All the triangle indices from the given mesh object</returns>
        public uint[] ReadIndices(MeshObject meshObject)
        {
            return ReadIndices(0, meshObject.IndexCount, meshObject);
        }

        /// <summary>
        /// Reads the triangle indices from the given mesh object starting from <paramref name="startPosition"/>.
        /// </summary>
        /// <param name="startPosition">The offset in number of elements to start reading from</param>
        /// <param name="count">The number of indices to read</param>
        /// <param name="meshObject">The mesh containing the indices</param>
        /// <returns>All the triangle indices from the given mesh object</returns>
        public uint[] ReadIndices(int startPosition, int count, MeshObject meshObject)
        {
            // TODO: Add option to not convert smaller types to larger types.
            using (var indexReader = new BinaryReader(new MemoryStream(meshFile.PolygonBuffer)))
            {
                if (meshObject.DrawElementType == DrawElementType.UnsignedInt)
                {
                    indexReader.BaseStream.Position = meshObject.ElementOffset + (startPosition * sizeof(uint));
                    return indexReader.ReadStructs<uint>(count);
                }
                else
                {
                    indexReader.BaseStream.Position = meshObject.ElementOffset + (startPosition * sizeof(ushort));
                    uint[] indices = new uint[count];
                    for (int i = 0; i < count; i++)
                    {
                        indices[i] = indexReader.ReadUInt16();
                    }
                    return indices;
                }
            }

        }

        /// <summary>
        /// Reads the vertex attribute information for the given attribute name inside of the mesh object.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="meshObject"></param>
        /// <returns>An empty array if <paramref name="attributeName"/> does not exist</returns>
        public SsbhVertexAttribute[] ReadAttribute(string attributeName, MeshObject meshObject)
        {
            return ReadAttribute(attributeName, meshObject.VertexCount, meshObject);
        }

        /// <summary>
        /// Reads the vertex attribute information for the given attribute name inside of the mesh object.
        /// </summary>
        /// <param name="attribute">The vertex attribute to read</param>
        /// <param name="meshObject">The mesh containing the attribute data</param>
        /// <returns>An empty array if <paramref name="attribute"/> does not exist</returns>
        public SsbhVertexAttribute[] ReadAttribute(MeshAttribute attribute, MeshObject meshObject)
        {
            return ReadAttribute(attribute, meshObject.VertexCount, meshObject);
        }

        /// <summary>
        /// Reads the attribute data from the mesh object with the given name
        /// </summary>
        /// <param name="attributeName">The name of the vertex attribute</param>
        /// <param name="count">The number of elements to read</param>
        /// <param name="meshObject">The mesh containing the attribute data</param>
        /// <returns>An empty array if <paramref name="attributeName"/> does not exist</returns>
        public SsbhVertexAttribute[] ReadAttribute(string attributeName, int count, MeshObject meshObject)
        {
            MeshAttribute attr = GetAttribute(attributeName, meshObject);

            return ReadAttribute(attr, count, meshObject);
        }

        private SsbhVertexAttribute[] ReadAttribute(MeshAttribute attr, int count, MeshObject meshObject)
        {
            if (attr == null)
            {
                return new SsbhVertexAttribute[0];
            }

            string attributeName = attr.AttributeStrings[0].Text;

            // TODO: There are optimizations possible for reading if the data is tighly packed.
            // The stride may not allow this.
            int offset = meshObject.VertexOffset;
            int stride = meshObject.Stride0;
            if (attr.BufferIndex == 1)
            {
                offset = meshObject.VertexOffset2;
                stride = meshObject.Stride2;
            }

            SsbhVertexAttribute[] attributes = null;
            int attributeLength = GetAttributeComponentCount(attributeName);

            using (BinaryReader attributeBuffer = new BinaryReader(new MemoryStream(meshFile.VertexBuffers[attr.BufferIndex].Buffer)))
            {
                attributes = ReadAttribute(attr, count, attributeBuffer, offset, stride, attributeLength);
            }

            return attributes;
        }

        private SsbhVertexAttribute[] ReadAttribute(MeshAttribute attr, int count, BinaryReader buffer, int offset, int stride, int attributeLength)
        {
            var format = attr.DataType;
            var attributes = new SsbhVertexAttribute[count];

            for (int i = 0; i < count; i++)
            {
                buffer.BaseStream.Position = offset + attr.BufferOffset + stride * i;

                attributes[i] = new SsbhVertexAttribute();

                switch (format)
                {
                    // TODO: Add option to not convert smaller types to larger types.
                    case MeshAttribute.AttributeDataType.Byte4:
                        for (int j = 0; j < attributeLength; j++)
                        {
                            attributes[i][j] = buffer.ReadByte();
                        }
                        break;
                    case MeshAttribute.AttributeDataType.Float3:
                        for (int j = 0; j < attributeLength; j++)
                        {
                            attributes[i][j] = buffer.ReadSingle();
                        }
                        break;
                    case MeshAttribute.AttributeDataType.HalfFloat4:
                        for (int j = 0; j < attributeLength; j++)
                        {
                            attributes[i][j] = ReadHalfFloat(buffer);
                        }
                        break;
                    case MeshAttribute.AttributeDataType.HalfFloat2:
                        for (int j = 0; j < attributeLength; j++)
                        {
                            attributes[i][j] = ReadHalfFloat(buffer);
                        }
                        break;
                }
            }

            return attributes;
        }

        public static int GetAttributeComponentCount(string attributeName)
        {
            int attributeSize = 3;
            if (attributeName.Contains("colorSet") || attributeName.Equals("Normal0") || attributeName.Equals("Tangent0"))
                attributeSize = 4;
            if (attributeName.Equals("map1") || attributeName.Equals("bake1") || attributeName.Contains("uvSet"))
                attributeSize = 2;
            return attributeSize;
        }

        private float ReadHalfFloat(BinaryReader reader)
        {
            int hbits = reader.ReadInt16();

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
    }
}
