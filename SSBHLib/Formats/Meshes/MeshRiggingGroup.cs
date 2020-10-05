namespace SSBHLib.Formats.Meshes
{
    public class MeshRiggingGroup : SsbhFile
    {
        /// <summary>
        /// The <see cref="MeshObject.Name"/> assigned to this group.
        /// </summary>
        public string MeshName { get; set; }

        /// <summary>
        /// The <see cref="MeshObject.SubIndex"/>.
        /// </summary>
        public long MeshSubIndex { get; set; }

        /// <summary>
        /// 0x0100 | (max number of weights the vertices use i.e. 0 if single bound 4 if multibound)
        /// </summary>
        public long Flags { get; set; }

        public MeshBoneBuffer[] Buffers { get; set; }
    }
}
