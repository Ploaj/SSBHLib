using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Tools;
using OpenTK;
using SSBHLib.Formats.Animation;
using SELib;

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

        public bool ExportToSE( string fileName )
        {
            if (animation == null)
                return false; //don't know how you got here, but stay out.

            SSBHAnimTrackDecoder decoder = new SSBHAnimTrackDecoder(animation);

            SEAnim seOut = new SEAnim();

            foreach (AnimGroup animGroup in animation.Animations)
            {
                if (animGroup.Type != ANIM_TYPE.Transform) //SEAnim only supports transform-type animations.
                    return false;

                foreach (AnimNode animNode in animGroup.Nodes)
                {
                    string name = animNode.Name;

                    foreach (AnimTrack track in animNode.Tracks)
                    {
                        if (track.Name.Equals("Transform"))
                        {
                            /*
                             *  Array of AnimTrackTransform after being read by Decoder.
                             */
                            object[] Trans = decoder.ReadTrack(track);
                            
                            for(int i = 0; i < Trans.Length; i++)
                            {
                                AnimTrackTransform currFrame = (AnimTrackTransform)Trans[i];
                                seOut.AddTranslationKey(name, i, currFrame.X, currFrame.Y, currFrame.Z);
                                seOut.AddRotationKey(name, i, currFrame.RX, currFrame.RY, currFrame.RZ, currFrame.RW);
                                seOut.AddScaleKey(name, i, currFrame.SX, currFrame.SY, currFrame.SZ);
;                           }

                        }
                    }
                }
            }

            seOut.Write(fileName);

            return false;
        }

        public IRenderable GetRenderableNode()
        {
            if (animation == null) return null;
            RAnimation renderAnimation = new RAnimation() { FrameCount = (int)animation.FrameCount };

            SSBHAnimTrackDecoder decoder = new SSBHAnimTrackDecoder(animation);

            foreach (AnimGroup animGroup in animation.Animations)
            {
                // Material Animations
                if (animGroup.Type == ANIM_TYPE.Material)
                {
                    foreach (AnimNode animNode in animGroup.Nodes)
                    {
                        foreach (AnimTrack track in animNode.Tracks)
                        {
                            object[] MaterialAnim = decoder.ReadTrack(track);
                        }
                    }
                }
                // Visibility Animations
                if (animGroup.Type == ANIM_TYPE.Visibilty)
                {
                    foreach (AnimNode animNode in animGroup.Nodes)
                    {
                        RVisibilityAnimation visAnim = new RVisibilityAnimation()
                        {
                            MeshName = animNode.Name
                        };
                        foreach (AnimTrack track in animNode.Tracks)
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
                    foreach (AnimNode animNode in animGroup.Nodes)
                    {
                        RTransformAnimation tfrmAnim = new RTransformAnimation()
                        {
                            Name = animNode.Name
                        };
                        foreach (AnimTrack track in animNode.Tracks)
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
