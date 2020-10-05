using CrossMod.Rendering;
using SSBHLib.Formats;
using SSBHLib;
using OpenTK;

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
            if (skel == null) return null;

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
                        Transform = SkelToTkMatrix(skel.Transform[i]),
                        InvTransform = SkelToTkMatrix(skel.InvTransform[i]),
                        WorldTransform = SkelToTkMatrix(skel.WorldTransform[i]),
                        InvWorldTransform = SkelToTkMatrix(skel.InvWorldTransform[i])
                    };
                    skeleton.Bones.Add(bone);
                }
            }

            skeleton.Reset();

            return skeleton;
        }

        private static Matrix4 SkelToTkMatrix(Matrix4x4 sm)
        {
            return new Matrix4(sm.Row1.X, sm.Row1.Y, sm.Row1.Z, sm.Row1.W,
                sm.Row2.X, sm.Row2.Y, sm.Row2.Z, sm.Row2.W,
                sm.Row3.X, sm.Row3.Y, sm.Row3.Z, sm.Row3.W,
                sm.Row4.X, sm.Row4.Y, sm.Row4.Z, sm.Row4.W);
        }
    }
}
