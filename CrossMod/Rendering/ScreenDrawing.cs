using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.ShapeMeshes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Samplers;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public static class ScreenDrawing
    {
        private static ScreenTriangle triangle = null;

        private static SamplerObject sampler = null;

        public static void DrawTexture(Texture texture)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();
            if (sampler == null)
                sampler = new SamplerObject { MagFilter = TextureMagFilter.Nearest, MinFilter = TextureMinFilter.Nearest };

            var shader = ShaderContainer.GetShader("ScreenTexture");
            shader.UseProgram();

            sampler.Bind(0);
            shader.SetTexture("image", texture, 0);

            triangle.Draw(shader);
        }

        public static void DrawBloomCombined(Texture colorTex, Texture colorBrightTex)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            var shader = ShaderContainer.GetShader("ScreenBloomCombined");
            shader.UseProgram();

            // TODO: Samplers are bound from the previous model drawing.
            GL.BindSampler(0, 0);
            GL.BindSampler(1, 0);

            shader.SetTexture("colorTex", colorTex, 0);
            shader.SetTexture("colorBrightTex", colorBrightTex, 1);

            shader.SetBoolToInt("enableBloom", RenderSettings.Instance.EnableBloom);
            shader.SetFloat("bloomIntensity", RenderSettings.Instance.BloomIntensity);

            triangle.Draw(shader);
        }

        public static void DrawGradient(Vector3 colorBottom, Vector3 colorTop)
        {
            if (triangle == null)
                triangle = new ScreenTriangle();

            var shader = ShaderContainer.GetShader("ScreenGradient");
            shader.UseProgram();

            shader.SetVector3("colorTop", colorTop);
            shader.SetVector3("colorBottom", colorBottom);

            triangle.Draw(shader);
        }
    }
}
