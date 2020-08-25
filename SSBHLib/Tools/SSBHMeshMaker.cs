using System;
using System.Collections.Generic;
using System.IO;
using SSBHLib.Formats.Meshes;
using System.Linq;

namespace SSBHLib.Tools
{
    /// <summary>
    /// Helps generate a MESH file
    /// </summary>
    public class SsbhMeshMaker
    {
        private class TempMesh
        {
            public string Name;
            public string ParentBone;
            public int VertexCount;
            public SsbhVertexAttribute BoundingSphere;
            public SsbhVertexAttribute BbMin;
            public SsbhVertexAttribute BbMax;
            public SsbhVertexAttribute ObbCenter;
            public SsbhVertexAttribute ObbSize;
            public float[] ObbMatrix3X3;
            public Dictionary<UltimateVertexAttribute, float[]> VertexData = new Dictionary<UltimateVertexAttribute, float[]>();
            public List<uint> Indices = new List<uint>();
            public List<SsbhVertexInfluence> Influences = new List<SsbhVertexInfluence>();
        }

        private TempMesh currentMesh;
        private List<TempMesh> meshes = new List<TempMesh>();

        /// <summary>
        /// Begins creating a new mesh object with given attributes
        /// </summary>
        /// <param name="name">The name of the Mesh Object</param>
        /// <param name="indices">The vertex indices as triangles</param>
        /// <paramref name="positions"/>
        /// <param name="parentBoneName"></param>
        /// <param name="generateBounding"></param>
        public void StartMeshObject(string name, uint[] indices, SsbhVertexAttribute[] positions, string parentBoneName = "", bool generateBounding = false)
        {
            currentMesh = new TempMesh
            {
                Name = name,
                ParentBone = parentBoneName
            };
            currentMesh.Indices.AddRange(indices);
            currentMesh.VertexCount = positions.Length;

            meshes.Add(currentMesh);
            AddAttributeToMeshObject(UltimateVertexAttribute.Position0, positions);

            if (generateBounding)
            {
                //TODO: sphere generation
                BoundingBoxGenerator.GenerateAabb(positions, out SsbhVertexAttribute max, out SsbhVertexAttribute min);
                SetAaBoundingBox(min, max);
                SetOrientedBoundingBox(
                    new SsbhVertexAttribute((max.X + min.X / 2), (max.Y + min.Y / 2), (max.Y + min.Y / 2)),
                    new SsbhVertexAttribute((max.X - min.X), (max.Y - min.Y), (max.Z - min.Z)),
                    new float[] {
                        1, 0, 0,
                        0, 1, 0,
                        0, 0, 1});
            }
        }

        /// <summary>
        /// Sets bounding sphere of current mesh
        /// </summary>
        public void SetBoundingSphere(float x, float y, float z, float r)
        {
            if (currentMesh == null)
                return;
            currentMesh.BoundingSphere = new SsbhVertexAttribute()
            {
                X = x,
                Y = y,
                Z = z,
                W = r
            };
        }

        /// <summary>
        /// Sets the axis aligned bounding box for the current Mesh
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void SetAaBoundingBox(SsbhVertexAttribute min, SsbhVertexAttribute max)
        {
            if (currentMesh == null)
                return;
            currentMesh.BbMax = max;
            currentMesh.BbMin = min;
        }

        /// <summary>
        /// Sets the oriented bounding box for the current Mesh
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="matrix3X3"></param>
        public void SetOrientedBoundingBox(SsbhVertexAttribute center, SsbhVertexAttribute size, float[] matrix3X3)
        {
            if (currentMesh == null)
                return;
            if (matrix3X3 == null)
                return;
            if (matrix3X3.Length != 9)
                throw new IndexOutOfRangeException("Matrix must contain 9 entries in row major order");
            currentMesh.ObbCenter = center;
            currentMesh.ObbSize = size;
            currentMesh.ObbMatrix3X3 = matrix3X3;
        }

