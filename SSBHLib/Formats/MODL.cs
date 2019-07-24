namespace SSBHLib.Formats 
{
    [SsbhFile("LDOM")]
    public class Modl : SsbhFile
    {
        public uint Magic { get; set; } = 0x4D4F444C;//= new char[] { 'L', 'D', 'O', 'M' };

        public ushort MajorVersion { get; set; } = 1;

        public ushort MinorVersion { get; set; } = 7;
        
        public string ModelFileName { get; set; }
        
        public string SkeletonFileName { get; set; }

        public ModlMaterialName[] MaterialFileNames { get; set; }

        public string UnknownFileName { get; set; }

        public string MeshString { get; set; }

        public ModlEntry[] ModelEntries { get; set; }
    }

    public class ModlMaterialName : SsbhFile
    {
        public string MaterialFileName { get; set; }
    }

    public class ModlEntry : SsbhFile
    {
        public string MeshName { get; set; }
        
        public long SubIndex { get; set; }
        
        public string MaterialName { get; set; }
    }
}
