using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Tools;
using OpenTK;
using SSBHLib.Formats.Animation;
using CrossMod.IO;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuanmb")]
    public class NuanimNode : FileNode, IExportableAnimationNode
    {
        private Anim animation;

        public NuanimNode(string path): base(path)
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }
        
        public override void Open()
        {
            if (Ssbh.TryParseSsbhFile(AbsolutePath, out SsbhFile ssbhFile))
            {
                if (ssbhFile is Anim anim)
                {
                    animation = anim;
                }
            }
        }

        public string GetLightInformation()
        {
            SsbhAnimTrackDecoder decoder = new SsbhAnimTrackDecoder(animation);

            var output = new System.Text.StringBuilder();
            foreach (AnimGroup animGroup in animation.Animations)
            {
                AddLightSetInfo(output, decoder, animGroup);
            }

            return output.ToString();
        }

        public void UpdateUniqueLightValues(Dictionary<string, HashSet<string>> valuesByName)
        {
            SsbhAnimTrackDecoder decoder = new SsbhAnimTrackDecoder(animation);

            foreach (AnimGroup animGroup in animation.Animations)
            {
                AddLightValues(valuesByName, decoder, animGroup);
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

            SsbhAnimTrackDecoder decoder = new SsbhAnimTrackDecoder(animation);

            foreach (AnimGroup animGroup in animation.Animations)
            {
                if (animGroup.Type == AnimType.Material)
                {
                    ReadMaterialAnimations(renderAnimation, decoder, animGroup);
                }
                else if (animGroup.Type == AnimType.Visibility)
                {
                    ReadVisAnimations(renderAnimation, decoder, animGroup);
                }
                else if (animGroup.Type == AnimType.Transform)
                {
                    ReadBoneAnimations(renderAnimation, decoder, animGroup);
                }
            }
            
            return renderAnimation;
        }

        private static void ReadMaterialAnimations(RAnimation renderAnimation, SsbhAnimTrackDecoder decoder, AnimGroup animGroup)
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
                    object[] materialAnim = decoder.ReadTrack(track);

                    // only get vectors for now
                    if (materialAnim == null || materialAnim.Length == 0 || materialAnim[0] == null || materialAnim[0].GetType() != typeof(AnimTrackCustomVector4))
                    {
                        continue;
                    }
                    renderAnimation.MaterialNodes.Add(matAnim);
                    for (int i = 0; i < materialAnim.Length; i++)
                    {
                        var vec = (AnimTrackCustomVector4)materialAnim[i];
                        matAnim.Keys.Keys.Add(new RKey<Vector4>()
                        {
                            Frame = i,
                            Value = new Vector4(vec.X, vec.Y, vec.Z, vec.W)
                        });
                    }
                }
            }
        }

        private static void ReadBoneAnimations(RAnimation renderAnimation, SsbhAnimTrackDecoder decoder, AnimGroup animGroup)
        {
            foreach (AnimNode animNode in animGroup.Nodes)
            {
                RTransformAnimation tfrmAnim = new RTransformAnimation()
                {
                    Name = animNode.Name
                };
                foreach (AnimTrack track in animNode.Tracks)
                {
                    object[] transform = decoder.ReadTrack(track);

                    if (track.Name.Equals("Transform"))
                    {
                        for (int i = 0; i < transform.Length; i++)
                        {
                            AnimTrackTransform t = (AnimTrackTransform)transform[i];
                            tfrmAnim.Transform.Keys.Add(new RKey<Matrix4>()
                            {
                                Frame = i,
                                Value = GetMatrix((AnimTrackTransform)transform[i]),
                                AbsoluteScale = t.CompensateScale
                            });
                        }
                    }
                }
                renderAnimation.TransformNodes.Add(tfrmAnim);
            }
        }

        private static void ReadVisAnimations(RAnimation renderAnimation, SsbhAnimTrackDecoder decoder, AnimGroup animGroup)
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
                        object[] visibility = decoder.ReadTrack(track);

                        for (int i = 0; i < visibility.Length; i++)
                        {
                            visAnim.Visibility.Keys.Add(new RKey<bool>()
                            {
                                Frame = i,
                                Value = (bool)visibility[i]
                            });
                        }
                    }
                }
                renderAnimation.VisibilityNodes.Add(visAnim);
            }
        }

        private static void AddLightSetInfo(System.Text.StringBuilder output, SsbhAnimTrackDecoder decoder, AnimGroup animGroup)
        {
            foreach (AnimNode animNode in animGroup.Nodes)
            {
                output.AppendLine(animNode.Name);

                foreach (AnimTrack track in animNode.Tracks)
                {
                    object[] values = decoder.ReadTrack(track);

                    output.AppendLine($"\t{track.Name}");
                    foreach (var value in values)
                    {
                        output.AppendLine($"\t\t{value}");
                    }
                }
            }
        }

        private static void AddLightValues(Dictionary<string, HashSet<string>> valuesByName, SsbhAnimTrackDecoder decoder, AnimGroup animGroup)
        {
            // Store all unique values for each parameter.
            foreach (AnimNode animNode in animGroup.Nodes)
            {
                foreach (AnimTrack track in animNode.Tracks)
                {
                    if (!valuesByName.ContainsKey(track.Name))
                        valuesByName[track.Name] = new HashSet<string>();

                    object[] values = decoder.ReadTrack(track);

                    foreach (var value in values)
                    {
                        if (!valuesByName[track.Name].Contains(value.ToString()))
                            valuesByName[track.Name].Add(value.ToString());
                    }
                }
            }
        }

        private static Matrix4 GetMatrix(AnimTrackTransform transform)
        {
            return Matrix4.CreateScale(transform.Sx, transform.Sy, transform.Sz) *
                Matrix4.CreateFromQuaternion(new Quaternion(transform.Rx, transform.Ry, transform.Rz, transform.Rw)) *
                Matrix4.CreateTranslation(transform.X, transform.Y, transform.Z);
        }

        public IOAnimation GetIOAnimation()
        {
            IOAnimation anim = new IOAnimation
            {
                Name = Text,
                FrameCount = animation.FrameCount,
                RotationType = IORotationType.Quaternion
            };

            SsbhAnimTrackDecoder decoder = new SsbhAnimTrackDecoder(animation);

            foreach (AnimGroup animGroup in animation.Animations)
            {
                // Bone Animations
                if (animGroup.Type == AnimType.Transform)
                {
                    foreach (AnimNode animNode in animGroup.Nodes)
                    {
                        foreach (AnimTrack track in animNode.Tracks)
                        {
                            if (track.Name.Equals("Transform"))
                            {
                                object[] transform = decoder.ReadTrack(track);
                                for (int i = 0; i < transform.Length; i++)
                                {
                                    AnimTrackTransform t = (AnimTrackTransform)transform[i];
                                    anim.AddKey(animNode.Name, IOTrackType.POSX, i, t.X);
                                    anim.AddKey(animNode.Name, IOTrackType.POSY, i, t.Y);
                                    anim.AddKey(animNode.Name, IOTrackType.POSZ, i, t.Z);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTX, i, t.Rx);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTY, i, t.Ry);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTZ, i, t.Rz);
                                    anim.AddKey(animNode.Name, IOTrackType.ROTW, i, t.Rw);
                                    anim.AddKey(animNode.Name, IOTrackType.SCAX, i, t.Sx);
                                    anim.AddKey(animNode.Name, IOTrackType.SCAY, i, t.Sy);
                                    anim.AddKey(animNode.Name, IOTrackType.SCAZ, i, t.Sz);
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
