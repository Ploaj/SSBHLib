namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic vector4 attribute
    /// Not all values will be used for every type
    /// </summary>
    public struct SsbhVertexAttribute
    {
        public static readonly SsbhVertexAttribute Zero = new SsbhVertexAttribute(0.0f, 0.0f, 0.0f, 0.0f);

        public static readonly SsbhVertexAttribute One = new SsbhVertexAttribute(1.0f, 1.0f, 1.0f, 1.0f);

        public float X;

        public float Y;

        public float Z;

        public float W;

        public SsbhVertexAttribute(float x, float y, float z, float w = 0)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override string ToString()
        {
            return $"{X},{Y},{Z},{W}";
        }
    }
}
