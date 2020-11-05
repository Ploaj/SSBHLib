namespace CrossMod.Tools
{
    public static class SsbhVectorToOpenTkExtensions
    {
        public static OpenTK.Vector3 ToOpenTK(this SSBHLib.Formats.Vector3 vector3)
        {
            return new OpenTK.Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static OpenTK.Matrix3 ToOpenTK(this SSBHLib.Formats.Matrix3x3 matrix)
        {
            return new OpenTK.Matrix3(matrix.Row1.ToOpenTK(), matrix.Row2.ToOpenTK(), matrix.Row3.ToOpenTK());
        }
    }
}
