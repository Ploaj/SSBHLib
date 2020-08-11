using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.IO;

namespace SSBHLib.Tools
{
    /// <summary>
    /// A tool for extracting rigging information out of a MESH file
    /// </summary>
    public class SsbhRiggingAccessor
    {
        private readonly Mesh meshFile;

        /// <summary>
        /// Creates a rigging accessor from a filepath
        /// </summary>
        /// <param name="meshFilePath"></param>
        public SsbhRiggingAccessor(string meshFilePath)
        {
            if (!Ssbh.TryParseSsbhFile(meshFilePath, out meshFile))
                throw new FormatException("Given file was not a MESH file");
        }

        /// <summary>
        /// Creates a rigging accessor from a mesh file
        /// </summary>
        /// <param name="meshFile"></param>
        public SsbhRiggingAccessor(Mesh meshFile)
        {
            this.meshFile = meshFile;
        }

        /// <summary>
        /// Reads all the rigging buffers out of the mesh file
        /// </summary>
        /// <returns>array of tuples containing mesh name, index, and influence array</returns>
        public Tuple<string, int, SsbhVertexInfluence[]>[] ReadRiggingBuffers()
        {
            List<Tuple<string, int, SsbhVertexInfluence[]>> o = new List<Tuple<string, int, SsbhVertexInfluence[]>>(meshFile.Objects.Length);
            foreach (var meshObject in meshFile.Objects)
            {
                o.Add(new Tuple<string, int, SsbhVertexInfluence[]>(meshObject.Name, (int)meshObject.SubIndex, ReadRiggingBuffer(meshObject.Name, (int)meshObject.SubIndex)));
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
        public SsbhVertexInfluence[] ReadRiggingBuffer(string meshName, int subIndex)
        {
            MeshRiggingGroup riggingGroup = FindRiggingGroup(meshName, subIndex);

            if (riggingGroup == null)
                return new SsbhVertexInfluence[0];

            List<SsbhVertexInfluence> influences = new List<SsbhVertexInfluence>();

            foreach (MeshBoneBuffer boneBuffer in riggingGroup.Buffers)
            {
                using (BinaryReader r = new BinaryReader(new MemoryStream(boneBuffer.Data)))
                {
                    for (int i = 0; i < boneBuffer.Data.Length / 6; i++)
                    {
                        influences.Add(new SsbhVertexInfluence()
                        {
                            BoneName = boneBuffer.BoneName,
                            // TODO: Read an array of influence structs and store the string separately?
                            VertexIndex = r.ReadUInt16(),
                            Weight = r.ReadSingle()
                        });
                    }
                }
            }

            return influences.ToArray();
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
                if (g.MeshName.Equals(meshName) && g.MeshSubIndex == subIndex)
                {
                    riggingGroup = g;
                    break;
                }
            }

            return riggingGroup;
        }
    }
}
