using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Tools;
using OpenTK;
using SSBHLib.Formats.Animation;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuanmb")]
    public class NuanimNode : FileNode
    {
        private Anim animation;

        public NuanimNode(string path): base(path)
        {
            ImageKey = "animation";
        }
        
        public override void Open()
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out animation);
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

        public IRenderableAnimation GetRenderableAnimation()
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
    }
}
