using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SFGenericModel.VertexAttributes;

namespace CrossMod.Rendering.Models
{
    struct CustomVertex
    {
        [VertexFloat("Position0", ValueCount.Four, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float)]
        public Vector3 Position0 { get; }

        [VertexFloat("Normal0", ValueCount.Four, OpenTK.Graphics.OpenGL.VertexAttribPointerType.Float)]
        public Vector3 Normal0 { get; }

        public CustomVertex(Vector3 position0, Vector3 normal0)
        {
            Position0 = position0;
            Normal0 = normal0;
        }
    }
}
