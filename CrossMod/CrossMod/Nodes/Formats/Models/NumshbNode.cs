using CrossMod.Nodes.Conversion;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using SSBHLib;
using SSBHLib.Formats.Meshes;
using System.IO;

namespace CrossMod.Nodes
{
    public class NumshbNode : FileNode
    {
        public Mesh mesh;
        public Adjb ExtendedMesh;

        public NumshbNode(string path) : base(path, "mesh", false)
        {
            Open();
        }

        public RModel GetRenderModel(RSkeleton? skeleton) => MeshToRenderable.GetRenderModel(mesh, skeleton);

        private void Open()
        {
            string adjb = Path.GetDirectoryName(AbsolutePath) + "/model.adjb";
            if (File.Exists(adjb))
            {
                ExtendedMesh = new Adjb();
                ExtendedMesh.Read(adjb);
            }

            Ssbh.TryParseSsbhFile(AbsolutePath, out mesh);
        }
    }
}
