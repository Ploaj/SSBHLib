using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RMesh : IRenderable
    {
        public static readonly int colMapIndex = 1;

        public string Name;

        public List<CustomVertexAttribute> VertexAttributes = new List<CustomVertexAttribute>();

        public int IndexOffset;
        public int IndexCount;

        public string SingleBindName = "";
        public int SingleBindIndex = -1;

        public Material Material;

        public void Draw(Shader s, Camera c)
        {
            s.SetInt("SingleBindIndex", SingleBindIndex);
            if (Material != null)
            {
                if (Material.COL != null)
                    s.SetTexture("_col", Material.COL, colMapIndex);
            }
            foreach (CustomVertexAttribute a in VertexAttributes)
            {
                a.Bind(s);
            }

            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedShort, IndexOffset);
        }

        public void Render(Camera Camera)
        {

        }
    }
}
