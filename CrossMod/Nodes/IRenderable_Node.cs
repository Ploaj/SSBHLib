using CrossMod.Rendering;

namespace CrossMod.Nodes
{
    public abstract class IRenderableNode : IFileNode
    {
        public abstract IRenderable GetRenderableNode();
    }
}
