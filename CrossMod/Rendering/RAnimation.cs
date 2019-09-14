using OpenTK;
using SFGraphics.Cameras;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RAnimation : IRenderableAnimation
    {
        public int FrameCount { get; set; }

        public List<RTransformAnimation> TransformNodes = new List<RTransformAnimation>();
        public List<RVisibilityAnimation> VisibilityNodes = new List<RVisibilityAnimation>();
        public List<RMaterialAnimation> MaterialNodes = new List<RMaterialAnimation>();

        public int GetFrameCount()
        {
            return FrameCount;
        }

        public void SetFrameModel(Models.RModel model, float frame)
        {
            if (model == null)
                return;

            // Visibility
            foreach(RVisibilityAnimation a in VisibilityNodes)
            {
                foreach (Models.RMesh m in model.subMeshes)
                {
                    // names match with start ignoreing the _VIS tags
                    if (m.Name.StartsWith(a.MeshName))
                    {
                        m.Visible = a.Visibility.GetValue(frame);
                    }
                }
            }

            // Material
            foreach (Models.RMesh m in model.subMeshes)
            {
                if(m.Material != null)
                {
                    m.Material.MaterialAnimation.Clear();
                    m.Material.CurrentFrame = frame;
                }
            }

            foreach (RMaterialAnimation a in MaterialNodes)
            {
                //System.Diagnostics.Debug.WriteLine($"Animation: {a.AttributeName} {a.Keys.GetValue(frame)}");

                foreach (Models.RMesh m in model.subMeshes)
                {
                    if (m.Material != null && m.Material.Name.Equals(a.MaterialName))
                    {
                        if (System.Enum.TryParse(a.AttributeName, out SSBHLib.Formats.Materials.MatlEnums.ParamId paramId))
                        {
                            m.Material.MaterialAnimation.Add((long)paramId, a.Keys.GetValue(frame));
                        }
                    }
                }
            }
        }

        public void SetFrameSkeleton(RSkeleton Skeleton, float Frame)
        {
            if (Skeleton == null) return;

            // Model scale
            float scale = RenderSettings.Instance.ModelScale;
            // BoneTransform
            foreach(RBone b in Skeleton.Bones)
            {
                foreach(RTransformAnimation a in TransformNodes)
                {
                    if (b.Name.Equals(a.Name))
                    {
                        var key = a.Transform.GetKey(Frame);
                        b.AnimationTransform = key.Value;
                        // work around
                        /*if (key.AbsoluteScale != 1)
                        {
                            if (b.ParentID != -1)
                            {
                                b.AnimationTransform = b.AnimationTransform.ClearScale();
                                if(b.Name.Equals("HandL"))
                                System.Console.WriteLine(key.AbsoluteScale + " " + (Skeleton.Bones[b.ParentID].GetAnimationTransform(Skeleton).ExtractScale() / key.AbsoluteScale).ToString());
                                Vector3 ParentScale = Skeleton.Bones[b.ParentID].GetAnimationTransform(Skeleton).ExtractScale();
                                b.AnimationTransform *= Matrix4.CreateScale(ParentScale.X - key.AbsoluteScale);
                            }
                        }*/
                        break;
                    }
                }
                //It's probably OK to do this
                if (b.ParentId == -1)
                    b.AnimationTransform *= Matrix4.CreateScale(scale);
            }
        }

        public void Render(Camera camera)
        {
        }
    }

    public class RMaterialAnimation
    {
        public string MaterialName;

        public string AttributeName;

        public RKeyGroup<Vector4> Keys = new RKeyGroup<Vector4>();
    }

    public class RVisibilityAnimation
    {
        public string MeshName { get; set; }
        public RKeyGroup<bool> Visibility { get; } = new RKeyGroup<bool>();
    }

    public class RTransformAnimation
    {
        public string Name { get; set; }
        public RKeyGroup<Matrix4> Transform { get; } = new RKeyGroup<Matrix4>();
    }

    public class RKeyGroup<T>
    {
        public List<RKey<T>> Keys = new List<RKey<T>>();


        public RKey<T> GetKey(float Frame)
        {
            //TODO: actually grab the right frame
            if (Frame >= Keys.Count)
                return Keys[0];
            return Keys[(int)Frame];
        }

        public T GetValue(float Frame)
        {
            //TODO: actually grab the right frame
            if (Frame >= Keys.Count)
                return Keys[0].Value;
            return Keys[(int)Frame].Value;
        }
    }

    public class RKey<T> 
    {
        public float Frame;
        public T Value;
        public float AbsoluteScale = 1; // workaround for strange scaling type
    }
}
