using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Tools;
using OpenTK;
using SSBHLib.Formats.Animation;
using CrossMod.IO;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuanmb")]
    public class NUANIM_Node : FileNode, IRenderableNode, IExportableAnimationNode
    {
        private ANIM animation;

        public NUANIM_Node(string path): base(path)
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }
        
        public override void Open()
        {
            if (SSBH.TryParseSSBHFile(AbsolutePath, out ISSBH_File SSBHFile))
            {
                if (SSBHFile is ANIM anim)
                {
                    animation = anim;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            if (animation == null)
                return null;

            RAnimation renderAnimation = new RAnimation()
            {
                FrameCount = (int)animation.FrameCount
            };

            SSBHAnimTrackDecoder decoder = new SSBHAnimTrackDecoder(animation);

            foreach (AnimGroup animGroup in animation.Animations)
            {
                if (animGroup.Type == ANIM_TYPE.Material)
                {
                    ReadMaterialAnimations(renderAnimation, decoder, animGroup);
                }
                else if (animGroup.Type == ANIM_TYPE.Visibilty)
                {
                    ReadVisAnimations(renderAnimation, decoder, animGroup);
                }
                else if (animGroup.Type == ANIM_TYPE.Transform)
                {
                    ReadBoneAnimations(renderAnimation, decoder, animGroup);
                }
            }
            
            return renderAnimation;
        }

        private static void ReadMaterialAnimations(RAnimation renderAnimation, SSBHAnimTrackDecoder decoder, AnimGroup animGroup)
        {
            foreach (AnimNode animNode in animGroup.Nodes)
            {
                foreach (AnimTrack track in animNode.Tracks)
                {
                    RMaterialAnimation matAnim = new RMaterialAnimation()
                    {
                        MaterialName = animNode.Name,
                        AttributeName = track.Name
                    };
                    object[] MaterialAnim = decoder.ReadTrack(track);

                    // only get vectors for now
                    if (MaterialAnim == null || MaterialAnim.Length == 0 || MaterialAnim[0] == null || MaterialAnim[0].GetType() != typeof(AnimTrackCustomVector4))
                    {
                        continue;
                    }
                    renderAnimation.MaterialNodes.Add(matAnim);
                    for (int i = 0; i < MaterialAnim.Length; i++)
                    {
                        var vec = (AnimTrackCustomVector4)MaterialAnim[i];
                        matAnim.Keys.Keys.Add(new RKey<Vector4>()
                        {
                            Frame = i,
                            Value = new Vector4(vec.X, vec.Y, vec.Z, vec.W)
                        });
                    }
                }
            }
        }

        private static void ReadBoneAnimations(RAnimation renderAnimation, SSBHAnimTrackDecoder decoder, AnimGroup animGroup)
        {
            foreach (AnimNode animNode in animGroup.Nodes)
            {
                //System.Diagnostics.Debug.WriteLine(animNode.Name);

                RTransformAnimation tfrmAnim = new RTransformAnimation()
                {
                    Name = animNode.Name
                };
                foreach (AnimTrack track in animNode.Tracks)
                {
                    object[] Transform = decoder.ReadTrack(track);

                    //System.Diagnostics.Debug.WriteLine($"\t{track.Name}");
                    //foreach (var value in Transform)
                    //{
                    //    System.Diagnostics.Debug.WriteLine($"\t\t{value}");
                    //}

                    if (track.Name.Equals("Transform"))
                    {
                        for (int i = 0; i < Transform.Length; i++)
                        {
                            AnimTrackTransform t = (AnimTrackTransform)Transform[i];
                            tfrmAnim.Transform.Keys.Add(new RKey<Matrix4>()
                            {
                                Frame = i,
                                Value = GetMatrix((AnimTrackTransform)Transform[i]),
                                AbsoluteScale = t.AbsoluteScale
                            });
                        }
                    }
                }
                renderAnimation.TransformNodes.Add(tfrmAnim);
            }
        }

        private static void ReadVisAnimations(RAnimation renderAnimation, SSBHAnimTrackDecoder decoder, AnimGroup animGroup)
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
                                Value = (bool)Visibility[i]
                            });
                        }
                    }
                }
                renderAnimation.VisibilityNodes.Add(visAnim);
            }
        }

        private static Matrix4 GetMatrix(AnimTrackTransform Transform)
        {
            return Matrix4.CreateScale(Transform.SX, Transform.SY, Transform.SZ) *
                Matrix4.CreateFromQuaternion(new Quaternion(Transform.RX, Transform.RY, Transform.RZ, Transform.RW)) *
                Matrix4.CreateTranslation(Transform.X, Transform.Y, Transform.Z);
        }

        public IOAnimation GetIOAnimation()
        {
            IOAnimation anim = new IOAnimation
            {
                Name = Text,
                FrameCount = animation.FrameCount,
                RotationType = IORotationType.Quaternion
            };

            SSBHAnimTrackDecoder decoder = new SSBHAnimTrackDecoder(animation);

            foreach (AnimGroup animGroup in animation.Animations)
            {
                // Bone Animations
                if (animGroup.Type == ANIM_TYPE.Transform)
                {
                    foreach (AnimNode animNode in animGroup.Nodes)
                    {
                        foreach (AnimTrack track in animNode.Tracks)
                        {
                            if (track.Name.Equals("Transform"))
                            {
                                object[] Transform = decoder.ReadTrack(track);
                                for (int i = 0; i < Transform.Length; i++)
                                {
                                    AnimTrackTransform t = (AnimTrackTransform)Transform[i];
                                    anim.AddKey(animNode.Name, IOTrackType.POSX, i, t.X);
                                    anim.AddKey(animNode.Name, IOTrackType.POSY, i, t.Y);
                                    anim.AddKey(animNode.Name, IOTrackType.POSZ, i, t.Z);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTX, i, t.RX);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTY, i, t.RY);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTZ, i, t.RZ);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTW, i, t.RW);
                                    anim.AddKey(animNode.Name, IOTrackType.SCAX, i, t.SX);
                                    anim.AddKey(animNode.Name, IOTrackType.SCAY, i, t.SY);
                                    anim.AddKey(animNode.Name, IOTrackType.SCAZ, i, t.SZ);
                                }
                            }
                        }
                    }
                }
            }

            return anim;
        }
    }
}
