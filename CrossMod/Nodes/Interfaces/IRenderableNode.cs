using CrossMod.Rendering;
using System;

namespace CrossMod.Nodes
{
    public interface IRenderableNode
    {
        Lazy<IRenderable> Renderable { get; }
    }
}
