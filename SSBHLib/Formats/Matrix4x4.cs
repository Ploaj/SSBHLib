namespace SSBHLib.Formats
{
    public struct Matrix4x4
    {
        public Vector4 Row1 { get; }
        public Vector4 Row2 { get; }
        public Vector4 Row3 { get; }
        public Vector4 Row4 { get; }

        public Matrix4x4(Vector4 row1, Vector4 row2, Vector4 row3, Vector4 row4)
        {
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
            Row4 = row4;
        }
    }
}
