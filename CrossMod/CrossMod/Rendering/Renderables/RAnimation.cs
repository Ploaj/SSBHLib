using OpenTK;
using SFGraphics.Cameras;
using SSBHLib.Tools;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RAnimation : IRenderableAnimation
    {
        public int FrameCount { get; set; }

        public List<RTransformAnimation> TransformNodes = new List<RTransformAnimation>();
        public List<RVisibilityAnimation> VisibilityNodes = new List<RVisibilityAnimation>();
        public List<RMaterialAnimation> MaterialNodes = new List<RMaterialAnimation>();
        public List<RCameraAnimation> CameraNodes = new List<RCameraAnimation>();

        public int GetFrameCount()
        {
            return FrameCount;
        }

        public void SetFrameModel(IEnumerable<Models.RMesh> subMeshes, float frame)
        {
            // Visibility
            foreach (RVisibilityAnimation a in VisibilityNodes)
            {
                foreach (Models.RMesh m in subMeshes)
                {
                    // names match with start ignoreing the _VIS tags
                    if (m.Name.StartsWith(a.MeshName))
                    {
                        m.IsVisible = a.Visibility.GetValue(frame);
                    }
                }
            }

            // Material
            foreach (Models.RMesh m in subMeshes)
            {
                if (m.Material != null)
                {
                    m.Material.Vec4ParamsMaterialAnimation.Clear();
                }
            }

            foreach (RMaterialAnimation a in MaterialNodes)
            {
                foreach (Models.RMesh m in subMeshes)
                {
                    if (m.Material != null && m.Material.MaterialLabel.Equals(a.MaterialName))
                    {
                        // Ensure the properties actually update.
                        m.Material.StartMaterialAnimation();

                        if (System.Enum.TryParse(a.AttributeName, out SSBHLib.Formats.Materials.MatlEnums.ParamId paramId))
                        {
                            m.Material.Vec4ParamsMaterialAnimation[paramId] = a.Keys.GetValue(frame);
                        }
                    }
                }
            }
        }

        public void SetFrameSkeleton(IEnumerable<RBone> bones, float frame)
        {
            // TODO: Model scale?
            float scale = RenderSettings.Instance.ModelScale;
            // BoneTransform
            foreach (RBone b in bones)
            {
                foreach (RTransformAnimation a in TransformNodes)
                {
                    if (b.Name.Equals(a.Name))
                    {
                        var key = a.Transform.GetKey(frame);
                        b.AnimationTrackTransform = key.Value;
                        break;
                    }
                }
            }
        }

        public void SetFrameCamera(Camera camera, float frame)
        {
            foreach (var a in CameraNodes)
            {
                var key = a.FieldOfView.GetKey(frame);
                camera.FovRadians = key.Value;
            }

            foreach (var a in TransformNodes)
            {
                // TODO: Should this be case sensitive and use equality?
                if (a.Name.ToLower().Contains("gya_camera"))
                {
                    var key = a.Transform.GetKey(frame);
                    // TODO: Store the translation, rotation, etc instead of decomposing the matrix again?
                    // Another option is to allow overriding the MVP matrix.
                    //camera.TranslationZ = key.Value.ExtractTranslation().Z;
                }
            }

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
        public RKeyGroup<AnimTrackTransform> Transform { get; } = new RKeyGroup<AnimTrackTransform>();
    }

    public class RCameraAnimation
    {
        public RKeyGroup<float> FieldOfView { get; } = new RKeyGroup<float>();
        // TODO: Add near clip and other missing attributes.
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
        public float CompensateScale; // workaround for strange scaling type
        public bool HasCompensateScale;
    }
}
