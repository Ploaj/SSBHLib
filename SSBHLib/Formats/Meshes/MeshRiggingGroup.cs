namespace SSBHLib.Formats.Meshes
{
    public class MeshRiggingGroup : ISSBH_File
    {
        public string Name { get; set; }
        
        public long SubMeshIndex { get; set; }
    
        public long Unk_Flags { get; set; }

        public MeshBoneBuffer[] Buffers { get; set; }
    }
}
