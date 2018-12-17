using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RenderMesh : GenericMesh<CustomVertex>
    {
        public RenderMesh(List<CustomVertex> vertices, List<uint> indices) : base(vertices, indices, PrimitiveType.Triangles, DrawElementsType.UnsignedInt)
        {

        }
    }
}
