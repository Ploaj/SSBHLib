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

        public void SetFrameModel(Models.RModel Model, float Frame)
        {
            if (Model == null) return;

            // Visibility
            foreach(RVisibilityAnimation a in VisibilityNodes)
            {
                foreach (Models.RMesh m in Model.subMeshes)
                {
                    // names match with start ignoreing the _VIS tags
                    if (m.Name.StartsWith(a.MeshName))
                    {
                        m.Visible = a.Visibility.GetValue(Frame);
                    }
                }
            }

            // Material
            foreach (Models.RMesh m in Model.subMeshes)
                m.Material.MaterialAnimation.Clear();

            foreach (RMaterialAnimation a in MaterialNodes)
            {
                System.Diagnostics.Debug.WriteLine($"Animation: {a.AttributeName} {a.Keys.GetValue(Frame)}");

                foreach (Models.RMesh m in Model.subMeshes)
                {
                    if (m.Material.Name.Equals(a.MaterialName))
                    {
                        if (System.Enum.TryParse(a.AttributeName, out SSBHLib.Formats.Materials.MatlEnums.ParamId paramId))
                        {

                            m.Material.MaterialAnimation.Add((long)paramId, a.Keys.GetValue(Frame));
                        }
                    }
                }
            }
        }

        public void SetFrameSkeleton(RSkeleton Skeleton, float Frame)
        {
            if (Skeleton == null) return;

            // BoneTransform
            foreach(RBone b in Skeleton.Bones)
            {
                foreach(RTransformAnimation a in TransformNodes)
                {
                    if (b.Name.Equals(a.Name))
                    {
                        b.AnimationTransform = a.Transform.GetValue(Frame);
                        break;
                    }
                }
            }
        }

        public void Render(Camera Camera)
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
    }
}
