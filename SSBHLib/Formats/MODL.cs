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

        public long Unk1 { get; set; } // always 0?

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

        /// <summary>
        /// Assigns the material with matching <see cref="Materials.MatlEntry.MaterialLabel"/> to this entry.
        /// </summary>
        public string MaterialLabel { get; set; }
    }
}
