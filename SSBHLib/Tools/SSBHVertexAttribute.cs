namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic vector4 attribute
    /// Not all values will be used for every type
    /// </summary>
    public struct SSBHVertexAttribute
    {
        public float X, Y, Z, W;

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}
