namespace SSBHLib.Formats 
{
    [SSBHFileAttribute("LDOM")]
    public class MODL : ISSBH_File
    {
        public uint Magic { get; set; } = 0x4D4F444C;//= new char[] { 'L', 'D', 'O', 'M' };

        public ushort MajorVersion { get; set; } = 1;

        public ushort MinorVersion { get; set; } = 7;
        
        public string ModelFileName { get; set; }
        
        public string SkeletonFileName { get; set; }

        public MODL_MaterialName[] MaterialFileNames { get; set; }

        public string UnknownFileName { get; set; } = "";

        public string MeshString { get; set; }

        public MODL_Entry[] ModelEntries { get; set; }
    }

    public class MODL_MaterialName : ISSBH_File
    {
        public string MaterialFileName { get; set; }
    }

    public class MODL_Entry : ISSBH_File
    {
        public string MeshName { get; set; }
        
        public long SubIndex { get; set; }
        
        public string MaterialName { get; set; }
    }
}
