namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic vector 4
    /// </summary>
    public struct AnimTrackCustomVector4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public AnimTrackCustomVector4(float x, float y, float z, float w)
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
