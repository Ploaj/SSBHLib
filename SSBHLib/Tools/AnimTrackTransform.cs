namespace SSBHLib.Tools
{
    /// <summary>
    /// Represents a generic transforms with translation, rotation (as quaternion), and scale.
    /// </summary>
    public struct AnimTrackTransform
    {
        public float X;
        public float Y;
        public float Z;

        public float RX;
        public float RY;
        public float RZ;
        public float RW;

        public float SX;
        public float SY;
        public float SZ;

        public float CompensateScale;

        public override string ToString()
        {
            return $"(Position: ({X}, {Y}, {Z}), Rotation: ({RX}, {RY}, {RZ}, {RW}), Scale: ({SX}, {SY}, {SZ}), CompensateScale: {CompensateScale})";
        }
    }
}
