using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;

namespace CrossMod.Rendering
{
    public class CustomVertexAttribute
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public VertexAttribPointerType Type { get; set; }
        public bool Normalized { get; set; } = false;
        public int Stride { get; set; }
        public int Offset { get; set; }

        public void Bind(Shader shader)
        {
            GL.VertexAttribPointer(shader.GetAttribLocation(Name), Size, Type, Normalized, Stride, Offset);
        }
    }
}
