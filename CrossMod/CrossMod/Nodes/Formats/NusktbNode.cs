using CrossMod.Nodes.Conversion;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats;
using System;

namespace CrossMod.Nodes
{
    public class NusktbNode : FileNode, IRenderableNode
    {
        private readonly Skel skel;

        public NusktbNode(string path) : base(path, "skeleton", false)
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out skel);
            Renderable = new Lazy<IRenderable>(() => SkelToRenderable.CreateRSkeleton(skel));
        }

        public Lazy<IRenderable> Renderable { get; }
    }
}
