using System;
namespace SSBHLib.Tools
{
    /// <summary>
    /// Unknown texture information stored in animation files
    /// </summary>
    public class AnimTrackTexture
    {
        public float[] Floats;

        public AnimTrackTexture(float[] floats)
        {
            this.Floats = floats;
        }

        public override string ToString()
        {
            return "{" + String.Join(",", Floats) + "}";
        }
    }
}
