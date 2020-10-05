namespace SSBHLib.Formats
{
    [SsbhFile("LEKS")]
    public class Skel : SsbhFile
    {
        public uint Magic { get; set; } = 0x534B454C;

        public ushort MajorVersion { get; set; } = 1;

        public ushort MinorVersion { get; set; } = 0;

        public SkelBoneEntry[] BoneEntries { get; set; }

        public Matrix4x4[] WorldTransform { get; set; }

        public Matrix4x4[] InvWorldTransform { get; set; }

        public Matrix4x4[] Transform { get; set; }

        public Matrix4x4[] InvTransform { get; set; }
    }
}
