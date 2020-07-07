using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using CrossMod.Rendering.ShapeMeshes;
using CrossMod.Rendering.GlTools;

namespace CrossMod.Rendering
{
    public class RTexture : IRenderable
    {
        private static ScreenTriangle triangle = null;

        // TODO: These propeties should not be mutable.
        public Texture RenderTexture { get; set; } = null;

        public bool IsSrgb { get; set; } = false;

        public void Render(Camera camera)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            // Texture unit 0 should be reserved for image preview.
            var shader = ShaderContainer.GetShader("RTexture");
            shader.UseProgram();
            if (RenderTexture != null)
                shader.SetTexture("image", RenderTexture, 0);

            // The colors need to be converted back to sRGB gamma.
            shader.SetBoolToInt("isSrgb", IsSrgb);

            triangle.Draw(shader);
        }
    }
}
