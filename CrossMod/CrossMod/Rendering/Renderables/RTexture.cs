using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.ShapeMeshes;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Samplers;
using SFGraphics.GLObjects.Textures;
using OpenTK.Graphics.OpenGL;
using SFShapes;
using OpenTK;
using System.Linq;

namespace CrossMod.Rendering
{
    public class RTexture : IRenderable
    {
        private static ScreenTriangle? triangle = null;
        private static Mesh3D? skyBox = null;
        private static SamplerObject? sampler = null;

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
            if (Texture == null)
                return;
            // Don't use the same shader because the "image" uniform can only have a single type.
            // Cube maps will require a separate shader.
            if (Texture is Texture2D)
                DrawTexture2d();
            else if (Texture is TextureCubeMap)
                DrawTextureCube(camera);
        }

        private void DrawTexture2d()
        {
            var shader = ShaderContainer.GetShader("RTexture");
            shader.UseProgram();

            if (triangle == null)
                triangle = new ScreenTriangle();

            // Use nearest neighbor to preserve pixel boundaries.
            if (sampler == null)
                sampler = new SamplerObject { MagFilter = TextureMagFilter.Nearest, MinFilter = TextureMinFilter.Nearest };

            // Texture unit 0 should be reserved for image preview.
            sampler.Bind(0);
            shader.SetTexture("image", Texture, 0);

            // The colors need to be converted back to sRGB gamma.
            shader.SetBoolToInt("isSrgb", IsSrgb);

            triangle.Draw(shader);
        }

        private void DrawTextureCube(Camera camera)
        {
            var shader = ShaderContainer.GetShader("RTextureCube");
            shader.UseProgram();

            if (skyBox == null)
            {
                var shape = ShapeGenerator.GetCubePositions(Vector3.Zero, 2.0f);
                skyBox = new Mesh3D(shape.Item1.Select(v => new Vertex3d(v.X, v.Y, v.Z)).ToArray(), shape.Item2);
            }

            // Use nearest neighbor to preserve pixel boundaries.
            if (sampler == null)
                sampler = new SamplerObject { MagFilter = TextureMagFilter.Nearest, MinFilter = TextureMinFilter.Nearest };

            // Texture unit 0 should be reserved for image preview.
            sampler.Bind(0);
            shader.SetTexture("image", Texture, 0);

            // Use the existing rotation matrix to support changing the view.
            shader.SetMatrix4x4("mvpMatrix", camera.RotationMatrix * Matrix4.CreatePerspectiveFieldOfView(1.5708f, camera.RenderWidth / (float)camera.RenderHeight, 0.01f, 10.0f));

            // The colors need to be converted back to sRGB gamma.
            shader.SetBoolToInt("isSrgb", IsSrgb);

            // Disable culling since only the backside will be visible.
            GL.Disable(EnableCap.CullFace);
            skyBox.Draw(shader);
        }
    }
}
