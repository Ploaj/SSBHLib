using CrossMod.Rendering;
using CrossMod.Tools;
using SSBHLib;
using SSBHLib.Formats;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nusktb")]
    public class SkelNode : FileNode, IRenderableNode
    {
        private Skel skel;

        private RSkeleton skeleton;

        public SkelNode(string path) : base(path)
        {
            ImageKey = "skeleton";
        }

        public override void Open()
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out skel);
        }

        public IRenderable GetRenderableNode()
        {
            if (skel == null) 
                return null;

            if (skeleton == null)
            {
                skeleton = new RSkeleton();

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
            }

            skeleton.Reset();

            return skeleton;
        }
    }
}
