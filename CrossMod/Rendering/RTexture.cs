using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.IO;

namespace CrossMod.Rendering
{
    public class RTexture : IRenderable
    {
        private static Shader textureShader = null;
        private static ScreenTriangle triangle = null;

        public SFGraphics.GLObjects.Textures.Texture texture = null;

        public void Render(Camera Camera)
        {
            // TODO: Render texture.
            if (triangle == null)
                triangle = new ScreenTriangle();

            if (textureShader == null)
            {
                textureShader = new Shader();
                textureShader.LoadShader(File.ReadAllText("Shaders/texture.frag"), OpenTK.Graphics.OpenGL.ShaderType.FragmentShader);
                textureShader.LoadShader(File.ReadAllText("Shaders/texture.vert"), OpenTK.Graphics.OpenGL.ShaderType.VertexShader);
            }

            textureShader.UseProgram();
            if (texture != null)
                textureShader.SetTexture("image", texture, 0);

            triangle.Draw(textureShader, null);
        }
    }
}
