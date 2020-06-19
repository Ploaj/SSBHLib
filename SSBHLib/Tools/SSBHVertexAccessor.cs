using System;
using System.IO;
using SSBHLib.Formats.Meshes;
using System.Collections.Generic;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Helper class for reading vertex information out of a MESH file
    /// </summary>
    public class SsbhVertexAccessor
    {
        private readonly Mesh meshFile;

        /// <summary>
        /// Creates a vertex accessor from given MESH filepath
        /// </summary>
        /// <param name="FilePath"></param>
        public SsbhVertexAccessor(string meshFilePath)
        {
            if (Ssbh.TryParseSsbhFile(meshFilePath, out SsbhFile file))
            {
                if (file == null)
                    throw new FileNotFoundException("File was null");

                if (file is Mesh mesh)
                    meshFile = mesh;
                else
                    throw new FormatException("Given file was not a MESH file");
            }
        }

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
                foreach (MeshAttributeString s in a.AttributeStrings)
                {
                    if (s.Name.Equals(attributeName))
                    {
                        return a;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the triangle indices from the given mesh object.
        /// </summary>
        /// <param name="meshObject"></param>
        /// <returns></returns>
        public uint[] ReadIndices(MeshObject meshObject)
        {
            return ReadIndices(0, meshObject.IndexCount, meshObject);
        }

        /// <summary>
        /// Reads the triangle indices from the given mesh object starting from <paramref name="startPosition"/>.
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="count"></param>
        /// <param name="meshObject"></param>
        /// <returns></returns>
        public uint[] ReadIndices(int startPosition, int count, MeshObject meshObject)
        {
            uint[] indices = new uint[count];
            using (var indexBuffer = new BinaryReader(new MemoryStream(meshFile.PolygonBuffer)))
            {
                indexBuffer.BaseStream.Position = meshObject.ElementOffset + startPosition * (meshObject.DrawElementType == 1 ? 4 : 2);
                for (int i = 0; i < count; i++)
                {
                    indices[i] = meshObject.DrawElementType == 1 ? indexBuffer.ReadUInt32() : indexBuffer.ReadUInt16();
                }
            }

            return indices;
        }

        /// <summary>
        /// Reads all attributes from the given mesh object.
        /// </summary>
        /// <returns>A <see cref="Tuple"/> containing the attribute name and its values</returns>
        public Tuple<string, SsbhVertexAttribute[]>[] ReadAttributes(MeshObject meshObject)
        {
            List<Tuple<string, SsbhVertexAttribute[]>> attrs = new List<Tuple<string, SsbhVertexAttribute[]>>(meshObject.Attributes.Length);

            foreach(var attr in meshObject.Attributes)
            {
                foreach(var attrname in attr.AttributeStrings)
                {
                    attrs.Add(new Tuple<string, SsbhVertexAttribute[]>(attrname.Name, ReadAttribute(attr, meshObject)));
                }
            }

            return attrs.ToArray();
        }

        /// <summary>
        /// Reads the vertex attribute information for the given attribute name inside of the mesh object.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="meshObject"></param>
        /// <returns>null if attribute name not found</returns>
        public SsbhVertexAttribute[] ReadAttribute(string attributeName, MeshObject meshObject)
        {
            return ReadAttribute(attributeName, 0, meshObject.VertexCount, meshObject);
        }

        /// <summary>
        /// Reads the vertex attribute information for the given attribute name inside of the mesh object.
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="meshObject"></param>
        /// <returns>null if attribute name not found</returns>
        public SsbhVertexAttribute[] ReadAttribute(MeshAttribute attributeName, MeshObject meshObject)
        {
            return ReadAttribute(attributeName, 0, meshObject.VertexCount, meshObject);
        }

        /// <summary>
        /// Reads the attribute data from the mesh object with the given name
        /// </summary>
        /// <param name="attributeName"></param>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <param name="meshObject"></param>
        /// <returns>null if attribute name not found</returns>
        public SsbhVertexAttribute[] ReadAttribute(string attributeName, int position, int count, MeshObject meshObject)
        {
            MeshAttribute attr = GetAttribute(attributeName, meshObject);

            return ReadAttribute(attr, position, count, meshObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="position"></param>
        /// <param name="count"></param>
        /// <param name="meshObject"></param>
        /// <returns></returns>
        private SsbhVertexAttribute[] ReadAttribute(MeshAttribute attr, int position, int count, MeshObject meshObject)
        {
            if (attr == null)
            {
                return new SsbhVertexAttribute[0];
            }

            string attributeName = attr.AttributeStrings[0].Name;

            var buffers = new BinaryReader[meshFile.VertexBuffers.Length];
            for (int i = 0; i < meshFile.VertexBuffers.Length; i++)
            {
                buffers[i] = new BinaryReader(new MemoryStream(meshFile.VertexBuffers[i].Buffer));
            }

            BinaryReader selectedBuffer = buffers[attr.BufferIndex];

            int offset = meshObject.VertexOffset;
            int stride = meshObject.Stride;
            if (attr.BufferIndex == 1)
            {
                offset = meshObject.VertexOffset2;
                stride = meshObject.Stride2;
            }

            SsbhVertexAttribute[] attributes = null;
            int attributeLength = GetAttributeLength(attributeName);
            switch (attributeLength)
            {
                case 1:
                    attributes = ReadAttributeScalar(attr, position, count, attributeName, selectedBuffer, offset, stride);
                    break;
                case 2:
                    attributes = ReadAttributeVec2(attr, position, count, attributeName, selectedBuffer, offset, stride);
                    break;
                case 3:
                    attributes = ReadAttributeVec3(attr, position, count, attributeName, selectedBuffer, offset, stride);
                    break;
                case 4:
                    attributes = ReadAttributeVec4(attr, position, count, attributeName, selectedBuffer, offset, stride);
                    break;
            }

            foreach (var buffer in buffers)
            {
                buffer.Close();
                buffer.Dispose();
            }

            return attributes;
        }

        // TODO: It may be better to convert bytes to floating point by diving by 255.0f.
        private SsbhVertexAttribute[] ReadAttributeScalar(MeshAttribute attr, int position, int count, string attributeName, BinaryReader buffer, int offset, int stride)
        {
            var format = (SsbVertexAttribFormat)attr.DataType;
            var attributes = new SsbhVertexAttribute[count];

            for (int i = 0; i < count; i++)
            {
                buffer.BaseStream.Position = offset + attr.BufferOffset + stride * (position + i);

                switch (format)
                {
                    case SsbVertexAttribFormat.Byte:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadByte(), 0, 0, 0);
                        break;
                    case SsbVertexAttribFormat.Float:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadSingle(), 0, 0, 0);
                        break;
                    case SsbVertexAttribFormat.HalfFloat:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), 0, 0, 0);
                        break;
                    case SsbVertexAttribFormat.HalfFloat2:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), 0, 0, 0);
                        break;
                }
            }

            return attributes;
        }

        private SsbhVertexAttribute[] ReadAttributeVec2(MeshAttribute attr, int position, int count, string attributeName, BinaryReader buffer, int offset, int stride)
        {
            var format = (SsbVertexAttribFormat)attr.DataType;
            var attributes = new SsbhVertexAttribute[count];

            for (int i = 0; i < count; i++)
            {
                buffer.BaseStream.Position = offset + attr.BufferOffset + stride * (position + i);

                switch (format)
                {
                    case SsbVertexAttribFormat.Byte:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadByte(), buffer.ReadByte(), 0, 0);
                        break;
                    case SsbVertexAttribFormat.Float:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadSingle(), buffer.ReadSingle(), 0, 0);
                        break;
                    case SsbVertexAttribFormat.HalfFloat:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), ReadHalfFloat(buffer), 0, 0);
                        break;
                    case SsbVertexAttribFormat.HalfFloat2:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), ReadHalfFloat(buffer), 0, 0);
                        break;
                }
            }

            return attributes;
        }

        private SsbhVertexAttribute[] ReadAttributeVec3(MeshAttribute attr, int position, int count, string attributeName, BinaryReader buffer, int offset, int stride)
        {
            var format = (SsbVertexAttribFormat)attr.DataType;
            var attributes = new SsbhVertexAttribute[count];

            for (int i = 0; i < count; i++)
            {
                buffer.BaseStream.Position = offset + attr.BufferOffset + stride * (position + i);

                switch (format)
                {
                    case SsbVertexAttribFormat.Byte:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadByte(), buffer.ReadByte(), buffer.ReadByte(), 0);
                        break;
                    case SsbVertexAttribFormat.Float:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), 0);
                        break;
                    case SsbVertexAttribFormat.HalfFloat:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), ReadHalfFloat(buffer), ReadHalfFloat(buffer), 0);
                        break;
                    case SsbVertexAttribFormat.HalfFloat2:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), ReadHalfFloat(buffer), ReadHalfFloat(buffer), 0);
                        break;
                }
            }

            return attributes;
        }

        private SsbhVertexAttribute[] ReadAttributeVec4(MeshAttribute attr, int position, int count, string attributeName, BinaryReader buffer, int offset, int stride)
        {
            var format = (SsbVertexAttribFormat)attr.DataType;
            var attributes = new SsbhVertexAttribute[count];

            for (int i = 0; i < count; i++)
            {
                buffer.BaseStream.Position = offset + attr.BufferOffset + stride * (position + i);

                switch (format)
                {
                    case SsbVertexAttribFormat.Byte:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadByte(), buffer.ReadByte(), buffer.ReadByte(), buffer.ReadByte());
                        break;
                    case SsbVertexAttribFormat.Float:
                        attributes[i] = new SsbhVertexAttribute(buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle(), buffer.ReadSingle());
                        break;
                    case SsbVertexAttribFormat.HalfFloat:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), ReadHalfFloat(buffer), ReadHalfFloat(buffer), ReadHalfFloat(buffer));
                        break;
                    case SsbVertexAttribFormat.HalfFloat2:
                        attributes[i] = new SsbhVertexAttribute(ReadHalfFloat(buffer), ReadHalfFloat(buffer), ReadHalfFloat(buffer), ReadHalfFloat(buffer));
                        break;
                }
            }

            return attributes;
        }

        private static int GetAttributeLength(string attributeName)
        {
            int attributeSize = 3;
            if (attributeName.Contains("colorSet") || attributeName.Equals("Normal0") || attributeName.Equals("Tangent0"))
                attributeSize = 4;
            if (attributeName.Equals("map1") || attributeName.Equals("bake1") || attributeName.Contains("uvSet"))
                attributeSize = 2;
            return attributeSize;
        }

        /// <summary>
        /// Reads attribute from buffer with given data type
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        private float ReadAttribute(BinaryReader buffer, SsbVertexAttribFormat format)
        {
            switch (format)
            {
                case SsbVertexAttribFormat.Byte:
                    return buffer.ReadByte();
                case SsbVertexAttribFormat.Float:
                    return buffer.ReadSingle();
                case SsbVertexAttribFormat.HalfFloat:
                    return ReadHalfFloat(buffer);
                case SsbVertexAttribFormat.HalfFloat2:
                    return ReadHalfFloat(buffer);
                default:
                    return buffer.ReadByte();
            }
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
