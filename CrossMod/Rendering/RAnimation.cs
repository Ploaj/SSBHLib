using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.Cameras;

namespace CrossMod.Rendering
{
    public class RAnimation : IRenderableAnimation
    {
        public int FrameCount { get; set; }

        public List<RVisibilityAnimation> VisibilityNodes = new List<RVisibilityAnimation>();

        public int GetFrameCount()
        {
            return FrameCount;
        }

        public void SetFrameModel(RModel Model, float Frame)
        {
            if (Model == null) return;
            foreach(RVisibilityAnimation a in VisibilityNodes)
            {
                foreach(RMesh m in Model.subMeshes)
                {
                    if (m.Name.StartsWith(a.MeshName))
                    {
                        m.Visible = a.Visibility.GetValue(Frame) == 1;
                    }
                }
            }
        }

        public void SetFrameSkeleton(RSkeleton Skeleton, float Frame)
        {

        }

        public void Render(Camera Camera)
        {
        }
    }

    public class RVisibilityAnimation
    {
        public string MeshName;
        public RKeyGroup Visibility { get { return _visibility; } }
        private RKeyGroup _visibility = new RKeyGroup();
    }

    public class RTransformAnimation
    {
        public string Name;
    }

    public class RKeyGroup
    {
        public List<RKey> Keys = new List<RKey>();

        public float GetValue(float Frame)
        {
            //TODO: actually grab the right frame
            if (Frame >= Keys.Count)
                return Keys[0].Value;
            return Keys[(int)Frame].Value;
        }
    }

    public class RKey
    {
        public float Frame;
        public float Value;
    }
}
