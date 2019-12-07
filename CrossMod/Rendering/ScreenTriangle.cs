using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel;
using System.Collections.Generic;
using SFGenericModel.VertexAttributes;

namespace CrossMod.Rendering
{
    class ScreenTriangle : GenericMesh<Vector3>
    {
        static ScreenTriangle()
        {
            vertexAttributes = new List<VertexAttribute>
            {
                new VertexFloatAttribute("position", ValueCount.Three, VertexAttribPointerType.Float, false),
            };
        }

        // A triangle that extends past the screen.
        private static readonly List<Vector3> screenTrianglePositions = new List<Vector3>()
        {
            new Vector3(-1f, -1f, 0.0f),
            new Vector3( 3f, -1f, 0.0f),
            new Vector3(-1f,  3f, 0.0f)
        };

        public ScreenTriangle() : base(screenTrianglePositions.ToArray(), PrimitiveType.Triangles)
        {

        }
    }
}