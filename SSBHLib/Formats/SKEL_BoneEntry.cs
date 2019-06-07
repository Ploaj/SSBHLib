namespace SSBHLib.Formats
{
    public class SKEL_BoneEntry : ISSBH_File
    {
        public string Name { get; set; }

        public short ID { get; set; }

        public short ParentID { get; set; }

        public int Type { get; set; }
    }
}