        /// <summary>
        /// Adds new attribute data to the mesh object
        /// Note: must call <see cref="StartMeshObject(string, uint[], SsbhVertexAttribute[], string, bool)"/> first.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="inputValues"></param>
        public void AddAttributeToMeshObject(UltimateVertexAttribute attribute, SsbhVertexAttribute[] inputValues)
        {
            // TODO: Why must StartMeshObject be called?
            if (currentMesh == null)
                return;

            int componentCount = attribute.ComponentCount;
            float[] values = new float[inputValues.Length * componentCount];
            for (int i = 0; i < inputValues.Length; i++)
            {
                // TODO: Add brackets operator to SsbhVertexAttribute.
                // Ex: attribute[0] == attribute.X
                if (componentCount > 0)
                    values[i * componentCount + 0] = inputValues[i].X;
                if (componentCount > 1)
                    values[i * componentCount + 1] = inputValues[i].Y;
                if (componentCount > 2)
                    values[i * componentCount + 2] = inputValues[i].Z;
                if (componentCount > 3)
                    values[i * componentCount + 3] = inputValues[i].W;
            }

            currentMesh.VertexData.Add(attribute, values);
        }

        /// <summary>
        /// Attaches rigging information to mesh object
        /// Note: must call StartMeshObject first
        /// </summary>
        public void AttachRiggingToMeshObject(SsbhVertexInfluence[] influences)
        {
            currentMesh?.Influences.AddRange(influences);
        }

