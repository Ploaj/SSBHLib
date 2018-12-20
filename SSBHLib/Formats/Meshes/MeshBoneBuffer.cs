namespace SSBHLib.Formats.Meshes
{
    public class MeshBoneBuffer : ISSBH_File
    {
        public string BoneName { get; set; }

        public byte[] Data { get; set; }
    }
}
