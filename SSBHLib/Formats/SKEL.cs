namespace SSBHLib.Formats
{
    [SsbhFile("LEKS")]
    public class Skel : SsbhFile
    {
        public uint Magic { get; set; }  = 0x534B454C;

        public ushort MajorVersion { get; set; } = 1;

        public ushort MinorVersion { get; set; } = 0;

        public SkelBoneEntry[] BoneEntries { get; set; }

        public SkelMatrix[] WorldTransform { get; set; }

        public SkelMatrix[] InvWorldTransform { get; set; }

        public SkelMatrix[] Transform { get; set; }

        public SkelMatrix[] InvTransform { get; set; }
    }
}
