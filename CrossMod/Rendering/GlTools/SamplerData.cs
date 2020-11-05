using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Samplers;

namespace CrossMod.Rendering.GlTools
{
    public class SamplerData
    {
        public TextureWrapMode WrapS { get; set; }
        public TextureWrapMode WrapT { get; set; }
        public TextureWrapMode WrapR { get; set; }
        public TextureMinFilter MinFilter { get; set; }
        public TextureMagFilter MagFilter { get; set; }
        public float LodBias { get; set; }
        public int MaxAnisotropy { get; set; }

        public SamplerObject ToSampler()
        {
            return new SamplerObject
            {
                TextureWrapS = WrapS,
                TextureWrapT = WrapT,
                TextureWrapR = WrapR,
                MinFilter = MinFilter,
                MagFilter = MagFilter,
                TextureLodBias = LodBias,
                TextureMaxAnisotropy = MaxAnisotropy
            };
        }
    }
}
