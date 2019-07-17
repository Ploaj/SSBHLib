using System.Collections.Generic;
using CrossMod.Rendering;

namespace CrossMod.IO
{
    public class IOModel
    {
        public string Name;
        public RSkeleton Skeleton;
        public List<IOMesh> Meshes = new List<IOMesh>();
        public List<IOMaterial> Materials = new List<IOMaterial>();

        public bool HasSkeleton { get { return Skeleton != null; } }
        public bool HasMeshes { get { return Meshes != null && Meshes.Count != 0; } }
        public bool HasMaterials { get { return Materials != null && Materials.Count > 0; } }
    }
}