        /// <summary>
        /// Creates and returns a mesh file
        /// </summary>
        /// <returns></returns>
        public Mesh GetMeshFile()
        {
            Mesh mesh = new Mesh
            {
                //TODO: bounding box stuff
                // Rigging
                Objects = new MeshObject[meshes.Count]
            };

            // create mesh objects and buffers
            BinaryWriter vertexBuffer1 = new BinaryWriter(new MemoryStream());
            BinaryWriter vertexBuffer2 = new BinaryWriter(new MemoryStream()); // there are actually 4 buffers, but only 2 seem to be used
            BinaryWriter indexBuffer = new BinaryWriter(new MemoryStream());
            int finalBufferOffset = 0;
            int meshIndex = 0;
            Dictionary<string, int> meshGroups = new Dictionary<string, int>();
            List<MeshRiggingGroup> riggingGroups = new List<MeshRiggingGroup>();
            foreach (var tempmesh in meshes)
            {
                MeshObject mo = new MeshObject
                {
                    Name = tempmesh.Name
                };
                if (meshGroups.ContainsKey(mo.Name))
                    meshGroups[mo.Name] += 1;
                else
                    meshGroups.Add(mo.Name, 0);

                mo.SubIndex = meshGroups[mo.Name];
                mo.BoundingSphereCenter = new Formats.Vector3 { X = tempmesh.BoundingSphere.X, Y = tempmesh.BoundingSphere.Y, Z = tempmesh.BoundingSphere.Z };
                mo.BoundingSphereRadius = tempmesh.BoundingSphere.W;

                mo.BoundingBoxMax = new Formats.Vector3 { X = tempmesh.BbMax.X, Y = tempmesh.BbMax.Y, Z = tempmesh.BbMax.Z };
                mo.BoundingBoxMin = new Formats.Vector3 { X = tempmesh.BbMin.X, Y = tempmesh.BbMin.Y, Z = tempmesh.BbMin.Z };
                mo.OrientedBoundingBoxCenter = new Formats.Vector3 { X = tempmesh.ObbCenter.X, Y = tempmesh.ObbCenter.Y, Z = tempmesh.ObbCenter.Z };

                mo.OrientedBoundingBoxSize = new Formats.Vector3 { X = tempmesh.ObbSize.X, Y = tempmesh.ObbSize.Y, Z = tempmesh.ObbSize.Z };

                mo.OrientedBoundingBoxTransform.Row1.X = tempmesh.ObbMatrix3X3[0];
                mo.OrientedBoundingBoxTransform.Row1.Y = tempmesh.ObbMatrix3X3[1];
                mo.OrientedBoundingBoxTransform.Row1.Z = tempmesh.ObbMatrix3X3[2];
                mo.OrientedBoundingBoxTransform.Row2.X = tempmesh.ObbMatrix3X3[3];
                mo.OrientedBoundingBoxTransform.Row2.Y = tempmesh.ObbMatrix3X3[4];
                mo.OrientedBoundingBoxTransform.Row2.Z = tempmesh.ObbMatrix3X3[5];
                mo.OrientedBoundingBoxTransform.Row3.X = tempmesh.ObbMatrix3X3[6];
                mo.OrientedBoundingBoxTransform.Row3.Y = tempmesh.ObbMatrix3X3[7];
                mo.OrientedBoundingBoxTransform.Row3.Z = tempmesh.ObbMatrix3X3[8];


                // Create Rigging
                riggingGroups.Add(SsbhRiggingCompiler.CreateRiggingGroup(mo.Name, (int)mo.SubIndex, tempmesh.Influences.ToArray()));

                // set object
                mesh.Objects[meshIndex++] = mo;

                mo.ParentBoneName = tempmesh.ParentBone;
                if (tempmesh.Influences.Count > 0 && (tempmesh.ParentBone == null || tempmesh.ParentBone.Equals("")))
                    mo.RiggingType = RiggingType.Regular;

                int stride1 = 0;
                int stride2 = 0;

                mo.VertexOffset = (int)vertexBuffer1.BaseStream.Length;
                mo.VertexOffset2 = (int)vertexBuffer2.BaseStream.Length;
                mo.ElementOffset = (uint)indexBuffer.BaseStream.Length;

                // gather strides
                mo.Attributes = new MeshAttribute[tempmesh.VertexData.Count];
                int attributeIndex = 0;
                foreach (var keypair in tempmesh.VertexData)
                {
                    // For some reason the attribute string doesn't match the attribute's name for Tangent0.
                    var attributeName = keypair.Key.Name;
                    if (keypair.Key.Name == "Tangent0")
                        attributeName = "map1";

                    MeshAttribute attr = new MeshAttribute
                    {
                        Name = attributeName,
                        Index = keypair.Key.Index,
                        BufferIndex = keypair.Key.BufferIndex,
                        DataType = keypair.Key.DataType,
                        BufferOffset = keypair.Key.BufferIndex == 0 ? stride1 : stride2,
                        AttributeStrings = new MeshAttributeString[] { new MeshAttributeString() { Name = keypair.Key.Name } }
                    };
                    mo.Attributes[attributeIndex++] = attr;

                    if (keypair.Key.BufferIndex == 0)
                        stride1 += keypair.Key.ComponentCount * keypair.Key.DataType.GetSizeInBytes();
                    else
                        stride2 += keypair.Key.ComponentCount * keypair.Key.DataType.GetSizeInBytes();
                }

                // now that strides are known...
                long buffer1Start = vertexBuffer1.BaseStream.Length;
                long buffer2Start = vertexBuffer2.BaseStream.Length;
                vertexBuffer1.Write(new byte[stride1 * tempmesh.VertexCount]);
                vertexBuffer2.Write(new byte[stride2 * tempmesh.VertexCount]);
                attributeIndex = 0;
                foreach (var keypair in tempmesh.VertexData)
                {
                    var attr = mo.Attributes[attributeIndex++];
                    float[] data = keypair.Value;
                    var buffer = attr.BufferIndex == 0 ? vertexBuffer1 : vertexBuffer2;
                    int bufferOffset = (int)(attr.BufferIndex == 0 ? buffer1Start : buffer2Start);
                    int stride = (attr.BufferIndex == 0 ? stride1 : stride2);
                    int componentCount = keypair.Key.ComponentCount;
                    for (int vertexIndex = 0; vertexIndex < tempmesh.VertexCount; vertexIndex++)
                    {
                        buffer.Seek(bufferOffset + stride * vertexIndex + attr.BufferOffset, SeekOrigin.Begin);
                        for (int j = 0; j < componentCount; j++)
                        {
                            WriteType(buffer, attr.DataType, data[vertexIndex * componentCount + j]);
                        }
                    }
                    // seek to end just to make sure
                    buffer.Seek((int)buffer.BaseStream.Length, SeekOrigin.Begin);
                }

                mo.FinalBufferOffset = finalBufferOffset;
                finalBufferOffset += (4 + stride1) * tempmesh.VertexCount;
                mo.VertexCount = tempmesh.VertexCount;
                mo.IndexCount = tempmesh.Indices.Count;
                mo.Stride = stride1;
                mo.Stride2 = stride2;

                // write index buffer
                if (tempmesh.VertexCount > ushort.MaxValue)
                {
                    mo.DrawElementType = DrawElementType.UnsignedInt;
                    foreach (var i in tempmesh.Indices)
                        indexBuffer.Write(i);
                }
                else
                {
                    foreach (var i in tempmesh.Indices)
                        indexBuffer.Write((ushort)i);
                }
            }

            mesh.PolygonIndexSize = indexBuffer.BaseStream.Length;
            mesh.BufferSizes = new int[] { (int)vertexBuffer1.BaseStream.Length, (int)vertexBuffer2.BaseStream.Length, 0, 0 };
            mesh.PolygonBuffer = ((MemoryStream)indexBuffer.BaseStream).ToArray();
            Console.WriteLine(mesh.PolygonBuffer.Length + " " + indexBuffer.BaseStream.Length);
            mesh.VertexBuffers = new MeshBuffer[]
            {
                new MeshBuffer { Buffer = ((MemoryStream)vertexBuffer1.BaseStream).ToArray() },
                new MeshBuffer { Buffer = ((MemoryStream)vertexBuffer2.BaseStream).ToArray() },
                new MeshBuffer { Buffer = new byte[0] },
                new MeshBuffer { Buffer = new byte[0] }
            };

            mesh.RiggingBuffers = riggingGroups.ToArray().OrderBy(o => o.MeshName, StringComparer.Ordinal).ToArray();

            vertexBuffer1.Close();
            vertexBuffer2.Close();
            indexBuffer.Close();

            return mesh;
        }

