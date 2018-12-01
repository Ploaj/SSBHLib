using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace CrossMod.Rendering
{
    public class RSkeleton
    {
        public List<RBone> Bones = new List<RBone>();

        public Matrix4[] GetTransforms()
        {
            Matrix4[] Transforms = new Matrix4[Bones.Count];
            for(int i = 0; i < Bones.Count; i++)
            {
                Transforms[i] = Bones[i].Transform;
            }
            return Transforms;
        }

        public Matrix4[] GetWorldTransforms()
        {
            Matrix4[] Transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                Transforms[i] = Bones[i].WorldTransform;
            }
            return Transforms;
        }

        public Matrix4[] GetInvTransforms()
        {
            Matrix4[] Transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                Transforms[i] = Bones[i].InvTransform;
            }
            return Transforms;
        }

        public Matrix4[] GetInvWorldTransforms()
        {
            Matrix4[] Transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                Transforms[i] = Bones[i].InvWorldTransform;
            }
            return Transforms;
        }
    }

    public class RBone
    {
        public string Name;
        public int ID;
        public int ParentID;

        public Matrix4 Transform;
        public Matrix4 InvTransform;
        public Matrix4 WorldTransform;
        public Matrix4 InvWorldTransform;

        public Vector3 Position
        {
            get
            {
                return Transform.ExtractTranslation();
            }
        }
        public Quaternion Rotation
        {
            get
            {
                return Transform.ExtractRotation();
            }
        }
        public Vector3 Scale
        {
            get
            {
                return Transform.ExtractScale();
            }
        }
    }
}
