using System;
using System.Collections.Generic;
using System.IO;
using SSBHLib.Formats.Meshes;
using System.Linq;

namespace SSBHLib.Tools
{
    public enum MESHAttribute
    {
        Position0,
        Normal0,
        Tangent0,
        map1,
        uvSet,
        uvSet1,
        uvSet2,
        colorSet1,
        colorSet2,
        colorSet2_1,
        colorSet2_2,
        colorSet2_3,
        colorSet3,
        colorSet4,
        colorSet5,
        colorSet6,
        colorSet7,
        bake1,
    }

    /// <summary>
    /// Helps generate a MESH file
    /// </summary>
    public class SSBHMeshMaker
    {
        private class TempMesh
        {
            public string Name;
            public string ParentBone;
            public int VertexCount;
            public SSBHVertexAttribute BoundingSphere;
            public SSBHVertexAttribute BBMin;
            public SSBHVertexAttribute BBMax;
            public Dictionary<MESHAttribute, float[]> VertexData = new Dictionary<MESHAttribute, float[]>();
            public List<uint> Indices = new List<uint>();
            public List<SSBHVertexInfluence> Influences = new List<SSBHVertexInfluence>();
        }

        private TempMesh CurrentMesh;
        private List<TempMesh> meshes = new List<TempMesh>();

        /// <summary>
        /// Begins creating a new mesh object with given attributes
        /// </summary>
        /// <param name="name">The name of the Mesh Object</param>
        /// <param name="indices">The vertex indices as triangles</param>
        public void StartMeshObject(string name, uint[] indices, SSBHVertexAttribute[] positions, string parentBoneName = "")
        {
            CurrentMesh = new TempMesh
            {
                Name = name,
                ParentBone = parentBoneName
            };
            CurrentMesh.Indices.AddRange(indices);
            CurrentMesh.VertexCount = positions.Length;

            meshes.Add(CurrentMesh);
            AddAttributeToMeshObject(MESHAttribute.Position0, positions);
        }

        /// <summary>
        /// Sets bounding sphere of current mesh
        /// </summary>
        public void SetBoundingSphere(float x, float y, float z, float r)
        {
            if (CurrentMesh == null)
                return;
            CurrentMesh.BoundingSphere = new SSBHVertexAttribute()
            {
                X = x,
                Y = y,
                Z = z,
                W = r
            };
        }

        /// <summary>
        /// Adds new attribute data to the mesh object
        /// Note: must call StartMeshObject first
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="inputValues"></param>
        public void AddAttributeToMeshObject(MESHAttribute attribute, SSBHVertexAttribute[] inputValues)
        {
            if (CurrentMesh == null)
                return;

            int size = GetAttributeSize(attribute);
            float[] values = new float[inputValues.Length * size];
            for (int i = 0; i < inputValues.Length; i++)
            {
                if (size > 0)
                    values[i * size + 0] = inputValues[i].X;
                if (size > 1)
                    values[i * size + 1] = inputValues[i].Y;
                if (size > 2)
                    values[i * size + 2] = inputValues[i].Z;
                if (size > 3)
                    values[i * size + 3] = inputValues[i].W;
            }

            CurrentMesh.VertexData.Add(attribute, values);
        }

        /// <summary>
        /// Attaches rigging information to mesh object
        /// Note: must call StartMeshObject first
        /// </summary>
        public void AttachRiggingToMeshObject(SSBHVertexInfluence[] influences)
        {
            if (CurrentMesh == null)
                return;
            CurrentMesh.Influences.AddRange(influences);
        }

