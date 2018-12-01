namespace SSBHLib.Formats 
{
    [SSBHFileAttribute("LDOM")]
    public class MODL : ISSBH_File
    {
        public uint Magic { get; set; } //= new char[] { 'L', 'D', 'O', 'M' };

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }
        
        public string ModelFileName { get; set; }
        
        public string SkeletonFileName { get; set; }

        public MODL_MaterialName[] MaterialFileNames { get; set; }

        public string UnknownFileName { get; set; }

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
