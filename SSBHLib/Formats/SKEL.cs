namespace SSBHLib.Formats
{
    [SSBHFileAttribute("LEKS")]
    public class SKEL : ISSBH_File
    {
        public uint Magic { get; set; }  = 0x534B454C;

        public ushort MajorVersion { get; set; } = 1;

        public ushort MinorVersion { get; set; } = 0;

        public SKEL_BoneEntry[] BoneEntries { get; set; }

        public SKEL_Matrix[] WorldTransform { get; set; }

        public SKEL_Matrix[] InvWorldTransform { get; set; }

        public SKEL_Matrix[] Transform { get; set; }

        public SKEL_Matrix[] InvTransform { get; set; }
    }
}
