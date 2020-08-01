using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.ShapeMeshes;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public static class ScreenDrawing
    {
        private static ScreenTriangle triangle = null;

        public static void Render(Texture texture)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            var shader = ShaderContainer.GetShader("RTexture");
            shader.UseProgram();
            if (texture != null)
                shader.SetTexture("image", texture, 0);

            // The colors need to be converted back to sRGB gamma.
            shader.SetBoolToInt("isSrgb", false);

            triangle.Draw(shader);
        }
    }
}
