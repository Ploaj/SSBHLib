using CrossMod.Rendering;
using CrossMod.Tools;
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
            Renderable = new Lazy<IRenderable>(() => GetRenderableNode());
        }

        public Lazy<IRenderable> Renderable { get; }

        public RSkeleton GetRenderableNode()
        {
            if (skel == null) 
                return null;

            var skeleton = new RSkeleton();

            for (int i = 0; i < skel.BoneEntries.Length; i++)
            {
                RBone bone = new RBone
                {
                    Name = skel.BoneEntries[i].Name,
                    Id = skel.BoneEntries[i].Id,
                    ParentId = skel.BoneEntries[i].ParentId,
                    Transform = skel.Transform[i].ToOpenTK(),
                    InvTransform = skel.InvTransform[i].ToOpenTK(),
                    WorldTransform = skel.WorldTransform[i].ToOpenTK(),
                    InvWorldTransform = skel.InvWorldTransform[i].ToOpenTK()
                };
                skeleton.Bones.Add(bone);
            }

            skeleton.Reset();

            return skeleton;
        }
    }
}
