using CrossMod.Rendering.ShapeMeshes;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Collections.Generic;
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

        public void Reset()
        {
            foreach (var bone in Bones)
            {
                bone.AnimationTransform = bone.Transform;
            }
        }

        public Matrix4[] GetTransforms()
        {
            Matrix4[] transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                transforms[i] = Bones[i].Transform;
            }
            return transforms;
        }

        public Matrix4[] GetWorldTransforms()
        {
            Matrix4[] transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                transforms[i] = Bones[i].WorldTransform;
            }
            return transforms;
        }

        public Matrix4[] GetInvTransforms()
        {
            Matrix4[] transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                transforms[i] = Bones[i].InvTransform;
            }
            return transforms;
        }

        public Matrix4[] GetInvWorldTransforms()
        {
            Matrix4[] transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                transforms[i] = Bones[i].InvWorldTransform;
            }
            return transforms;
        }

        public Matrix4[] GetAnimationTransforms()
        {
            Matrix4[] transforms = new Matrix4[Bones.Count];
            for (int i = 0; i < Bones.Count; i++)
            {
                transforms[i] = Bones[i].InvWorldTransform * Bones[i].GetAnimationTransform(this);
            }

            return transforms;
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
        public const float KEpsilon = 0.000001F;

        private static bool IsEqualUsingDot(float dot)
        {
            // Returns false in the presence of NaN values.
            return dot > 1.0f - KEpsilon;
        }

        public Matrix4 GetAnimationSingleBindsTransform(int index)
        {
            if (index != -1 && Bones.Count > 0)
                return Bones[index].GetAnimationTransform(this);

            return Matrix4.Identity;
        }

        public int GetBoneIndex(string boneName)
        {
            for (int i = 0; i < Bones.Count; i++)
            {
                if (Bones[i].Name.Equals(boneName))
                    return i;
            }
            return -1;
        }

        public void Render(Camera camera)
        {
            // Render skeleton on top.
            GL.Disable(EnableCap.DepthTest);

            if (bonePrism == null)
                bonePrism = new PrimBonePrism();

            if (boneShader == null)
            {
                boneShader = new Shader();
                boneShader.LoadShaders(File.ReadAllText("Shaders/Bone.vert"), File.ReadAllText("Shaders/Bone.frag"));
            }

            boneShader.UseProgram();

            boneShader.SetVector4("boneColor", RenderSettings.Instance.BoneColor);

            Matrix4 mvp = camera.MvpMatrix;
            boneShader.SetMatrix4x4("mvp", ref mvp);
            boneShader.SetMatrix4x4("rotation", ref prismRotation);

            foreach (RBone b in Bones)
            {
                Matrix4 transform = b.GetAnimationTransform(this);
                boneShader.SetMatrix4x4("bone", ref transform);
                boneShader.SetInt("hasParent", b.ParentId != -1 ? 1 : 0);
                if (b.ParentId != -1)
                {
                    Matrix4 parenttransform = Bones[b.ParentId].GetAnimationTransform(this);
                    boneShader.SetMatrix4x4("parent", ref parenttransform);
                }
                bonePrism.Draw(boneShader);

                // leaf node
                boneShader.SetInt("hasParent", 0);
                bonePrism.Draw(boneShader);
            }

        }
    }

    public class RBone
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int ParentId { get; set; }

        public Matrix4 Transform
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
                AnimationTransform = value;
            }
        }
        private Matrix4 transform;

        public Matrix4 InvTransform { get; set; }
        public Matrix4 WorldTransform { get; set; }
        public Matrix4 InvWorldTransform { get; set; }

        // for rendering animation
        public Matrix4 AnimationTransform;

        public Matrix4 GetAnimationTransform(RSkeleton skeleton)
        {
            if (ParentId != -1)
            {
                return AnimationTransform * skeleton.Bones[ParentId].GetAnimationTransform(skeleton);
            }
            return AnimationTransform;
        }

    }

    public class RHelperBone
    {
        public string WatcherBone { get; set; }
        public string ParentBone { get; set; }
        public string HelperBoneName { get; set; }

        public Vector3 Aoi { get; set; }
        public Quaternion WatchRotation { get; set; }
        public Quaternion HelperTargetRotation { get; set; }
        public Vector3 MinRange { get; set; }
        public Vector3 MaxRange { get; set; }
    }
}
