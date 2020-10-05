namespace SSBHLib.Tools
{
    /// <summary>
    /// Unknown texture information stored in animation files
    /// </summary>
    public class AnimTrackTexture
    {
        public float UnkFloat1 = 1;
        public float UnkFloat2 = 1;
        public float UnkFloat3 = 0;
        public float UnkFloat4 = 0;
        public int Unknown = 0;

        public override string ToString()
        {
            return $"[ {UnkFloat1}, {UnkFloat2}, {UnkFloat3}, {UnkFloat4}, {Unknown} ]";
        }
    }
}