        /// <summary>
        /// Creates and returns a mesh file
        /// </summary>
        /// <returns></returns>
        public MESH GetMeshFile()
        {
            MESH mesh = new MESH
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
            Dictionary<string, int> MeshGroups = new Dictionary<string, int>();
            List<MeshRiggingGroup> RiggingGroups = new List<MeshRiggingGroup>();
            foreach (var tempmesh in meshes)
            {
                MeshObject mo = new MeshObject
                {
                    Name = tempmesh.Name
                };
                if (MeshGroups.ContainsKey(mo.Name))
                    MeshGroups[mo.Name] += 1;
                else
                    MeshGroups.Add(mo.Name, 0);

                mo.SubMeshIndex = MeshGroups[mo.Name];
                mo.BoundingSphereX = tempmesh.BoundingSphere.X;
                mo.BoundingSphereY = tempmesh.BoundingSphere.Y;
                mo.BoundingSphereZ = tempmesh.BoundingSphere.Z;
                mo.BoundingSphereRadius = tempmesh.BoundingSphere.W;

                // Create Rigging
                RiggingGroups.Add(SSBHRiggingCompiler.CreateRiggingGroup(mo.Name, (int)mo.SubMeshIndex, tempmesh.Influences.ToArray()));

                // set object
                mesh.Objects[meshIndex++] = mo;

                mo.ParentBoneName = tempmesh.ParentBone;
                if (tempmesh.ParentBone == null || tempmesh.ParentBone.Equals(""))
                    mo.HasRigging = 1;
                
                int Stride1 = 0;
                int Stride2 = 0;

                mo.VertexOffset = (int)vertexBuffer1.BaseStream.Length;
                mo.VertexOffset2 = (int)vertexBuffer2.BaseStream.Length;
                mo.ElementOffset = (uint)indexBuffer.BaseStream.Length;

                // gather strides
                mo.Attributes = new MeshAttribute[tempmesh.VertexData.Count];
                int attributeIndex = 0;
                foreach (var keypair in tempmesh.VertexData)
                {
                    MeshAttribute attr = new MeshAttribute
                    {
                        Name = GetAttributeName(keypair.Key),
                        Index = GetAttributeIndex(keypair.Key),
                        BufferIndex = GetBufferIndex(keypair.Key),
                        DataType = GetAttributeDataType(keypair.Key),
                        BufferOffset = (GetBufferIndex(keypair.Key) == 0) ? Stride1 : Stride2,
                        AttributeStrings = new MeshAttributeString[] { new MeshAttributeString() { Name = keypair.Key.ToString() } }
                    };
                    mo.Attributes[attributeIndex++] = attr;

                    if (GetBufferIndex(keypair.Key) == 0)
                        Stride1 += GetAttributeSize(keypair.Key) * GetAttributeDataSize(keypair.Key);
                    else
                        Stride2 += GetAttributeSize(keypair.Key) * GetAttributeDataSize(keypair.Key);
                }

                // now that strides are known...
                long Buffer1Start = vertexBuffer1.BaseStream.Length;
                long Buffer2Start = vertexBuffer2.BaseStream.Length;
                vertexBuffer1.Write(new byte[Stride1 * tempmesh.VertexCount]);
                vertexBuffer2.Write(new byte[Stride2 * tempmesh.VertexCount]);
                attributeIndex = 0;
                foreach (var keypair in tempmesh.VertexData)
                {
                    var attr = mo.Attributes[attributeIndex++];
                    float[] Data = keypair.Value;
                    var buffer = attr.BufferIndex == 0 ? vertexBuffer1 : vertexBuffer2;
                    int bufferOffset = (int)(attr.BufferIndex == 0 ? Buffer1Start : Buffer2Start);
                    int stride = (attr.BufferIndex == 0 ? Stride1 : Stride2);
                    int size = GetAttributeSize(keypair.Key);
                    for (int vertexIndex = 0; vertexIndex < tempmesh.VertexCount; vertexIndex++)
                    {
                        buffer.Seek(bufferOffset + stride * vertexIndex + attr.BufferOffset, SeekOrigin.Begin);
                        for(int j = 0; j < size; j++)
                        {
                            WriteType(buffer, attr.DataType, Data[vertexIndex * size + j]);
                        }
                    }
                    // seek to end just to make sure
                    buffer.Seek((int)buffer.BaseStream.Length, SeekOrigin.Begin);
                }

                mo.FinalBufferOffset = finalBufferOffset;
                finalBufferOffset += (4 + Stride1) * tempmesh.VertexCount;
                mo.VertexCount = tempmesh.VertexCount;
                mo.IndexCount = tempmesh.Indices.Count;
                mo.Stride = Stride1;
                mo.Stride2 = Stride2;
                
                // write index buffer
                if(tempmesh.VertexCount > ushort.MaxValue)
                {
                    mo.DrawElementType = 1;
                    foreach (var i in tempmesh.Indices)
                        indexBuffer.Write(i);
                }else
                {
                    foreach (var i in tempmesh.Indices)
                        indexBuffer.Write((ushort)i);
                }
            }

            mesh.PolygonIndexSize = indexBuffer.BaseStream.Length;
            mesh.BufferSizes = new int[] { (int)vertexBuffer1.BaseStream.Length, (int)vertexBuffer2.BaseStream.Length, 0, 0};
            mesh.PolygonBuffer = ((MemoryStream)indexBuffer.BaseStream).ToArray();
            Console.WriteLine(mesh.PolygonBuffer.Length + " " + indexBuffer.BaseStream.Length);
            mesh.VertexBuffers = new MeshBuffer[]
            {
                new MeshBuffer(){ Buffer = ((MemoryStream)vertexBuffer1.BaseStream).ToArray() },
                new MeshBuffer(){ Buffer = ((MemoryStream)vertexBuffer2.BaseStream).ToArray() },
                new MeshBuffer(){ Buffer = new byte[0] },
                new MeshBuffer(){ Buffer = new byte[0] }
            };

            mesh.RiggingBuffers = RiggingGroups.ToArray().OrderBy(o => o.Name, StringComparer.Ordinal).ToArray();

            vertexBuffer1.Close();
            vertexBuffer2.Close();
            indexBuffer.Close();

            return mesh;
        }

        private void WriteType(BinaryWriter writer, int type, float value)
        {
            switch (type)
            {
                case 0: writer.Write(value); break;
                case 2: writer.Write((byte)value); break;
                case 5: writer.Write((ushort)FromFloat(value)); break;
                case 8: writer.Write((ushort)FromFloat(value)); break;
                default:
                    break;
            }
        }

