using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Linq;

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

        // HACK: Some of the attributes are crashing.
        // This should be handled by SFGraphics for better error checking.
        private string[] usedAttributes = new string[] { "Position0", "Normal0", "map1", "Tangent0", "Bone", "Weight", "bake1" };

        public void Bind(Shader shader)
        {
            // Not all attributes are rendered.
            int location = shader.GetAttribLocation(Name);
            if (location == -1)
                return;

            if (!usedAttributes.Contains(Name))
                return;

            if (Integer)
                GL.VertexAttribIPointer(location, Size, IType, Stride, (IntPtr)Offset);
            else
                GL.VertexAttribPointer(location, Size, Type, Normalized, Stride, Offset);
        }
    }
}
