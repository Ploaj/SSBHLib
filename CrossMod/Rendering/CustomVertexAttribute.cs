using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;
using System;

namespace CrossMod.Rendering
{
    public class CustomVertexAttribute
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public VertexAttribPointerType Type { get; set; }
        public VertexAttribIntegerType IType { get; set; }
        public bool Normalized { get; set; } = false;
        public int Stride { get; set; }
        public int Offset { get; set; }
        public bool Integer { get; set; } = false;

        public void Bind(Shader shader)
        {
            if (Integer)
                GL.VertexAttribIPointer(shader.GetAttribLocation(Name), Size, IType, Stride, (IntPtr)Offset);
            else
                GL.VertexAttribPointer(shader.GetAttribLocation(Name), Size, Type, Normalized, Stride, Offset);
        }
    }
}
