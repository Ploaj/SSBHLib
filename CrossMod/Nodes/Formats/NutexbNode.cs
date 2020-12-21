using CrossMod.Rendering;
using CrossMod.Rendering.Resources;
using System;
using System.IO;

namespace CrossMod.Nodes
{
    public class NutexbNode : FileNode, IRenderableNode
    {
        public string TexName { get; }

        public Lazy<IRenderable> Renderable { get; }

        public NutexbNode(string path) : base(path, "texture", true)
        {
            var surface = SBSurface.FromNutexb(AbsolutePath);
            TexName = surface?.Name ?? Path.GetFileNameWithoutExtension(AbsolutePath);

            if (surface == null)
                Renderable = new Lazy<IRenderable>(new RTexture(DefaultTextures.Instance.Value.DefaultBlack, false));
            else
                Renderable = new Lazy<IRenderable>(new RTexture(surface.GetRenderTexture(), surface.IsSRGB));
        }

        public override string ToString()
        {
            return Text.Contains(".") ? Text.Substring(0, Text.IndexOf(".")) : Text;
        }
    }
}
