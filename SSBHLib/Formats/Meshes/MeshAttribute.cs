namespace SSBHLib.Formats.Meshes
{
    public class MeshAttribute : SsbhFile
    {
        public int Index { get; set; }
        
        public int DataType { get; set; }
        
        public int BufferIndex { get; set; }
        
        public int BufferOffset { get; set; }
        
        public int Unk4 { get; set; } // usually 0 padding?
        
        public int Unk5 { get; set; } // usually 0 padding?

        public string Name { get; set; }
        
        public MeshAttributeString[] AttributeStrings { get; set; }
    }
}
