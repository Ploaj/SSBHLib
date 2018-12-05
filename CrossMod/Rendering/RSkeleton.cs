using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using SFGraphics.GLObjects.Shaders;
using SFGraphics.Cameras;
using SFGenericModel;
using SFGenericModel.VertexAttributes;
using OpenTK.Graphics.OpenGL;

namespace CrossMod.Rendering
{
    public class RSkeleton : IRenderable
    {
        public List<RBone> Bones = new List<RBone>();
        public List<RHelperBone> HelperBone = new List<RHelperBone>();

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

        public Matrix4[] GetAnimationTransforms()
        {
            Matrix4[] Transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                Transforms[i] = Bones[i].InvWorldTransform * Bones[i].GetAnimationTransform(this);
            }

            // Process HelperBones

            /*foreach(RHelperBone hBone in HelperBone)
            {
                // get watcher bone
                RBone WatcherBone = Bones[GetBoneIndex(hBone.WatcherBone)];
                Quaternion watcherCurrentRotation = WatcherBone.GetAnimationTransform(this).ExtractRotation();
                
                RBone HelperBone = Bones[GetBoneIndex(hBone.HelperBoneName)];
            }*/

            return Transforms;
        }
        
        public Matrix4 GetAnimationSingleBindsTransform(int Index)
        {
            return Bones[Index].GetAnimationTransform(this);
        }

        public int GetBoneIndex(string BoneName)
        {
            for(int i = 0; i < Bones.Count; i++)
            {
                if (Bones[i].Name.Equals(BoneName))
                    return i;
            }
            return -1;
        }

        public void Render(Camera Camera)
        {
            //TODO:
        }
    }

    public class RBone
    {
        public string Name;
        public int ID;
        public int ParentID;

        public Matrix4 Transform
        {
            get
            {
                return _transform;
            }
            set
            {
                _transform = value;
                AnimationTransform = value;
            }
        }
        private Matrix4 _transform;
        public Matrix4 InvTransform;
        public Matrix4 WorldTransform;
        public Matrix4 InvWorldTransform;

        // for rendering animation
        public Matrix4 AnimationTransform;

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
        public Vector3 EulerRotation
        {
            get
            {
                return ToEulerAngles(InvTransform.ExtractRotation());
            }
        }
        public Vector3 Scale
        {
            get
            {
                return Transform.ExtractScale();
            }
        }

        public Matrix4 GetAnimationTransform(RSkeleton Skeleton)
        {
            if(ParentID != -1)
            {
                return AnimationTransform * Skeleton.Bones[ParentID].GetAnimationTransform(Skeleton);
            }
            return AnimationTransform;
        }

        private static float Clamp(float v, float min, float max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        private static Vector3 ToEulerAngles(Quaternion q)
        {
            Matrix4 mat = Matrix4.CreateFromQuaternion(q);
            float x, y, z;

            y = (float)Math.Asin(-Clamp(mat.M31, -1, 1));

            if (Math.Abs(mat.M31) < 0.99999)
            {
                x = (float)Math.Atan2(mat.M32, mat.M33);
                z = (float)Math.Atan2(mat.M21, mat.M11);
            }
            else
            {
                x = 0;
                z = (float)Math.Atan2(-mat.M12, mat.M22);
            }
            return new Vector3(x, y, z);
        }
    }

    public class RHelperBone
    {
        public string WatcherBone;
        public string HelperBoneName;

        public Vector3 AOI;
        public Quaternion WatchRotation;
        public Quaternion HelperTargetRotation;
        public Vector3 MinRange;
        public Vector3 MaxRange;
    }
}
