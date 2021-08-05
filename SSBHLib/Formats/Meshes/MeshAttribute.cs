namespace SSBHLib.Formats.Meshes
{
    public class MeshAttribute : SsbhFile
    {
        public enum AttributeDataType : uint
        {
            Float3 = 0,
            Byte4 = 2,
            Float4 = 4,
            HalfFloat4 = 5,
            Float2 = 7,
            HalfFloat2 = 8,
        }

        public int Usage { get; set; }

        public AttributeDataType DataType { get; set; }

        public int BufferIndex { get; set; }

        public int BufferOffset { get; set; }

        public ulong SubIndex { get; set; } // usually 0 padding?

        /// <summary>
        /// The name of the attribute, which may not be the same as the name in <see cref="AttributeStrings"/>.
        /// </summary>
        public string Name { get; set; }

        public SsbhString[] AttributeStrings { get; set; }
    }
}
