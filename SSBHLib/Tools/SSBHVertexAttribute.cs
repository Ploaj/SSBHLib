namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic vector4 attribute
    /// Not all values will be used for every type
    /// </summary>
    public struct SsbhVertexAttribute
    {
        public float X, Y, Z, W;

        public SsbhVertexAttribute(float x, float y, float z, float w = 0)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}
