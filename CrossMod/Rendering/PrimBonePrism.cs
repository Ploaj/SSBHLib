using System.Collections.Generic;
using OpenTK;
using SFGenericModel;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.VertexAttributes;

namespace CrossMod.Rendering
{
    public class PrimBonePrism : GenericMesh<Vector4>
    {
        static PrimBonePrism()
        {
            vertexAttributes = new List<VertexAttribute>
            {
                new VertexFloatAttribute("point", ValueCount.Four, VertexAttribPointerType.Float, false),
            };
        }

        // A prism that is used to indicated bones
        private static readonly List<Vector4> screenPositions = new List<Vector4>()
        {
            // cube
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, 0f, -1f, 0),

            //point top
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(0f, 1f, 0f, 1),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, 1f, 0f, 1),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, 1f, 0f, 1),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, 1f, 0f, 1),

            //point bottom
            new Vector4(0f, 0f, -1f, 0),
            new Vector4(0f, -1f, 0f, 0),
            new Vector4(0f, 0f, 1f, 0),
            new Vector4(0f, -1f, 0f, 0),
            new Vector4(1f, 0f, 0f, 0),
            new Vector4(0f, -1f, 0f, 0),
            new Vector4(-1f, 0f, 0f, 0),
            new Vector4(0f, -1f, 0f, 0),
        };

        public PrimBonePrism() : base(screenPositions.ToArray(), PrimitiveType.Lines)
        {

        }
    }
}
