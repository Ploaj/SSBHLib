using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.ShapeMeshes;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Samplers;
using SFGraphics.GLObjects.Textures;
using OpenTK.Graphics.OpenGL;

namespace CrossMod.Rendering
{
    public class RTexture : IRenderable
    {
        private static ScreenTriangle triangle = null;
        private static SamplerObject sampler = null;

        public Texture Texture { get; }

        /// <summary>
        /// <c>true</c> if the texture uses an _SRGB format, 
        /// which requires undoing the gamma correction applied by OpenGL to display the raw texture data.
        /// </summary>
        public bool IsSrgb { get; }

        public RTexture(Texture sfTexture, bool isSrgb)
        {
            Texture = sfTexture;
            IsSrgb = isSrgb;
        }

        public void Render(Camera camera)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            // Use nearest neighbor to preserve pixel boundaries.
            if (sampler == null)
                sampler = new SamplerObject { MagFilter = TextureMagFilter.Nearest, MinFilter = TextureMinFilter.Nearest };

            var shader = ShaderContainer.GetShader("RTexture");
            shader.UseProgram();

            if (Texture != null)
            {
                // Texture unit 0 should be reserved for image preview.
                sampler.Bind(0);
                shader.SetTexture("image", Texture, 0);

                // The colors need to be converted back to sRGB gamma.
                shader.SetBoolToInt("isSrgb", IsSrgb);

                triangle.Draw(shader);
            }

        }
    }
}
