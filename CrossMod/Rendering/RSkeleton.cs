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
using System.IO;

namespace CrossMod.Rendering
{
    public class RSkeleton : IRenderable
    {
        // Processing
        public List<RBone> Bones = new List<RBone>();
        public List<RHelperBone> HelperBone = new List<RHelperBone>();

        // Rendering
        private PrimBonePrism bonePrism;
        public static Shader boneShader = null;
        private static Matrix4 prismRotation = Matrix4.CreateFromAxisAngle(new Vector3(0, 0, 1), 1.5708f);
        

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
                Quaternion watcherCurrentRotation = WatcherBone.AnimationTransform.ExtractRotation();
                ;
                if(hBone.HelperBoneName.Equals("H_SholderL"))
                System.Diagnostics.Debug.WriteLine(hBone.HelperBoneName + " " + Angle(hBone.WatchRotation, watcherCurrentRotation));
                float Angl = Angle(hBone.WatchRotation, watcherCurrentRotation);

                int index = GetBoneIndex(hBone.HelperBoneName);
                RBone HelperBone = Bones[index];
                HelperBone.AnimationTransform =
                Matrix4.CreateFromQuaternion(Quaternion.Slerp(HelperBone.Rotation, hBone.HelperTargetRotation, Angl)) * 
                Matrix4.CreateTranslation(HelperBone.Position);
                Transforms[index] = HelperBone.InvWorldTransform * HelperBone.GetAnimationTransform(this);
            }*/

            return Transforms;
        }

        public static float Angle(Quaternion a, Quaternion b)
        {
            float dot = Dot(a, b);
            return IsEqualUsingDot(dot) ? 0.0f : (float)Math.Acos(Math.Min(Math.Abs(dot), 1.0f)) * 2.0f;
        }

        public static float Dot(Quaternion a, Quaternion b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
        }
        public const float kEpsilon = 0.000001F;
        private static bool IsEqualUsingDot(float dot)
        {
            // Returns false in the presence of NaN values.
            return dot > 1.0f - kEpsilon;
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
            // Render skeleton on top.
            GL.Disable(EnableCap.DepthTest);

            // TODO: Render texture.
            if (bonePrism == null)
                bonePrism = new PrimBonePrism();

            if (boneShader == null)
            {
                boneShader = new Shader();
                boneShader.LoadShader(File.ReadAllText("Shaders/bone.frag"), ShaderType.FragmentShader);
                boneShader.LoadShader(File.ReadAllText("Shaders/bone.vert"), ShaderType.VertexShader);
            }
            
            boneShader.UseProgram();

            Matrix4 mvp = Camera.MvpMatrix;
            boneShader.SetMatrix4x4("mvp", ref mvp);
            boneShader.SetMatrix4x4("rotation", ref prismRotation);

            foreach (RBone b in Bones)
            {
                boneShader.SetMatrix4x4("bone", ref b.WorldTransform);
                boneShader.SetInt("hasParent", b.ParentID != -1 ? 1 : 0);
                if(b.ParentID != -1)
                {
                    boneShader.SetMatrix4x4("parent", ref Bones[b.ParentID].WorldTransform);
                }
                bonePrism.Draw(boneShader, null);

                // leaf node
                boneShader.SetInt("hasParent", 0); 
                bonePrism.Draw(boneShader, null);
            }

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
