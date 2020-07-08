using OpenTK.Graphics.OpenGL;
using SFGenericModel;

namespace CrossMod.Rendering.Models
{
    public class UltimateMesh : GenericMeshNonInterleaved
    {
        public UltimateMesh(uint[] indices, int vertexCount) : base(indices, PrimitiveType.Triangles, vertexCount)
        {

        }
    }
}
