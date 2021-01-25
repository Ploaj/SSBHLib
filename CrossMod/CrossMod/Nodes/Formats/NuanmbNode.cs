using CrossMod.Nodes.Conversion;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats.Animation;
using System;

namespace CrossMod.Nodes
{
    public class NuanmbNode : FileNode
    {
        private readonly Lazy<Anim> animation;

        public NuanmbNode(string path) : base(path, "animation", true)
        {
            animation = new Lazy<Anim>(() =>
            {
                Ssbh.TryParseSsbhFile(AbsolutePath, out Anim anim);
                return anim;
            });
        }

        public IRenderableAnimation GetRenderableAnimation() => AnimToRenderable.CreateRenderableAnimation(animation.Value);
    }
}
