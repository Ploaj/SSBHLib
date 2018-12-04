using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.Cameras;
using OpenTK;

namespace CrossMod.Rendering
{
    public class RAnimation : IRenderableAnimation
    {
        public int FrameCount { get; set; }

        public List<RTransformAnimation> TransformNodes = new List<RTransformAnimation>();
        public List<RVisibilityAnimation> VisibilityNodes = new List<RVisibilityAnimation>();

        public int GetFrameCount()
        {
            return FrameCount;
        }

        public void SetFrameModel(RModel Model, float Frame)
        {
            if (Model == null) return;

            // Visibility
            foreach(RVisibilityAnimation a in VisibilityNodes)
            {
                foreach(RMesh m in Model.subMeshes)
                {
                    // names match with start ignoreing the _VIS tags
                    if (m.Name.StartsWith(a.MeshName))
                    {
                        m.Visible = a.Visibility.GetValue(Frame);
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

    public class RVisibilityAnimation
    {
        public string MeshName;
        public RKeyGroup<bool> Visibility { get { return _visibility; } }
        private RKeyGroup<bool> _visibility = new RKeyGroup<bool>();
    }

    public class RTransformAnimation
    {
        public string Name;
        public RKeyGroup<Matrix4> Transform { get { return _transform; } }
        private RKeyGroup<Matrix4> _transform = new RKeyGroup<Matrix4>();
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