        private void WriteType(BinaryWriter writer, MeshAttribute.AttributeDataType type, float value)
        {
            switch (type)
            {
                case MeshAttribute.AttributeDataType.Float:
                    writer.Write(value);
                    break;
                case MeshAttribute.AttributeDataType.Byte:
                    writer.Write((byte)value);
                    break;
                case MeshAttribute.AttributeDataType.HalfFloat:
                    writer.Write((ushort)FloatToHalfFloatBits(value));
                    break;
                case MeshAttribute.AttributeDataType.HalfFloat2:
                    writer.Write((ushort)FloatToHalfFloatBits(value));
                    break;
            }
        }

        private static int SingleToInt32Bits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        private static int FloatToHalfFloatBits(float floatValue)
        {
            int floatBits = SingleToInt32Bits(floatValue);
            int sign = floatBits >> 16 & 0x8000;          // sign only
            int val = (floatBits & 0x7fffffff) + 0x1000; // rounded value

            if (val >= 0x47800000)               // might be or become NaN/Inf
            {                                     // avoid Inf due to rounding
                if ((floatBits & 0x7fffffff) >= 0x47800000)
                {                                 // is or must become NaN/Inf
                    if (val < 0x7f800000)        // was value but too large
                        return sign | 0x7c00;     // make it +/-Inf
                    return sign | 0x7c00 |        // remains +/-Inf or NaN
                        (floatBits & 0x007fffff) >> 13; // keep NaN (and Inf) bits
                }
                return sign | 0x7bff;             // unrounded not quite Inf
            }
            if (val >= 0x38800000)               // remains normalized value
                return sign | val - 0x38000000 >> 13; // exp - 127 + 15
            if (val < 0x33000000)                // too small for subnormal
                return sign;                      // becomes +/-0
            val = (floatBits & 0x7fffffff) >> 23;  // tmp exp for subnormal calc
            return sign | ((floatBits & 0x7fffff | 0x800000) // add subnormal bit
                + (0x800000 >> val - 102)     // round depending on cut off
                >> 126 - val);   // div by 2^(1-(exp-127+15)) and >> 13 | exp=0
        }

    }
}
