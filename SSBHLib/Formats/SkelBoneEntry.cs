namespace SSBHLib.Formats
{
    public class SkelBoneEntry : SsbhFile
    {
        public string Name { get; set; }

        public short Id { get; set; }

        public short ParentId { get; set; }

        public int Type { get; set; }
    }
}
