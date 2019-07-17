using CrossMod.Rendering;
using SSBHLib.Formats;
using SSBHLib;
using OpenTK;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nusktb")]
    public class SkelNode : FileNode, IRenderableNode
    {
        private SKEL skel;

        private RSkeleton skeleton;

        public SkelNode(string path) : base(path)
        {
            ImageKey = "skeleton";
            SelectedImageKey = "skeleton";
        }

        public override void Open()
        {
            if (SSBH.TryParseSSBHFile(AbsolutePath, out var SSBHFile))
            {
                if (SSBHFile is SKEL)
                {
                    skel = (SKEL)SSBHFile;
                }
            }
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
                        ID = skel.BoneEntries[i].ID,
                        ParentID = skel.BoneEntries[i].ParentID,
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

        private static Matrix4 SkelToTkMatrix(SKEL_Matrix sm)
        {
            return new Matrix4(sm.M11, sm.M12, sm.M13, sm.M14,
                sm.M21, sm.M22, sm.M23, sm.M24,
                sm.M31, sm.M32, sm.M33, sm.M34,
                sm.M41, sm.M42, sm.M43, sm.M44);
        }
    }
}
