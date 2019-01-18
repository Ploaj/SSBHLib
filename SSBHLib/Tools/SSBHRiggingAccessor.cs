using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSBHLib.Tools
{
    /// <summary>
    /// A tool for extracting rigging information out of a MESH file
    /// </summary>
    public class SSBHRiggingAccessor
    {
        private readonly MESH meshFile;

        /// <summary>
        /// Creates a rigging accessor from a filepath
        /// </summary>
        /// <param name="MESHFilePath"></param>
        public SSBHRiggingAccessor(string MESHFilePath)
        {
            ISSBH_File File;
            if (SSBH.TryParseSSBHFile(MESHFilePath, out File))
            {
                if (File == null)
                    throw new FileNotFoundException("File was null");

                if (File is MESH mesh)
                    meshFile = mesh;
                else
                    throw new FormatException("Given file was not a MESH file");
            }
        }

        /// <summary>
        /// Creates a rigging accessor from a mesh file
        /// </summary>
        /// <param name="meshFile"></param>
        public SSBHRiggingAccessor(MESH meshFile)
        {
            this.meshFile = meshFile;
        }

        /// <summary>
        /// Reads all the rigging buffers out of the mesh file
        /// </summary>
        /// <returns>array of tuples containing mesh name, index, and influence array</returns>
        public Tuple<string, int, SSBHVertexInfluence[]>[] ReadRiggingBuffers()
        {
            List<Tuple<string, int, SSBHVertexInfluence[]>> o = new List<Tuple<string, int, SSBHVertexInfluence[]>>(meshFile.Objects.Length);
            foreach (var meshObject in meshFile.Objects)
            {
                o.Add(new Tuple<string, int, SSBHVertexInfluence[]>(meshObject.Name, (int)meshObject.SubMeshIndex, ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubMeshIndex)));
            }
            return o.ToArray();
        }

        /// <summary>
        /// Reads a vertex influence array for given mesh name and subindex
        /// returns null if not found
        /// </summary>
        /// <param name="meshName"></param>
        /// <param name="subIndex"></param>
        /// <returns></returns>
        public SSBHVertexInfluence[] ReadRiggingBuffer(string meshName, int subIndex)
        {
            MeshRiggingGroup riggingGroup = FindRiggingGroup(meshName, subIndex);

            if (riggingGroup == null)
                return new SSBHVertexInfluence[0];

            List<SSBHVertexInfluence> Influences = new List<SSBHVertexInfluence>();

            foreach (MeshBoneBuffer boneBuffer in riggingGroup.Buffers)
            {
                using (BinaryReader R = new BinaryReader(new MemoryStream(boneBuffer.Data)))
                {
                    for (int i = 0; i < boneBuffer.Data.Length / 6; i++)
                    {
                        Influences.Add(new SSBHVertexInfluence()
                        {
                            BoneName = boneBuffer.BoneName,
                            VertexIndex = R.ReadUInt16(),
                            Weight = R.ReadSingle()
                        });
                    }
                }
            }

            return Influences.ToArray();
        }
        
        /// <summary>
        /// Finds a rigging group for given mesh name and sub index
        /// </summary>
        /// <param name="meshName"></param>
        /// <param name="subIndex"></param>
        /// <returns>null if not found</returns>
        private MeshRiggingGroup FindRiggingGroup(string meshName, int subIndex)
        {
            MeshRiggingGroup riggingGroup = null;

            foreach (MeshRiggingGroup g in meshFile.RiggingBuffers)
            {
                if (g.Name.Equals(meshName) && g.SubMeshIndex == subIndex)
                {
                    riggingGroup = g;
                    break;
                }
            }

            return riggingGroup;
        }
    }
}
