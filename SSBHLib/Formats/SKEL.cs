namespace SSBHLib.Formats
{
    [SSBHFileAttribute("LEKS")]
    public class SKEL : ISSBH_File
    {
        public uint Magic { get; set; } //= new char[] { 'L', 'D', 'O', 'M' };

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }

        public SKEL_BoneEntry[] BoneEntries { get; set; }

        public SKEL_Matrix[] WorldTransform { get; set; }

        public SKEL_Matrix[] InvWorldTransform { get; set; }

        public SKEL_Matrix[] Transform { get; set; }

        public SKEL_Matrix[] InvTransform { get; set; }
    }

    public class SKEL_BoneEntry : ISSBH_File
    {
        public string Name { get; set; }

        public short ID { get; set; }

        public short ParentID { get; set; }

        public int Type { get; set; }
    }

    public class SKEL_Matrix : ISSBH_File
    {
        public float M11 { get; set; }
        public float M12 { get; set; }
        public float M13 { get; set; }
        public float M14 { get; set; }
        public float M21 { get; set; }
        public float M22 { get; set; }
        public float M23 { get; set; }
        public float M24 { get; set; }
        public float M31 { get; set; }
        public float M32 { get; set; }
        public float M33 { get; set; }
        public float M34 { get; set; }
        public float M41 { get; set; }
        public float M42 { get; set; }
        public float M43 { get; set; }
        public float M44 { get; set; }
    }
}
