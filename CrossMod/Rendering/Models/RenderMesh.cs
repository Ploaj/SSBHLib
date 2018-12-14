using SFGenericModel;
using SFGraphics.Cameras;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RenderMesh : GenericMesh<CustomVertex>
    {
        public RenderMesh(List<CustomVertex> vertices, List<int> indices) : base(vertices, indices, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles)
        {

        }
    }
}
