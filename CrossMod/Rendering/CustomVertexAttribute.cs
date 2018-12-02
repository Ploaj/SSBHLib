using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;

namespace CrossMod.Rendering
{
    public class CustomVertexAttribute
    {
        public string Name;
        public int Size;
        public VertexAttribPointerType Type;
        public bool Normalized = false;
        public int Stride;
        public int Offset;

        public void Bind(Shader shader)
        {
            GL.VertexAttribPointer(shader.GetAttribLocation(Name), Size, Type, Normalized, Stride, Offset);
        }
    }
}
