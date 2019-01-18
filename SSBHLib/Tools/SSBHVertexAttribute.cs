namespace SSBHLib.Tools
{
    public struct SSBHVertexAttribute
    {
        public float X, Y, Z, W;

        public override string ToString()
        {
            return $"({X}, {Y}, {Z}, {W})";
        }
    }
}
