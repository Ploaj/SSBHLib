namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic vector4 attribute
    /// Not all values will be used for every type
    /// </summary>
    public struct SSBHVertexAttribute
    {
        public float X, Y, Z, W;

        public SSBHVertexAttribute(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            W = 0;
        }

        public SSBHVertexAttribute(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}
