namespace SSBHLib.Formats.Meshes
{
    public class MeshRiggingGroup : ISSBH_File
    {
        public string Name { get; set; }
        
        public long SubMeshIndex { get; set; }
    
        public long Flags { get; set; } // 0x0100 | (max number of weights the vertices use i.e. 0 if single bound 4 if multibound)

        public MeshBoneBuffer[] Buffers { get; set; }
    }
}
