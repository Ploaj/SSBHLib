using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.ShapeMeshes;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public class RTexture : IRenderable
    {
        private static ScreenTriangle triangle = null;

        // TODO: RTexture properties should not be mutable.
        public Texture SfTexture { get; set; } = null;

        public byte[] BitmapImageData { get; set; }

        public bool IsSrgb { get; set; } = false;

        public void Render(Camera camera)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            // Texture unit 0 should be reserved for image preview.
            var shader = ShaderContainer.GetShader("RTexture");
            shader.UseProgram();
            if (SfTexture != null)
                shader.SetTexture("image", SfTexture, 0);

            // The colors need to be converted back to sRGB gamma.
            shader.SetBoolToInt("isSrgb", IsSrgb);

            triangle.Draw(shader);
        }
    }
}
