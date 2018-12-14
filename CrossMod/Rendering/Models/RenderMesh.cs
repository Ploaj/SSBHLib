using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGenericModel;
using SFGraphics.Cameras;

namespace CrossMod.Rendering.Models
{
    class RenderMesh : GenericMesh<CustomVertex>, IRenderable
    {
        public RenderMesh(List<CustomVertex> vertices, List<int> indices) : base(vertices, indices, OpenTK.Graphics.OpenGL.PrimitiveType.Triangles)
        {

        }

        public void Render(Camera camera)
        {
            // TODO: Select the proper shader.
            Draw(null, camera);
        }
    }
}
