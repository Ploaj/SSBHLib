using System;
using CrossMod.Rendering;
using SSBHLib.Formats;
using SSBHLib;
using OpenTK;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nusktb")]
    public class SKEL_Node : FileNode, IRenderableNode
    {
        private SKEL _skel;

        private RSkeleton Skeleton;

        public SKEL_Node(string path) : base(path)
        {
            ImageKey = "skeleton";
            SelectedImageKey = "skeleton";
        }

        public override void Open()
        {
            ISSBH_File SSBHFile;
            if (SSBH.TryParseSSBHFile(AbsolutePath, out SSBHFile))
            {
                if(SSBHFile is SKEL)
                {
                    _skel = (SKEL)SSBHFile;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            if (_skel == null) return null;
            
            if(Skeleton == null)
            {
                Skeleton = new RSkeleton();

                for (int i = 0; i < _skel.BoneEntries.Length; i++)
                {
                    RBone Bone = new RBone();
                    Bone.Name = _skel.BoneEntries[i].Name;
                    Bone.ID = _skel.BoneEntries[i].ID;
                    Bone.ParentID = _skel.BoneEntries[i].ParentID;
                    Bone.Transform = Skel_to_TKMatrix(_skel.Transform[i]);
                    Bone.InvTransform = Skel_to_TKMatrix(_skel.InvTransform[i]);
                    Bone.WorldTransform = Skel_to_TKMatrix(_skel.WorldTransform[i]);
                    Bone.InvWorldTransform = Skel_to_TKMatrix(_skel.InvWorldTransform[i]);
                    Skeleton.Bones.Add(Bone);
                }
            }

            Skeleton.Reset();

            return Skeleton;
        }

        private Matrix4 Skel_to_TKMatrix(SKEL_Matrix sm)
        {
            return new Matrix4(sm.M11, sm.M12, sm.M13, sm.M14,
                sm.M21, sm.M22, sm.M23, sm.M24,
                sm.M31, sm.M32, sm.M33, sm.M34,
                sm.M41, sm.M42, sm.M43, sm.M44);
        }
    }
}
