using System;
using System.Collections.Generic;
using CrossMod.Rendering;

namespace CrossMod.IO
{
    public class IOModel
    {
        public string Name;
        public RSkeleton Skeleton;
        public List<IOMesh> Meshes = new List<IOMesh>();

        public bool HasSkeleton { get { return Skeleton != null; } }
        public bool HasMeshes { get { return Meshes != null; } }
    }
}
