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

        public float Rx;
        public float Ry;
        public float Rz;
        public float Rw;

        public float Sx;
        public float Sy;
        public float Sz;

        public float CompensateScale;

        public override string ToString()
        {
            return $"(Position: ({X}, {Y}, {Z}), Rotation: ({Rx}, {Ry}, {Rz}, {Rw}), Scale: ({Sx}, {Sy}, {Sz}), CompensateScale: {CompensateScale})";
        }
    }
}
