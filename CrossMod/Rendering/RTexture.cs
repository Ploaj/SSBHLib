using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public class RTexture : IRenderable
    {
        private static ScreenTriangle triangle = null;

        public Texture renderTexture = null;

        public bool IsSrgb { get; set; } = false;

        public void Render(Camera Camera)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            // Texture unit 0 should be reserved for image preview.
            var shader = ShaderContainer.GetShader("RTexture");
            shader.UseProgram();
            if (renderTexture != null)
                shader.SetTexture("image", renderTexture, 0);

            // The colors need to be converted back to sRGB gamma.
            shader.SetBoolToInt("isSrgb", IsSrgb);

            triangle.Draw(shader);
        }
    }
}
