using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Tools;
using OpenTK;
using SSBHLib.Formats.Animation;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuanmb")]
    public class NUANIM_Node : FileNode, IRenderableNode
    {
        private ANIM animation;

        public NUANIM_Node()
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }
        
        public override void Open(string Path)
        {
            ISSBH_File SSBHFile;
            if (SSBH.TryParseSSBHFile(Path, out SSBHFile))
            {
                if (SSBHFile is ANIM anim)
                {
                    animation = anim;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            if (animation == null) return null;
            RAnimation renderAnimation = new RAnimation() { FrameCount = (int)animation.FrameCount };

            SSBHAnimTrackDecoder decoder = new SSBHAnimTrackDecoder(animation);

            foreach (ANIM_Group animGroup in animation.Animations)
            {
                // Material Animations
                if (animGroup.Type == ANIM_TYPE.Material)
                {
                    foreach (ANIM_Node animNode in animGroup.Nodes)
                    {
                        foreach (ANIM_Track track in animNode.Tracks)
                        {
                            object[] MaterialAnim = decoder.ReadTrack(track);
                        }
                    }
                }
                // Visibility Animations
                if (animGroup.Type == ANIM_TYPE.Visibilty)
                {
                    foreach (ANIM_Node animNode in animGroup.Nodes)
                    {
                        RVisibilityAnimation visAnim = new RVisibilityAnimation()
                        {
                            MeshName = animNode.Name
                        };
                        foreach (ANIM_Track track in animNode.Tracks)
                        {
                            if (track.Name.Equals("Visibility"))
                            {
                                object[] Visibility = decoder.ReadTrack(track);
                                
                                for (int i = 0; i < Visibility.Length; i++)
                                {
                                    visAnim.Visibility.Keys.Add(new RKey<bool>()
                                    {
                                        Frame = i,
                                        Value = ((AnimTrackBool)Visibility[i]).Value
                                    });
                                }
                            }
                        }
                        renderAnimation.VisibilityNodes.Add(visAnim);
                    }
                }
                // Bone Animations
                if (animGroup.Type == ANIM_TYPE.Transform)
                {
                    foreach (ANIM_Node animNode in animGroup.Nodes)
                    {
                        RTransformAnimation tfrmAnim = new RTransformAnimation()
                        {
                            Name = animNode.Name
                        };
                        foreach (ANIM_Track track in animNode.Tracks)
                        {
                            if (track.Name.Equals("Transform"))
                            {
                                object[] Transform = decoder.ReadTrack(track);
                                for (int i = 0; i < Transform.Length; i++)
                                {
                                    AnimTrackTransform t = (AnimTrackTransform)Transform[i];
                                    tfrmAnim.Transform.Keys.Add(new RKey<Matrix4>()
                                    {
                                        Frame = i,
                                        Value = GetMatrix((AnimTrackTransform)Transform[i])
                                    });
                                }
                            }
                        }
                        renderAnimation.TransformNodes.Add(tfrmAnim);
                    }
                }
            }
            
                

            return renderAnimation;
        }

        private static Matrix4 GetMatrix(AnimTrackTransform Transform)
        {
            return Matrix4.CreateScale(Transform.SX, Transform.SY, Transform.SZ) *
                Matrix4.CreateFromQuaternion(new Quaternion(Transform.RX, Transform.RY, Transform.RZ, Transform.RW)) *
                Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
        }
    }
}
