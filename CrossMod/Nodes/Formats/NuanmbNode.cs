using CrossMod.Nodes.Conversion;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats.Animation;

namespace CrossMod.Nodes
{
    public class NuanmbNode : FileNode
    {
        private readonly Anim animation;

        public NuanmbNode(string path) : base(path, "animation", true)
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out animation);
        }

        public IRenderableAnimation GetRenderableAnimation() => AnimToRenderable.CreateRenderableAnimation(animation);
    }
}
