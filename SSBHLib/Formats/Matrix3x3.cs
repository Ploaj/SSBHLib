namespace SSBHLib.Formats
{
    public struct Matrix3x3
    {
        public Vector3 Row1 { get; }
        public Vector3 Row2 { get; }
        public Vector3 Row3 { get; }

        public Matrix3x3(Vector3 row1, Vector3 row2, Vector3 row3)
        {
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }
    }
}
