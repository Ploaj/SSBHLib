using System;
namespace SSBHLib.Tools
{
    /// <summary>
    /// Unknown texture information stored in animation files
    /// </summary>
    public class AnimTrackTexture
    {
        public float[] Floats { get; }

        public AnimTrackTexture(float[] floats)
        {
            Floats = floats;
        }

        public override string ToString()
        {
            return "{" + string.Join(",", Floats) + "}";
        }
    }
}