        private int GetBufferIndex(MESHAttribute attribute)
        {
            switch (attribute)
            {
                case MESHAttribute.Position0:
                case MESHAttribute.Normal0:
                case MESHAttribute.Tangent0:
                    return 0;
                default:
                    return 1;
            }
        }

        public static int GetAttributeSize(MESHAttribute attribute)
        {
            switch (attribute)
            {
                case MESHAttribute.Position0:
                    return 3;
                case MESHAttribute.Normal0:
                    return 4;
                case MESHAttribute.Tangent0:
                    return 4;
                case MESHAttribute.map1:
                case MESHAttribute.uvSet:
                case MESHAttribute.uvSet1:
                case MESHAttribute.uvSet2:
                case MESHAttribute.bake1:
                    return 2;
                case MESHAttribute.colorSet1:
                case MESHAttribute.colorSet2:
                case MESHAttribute.colorSet2_1:
                case MESHAttribute.colorSet2_2:
                case MESHAttribute.colorSet2_3:
                case MESHAttribute.colorSet3:
                case MESHAttribute.colorSet4:
                case MESHAttribute.colorSet5:
                case MESHAttribute.colorSet6:
                case MESHAttribute.colorSet7:
                    return 4;
                default:
                    return 3;
            }
        }

        private string GetAttributeName(MESHAttribute attribute)
        {
            switch (attribute)
            {
                case MESHAttribute.Tangent0:
                    return "map1";
                default:
                    return attribute.ToString();
            }
        }

        private int GetAttributeDataSize(MESHAttribute attribute)
        {
            // TODO: Use enum?
            switch (GetAttributeDataType(attribute))
            {
                case 0:
                    return 4;
                case 2:
                    return 1;
                case 5:
                    return 2;
                case 8:
                    return 2;
                default:
                    return 1;
            }
        }

        private int GetAttributeDataType(MESHAttribute attribute)
        {
            // TODO: Use enum?
            switch (attribute)
            {
                case MESHAttribute.Position0:
                    return 0;
                case MESHAttribute.Normal0:
                case MESHAttribute.Tangent0:
                    return 5;
                case MESHAttribute.map1:
                case MESHAttribute.uvSet:
                case MESHAttribute.uvSet1:
                case MESHAttribute.uvSet2:
                case MESHAttribute.bake1:
                    return 8;
                case MESHAttribute.colorSet1:
                case MESHAttribute.colorSet2:
                case MESHAttribute.colorSet2_1:
                case MESHAttribute.colorSet2_2:
                case MESHAttribute.colorSet2_3:
                case MESHAttribute.colorSet3:
                case MESHAttribute.colorSet4:
                case MESHAttribute.colorSet5:
                case MESHAttribute.colorSet6:
                case MESHAttribute.colorSet7:
                    return 2;
                default:
                    return -1;
            }
        }

        private int GetAttributeIndex(MESHAttribute attribute)
        {
            switch (attribute)
            {
                case MESHAttribute.Position0:
                    return 0;
                case MESHAttribute.Normal0:
                    return 1;
                case MESHAttribute.Tangent0:
                    return 3;
                case MESHAttribute.map1:
                    return 4;
                case MESHAttribute.uvSet:
                case MESHAttribute.uvSet1:
                case MESHAttribute.uvSet2:
                case MESHAttribute.bake1:
                case MESHAttribute.colorSet1:
                    return 5;
                case MESHAttribute.colorSet2:
                case MESHAttribute.colorSet2_1:
                case MESHAttribute.colorSet2_2:
                case MESHAttribute.colorSet2_3:
                case MESHAttribute.colorSet3:
                case MESHAttribute.colorSet4:
                case MESHAttribute.colorSet5:
                case MESHAttribute.colorSet6:
                case MESHAttribute.colorSet7:
                default:
                    return -1;
            }
        }

        public static int SingleToInt32Bits(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        public static int FromFloat(float fval)
        {
            int fbits = SingleToInt32Bits(fval);
            int sign = fbits >> 16 & 0x8000;          // sign only
            int val = (fbits & 0x7fffffff) + 0x1000; // rounded value

            if (val >= 0x47800000)               // might be or become NaN/Inf
            {                                     // avoid Inf due to rounding
                if ((fbits & 0x7fffffff) >= 0x47800000)
                {                                 // is or must become NaN/Inf
                    if (val < 0x7f800000)        // was value but too large
                        return sign | 0x7c00;     // make it +/-Inf
                    return sign | 0x7c00 |        // remains +/-Inf or NaN
                        (fbits & 0x007fffff) >> 13; // keep NaN (and Inf) bits
                }
                return sign | 0x7bff;             // unrounded not quite Inf
            }
            if (val >= 0x38800000)               // remains normalized value
                return sign | val - 0x38000000 >> 13; // exp - 127 + 15
            if (val < 0x33000000)                // too small for subnormal
                return sign;                      // becomes +/-0
            val = (fbits & 0x7fffffff) >> 23;  // tmp exp for subnormal calc
            return sign | ((fbits & 0x7fffff | 0x800000) // add subnormal bit
                + (0x800000 >> val - 102)     // round depending on cut off
                >> 126 - val);   // div by 2^(1-(exp-127+15)) and >> 13 | exp=0
        }

    }
}
