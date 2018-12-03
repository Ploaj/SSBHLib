using System;
using System.Collections.Generic;
using CrossMod.Rendering;
using OpenTK;

namespace CrossMod.IO
{
    public class IOModel
    {
        public string Name;
        public RSkeleton Skeleton;

        public bool HasSkeleton { get { return Skeleton != null; } }

        public List<IOMesh> Meshes = new List<IOMesh>();
    }
}
