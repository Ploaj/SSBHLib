using System;
using System.Collections.Generic;
using OpenTK;
using CrossMod.Rendering;
using System.Linq;
using System.Windows.Forms;

namespace CrossMod.IO
{
    class IO_MayaANIM
    {
        private enum InfinityType
        {
            constant,
            linear,
            cycle,
            cycleRelative,
            oscillate
        }

        private enum InputType
        {
            time,
            unitless
        }

        private enum OutputType
        {
            time,
            linear,
            angular,
            unitless
        }

        private enum ControlType
        {
            translate,
            rotate,
            scale
        }

        private enum TrackType
        {
            translateX,
            translateY,
            translateZ,
            rotateX,
            rotateY,
            rotateZ,
            scaleX,
            scaleY,
            scaleZ
        }

        private class Header
        {
            public float animVersion;
            public string mayaVersion;
            public float startTime;
            public float endTime;
            public float startUnitless;
            public float endUnitless;
            public string timeUnit;
            public string linearUnit;
            public string angularUnit;

            public Header()
            {
                animVersion = 1.1f;
                mayaVersion = "2015";
                startTime = 1;
                endTime = 1;
                startUnitless = 0;
                endUnitless = 0;
                timeUnit = "ntscf";
                linearUnit = "cm";
                angularUnit = "rad";
            }
        }

        private class AnimKey
        {
            public float input, output;
            public string intan, outtan;
            public float t1 = 0, w1 = 1;

            public AnimKey()
            {
                intan = "linear";
                outtan = "linear";
            }
        }

        private class AnimData
        {
            public ControlType controlType;
            public TrackType type;
            public InputType input;
            public OutputType output;
            public InfinityType preInfinity, postInfinity;
            public bool weighted = false;
            public List<AnimKey> keys = new List<AnimKey>();

            public AnimData()
            {
                input = InputType.time;
                output = OutputType.linear;
                preInfinity = InfinityType.constant;
                postInfinity = InfinityType.constant;
                weighted = false;
            }
        }

        private class AnimBone
        {
            public string name;
            public List<AnimData> atts = new List<AnimData>();
        }

        private Header header;
        private List<AnimBone> Bones = new List<AnimBone>();

        public IO_MayaANIM()
        {
            header = new IO_MayaANIM.Header();
        }

        public void Save(string fileName)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("animVersion " + header.animVersion + ";");
                file.WriteLine("mayaVersion " + header.mayaVersion + ";");
                file.WriteLine("timeUnit " + header.timeUnit + ";");
                file.WriteLine("linearUnit " + header.linearUnit + ";");
                file.WriteLine("angularUnit " + header.angularUnit + ";");
                file.WriteLine("startTime " + 1 + ";");
                file.WriteLine("endTime " + header.endTime + ";");

                int Row = 0;

                foreach(AnimBone animBone in Bones)
                {
                    int TrackIndex = 0;
                    if(animBone.atts.Count == 0)
                    {
                        file.WriteLine($"anim {animBone.name} 0 1 {TrackIndex++};");
                    }
                    foreach (AnimData animData in animBone.atts)
                    {
                        file.WriteLine($"anim {animData.controlType}.{animData.type} {animData.type} {animBone.name} 0 1 {TrackIndex++};");
                        file.WriteLine("animData {");
                        file.WriteLine($" input {animData.input};");
                        file.WriteLine($" output {animData.output};");
                        file.WriteLine($" weighted {(animData.weighted ? 1 : 0)};");
                        file.WriteLine($" preInfinity {animData.preInfinity};");
                        file.WriteLine($" postInfinity {animData.postInfinity};");

                        file.WriteLine(" keys {");
                        foreach (AnimKey key in animData.keys)
                        {
                            // TODO: fixed splines
                            file.WriteLine($" {key.input} {key.output:N6} {key.intan} {key.outtan} 1 1 0;");
                        }
                        file.WriteLine(" }");

                        file.WriteLine("}");
                    }
                    Row++;
                }
            }
        }

        public static List<RBone> getBoneTreeOrder(RSkeleton Skeleton)
        {
            if (Skeleton.Bones.Count == 0)
                return null;
            List<RBone> bone = new List<RBone>();
            Queue<RBone> q = new Queue<RBone>();

            foreach(RBone b in Skeleton.Bones)
            {
                if(b.ParentID == -1)
                    QueueBones(b, q, Skeleton);
            }

            while (q.Count > 0)
            {
                bone.Add(q.Dequeue());
            }
            return bone;
        }

        public static void QueueBones(RBone b, Queue<RBone> q, RSkeleton Skeleton)
        {
            q.Enqueue(b);
            foreach (RBone c in GetChildren(b, Skeleton))
                QueueBones(c, q, Skeleton);
        }

        public static List<RBone> GetChildren(RBone bone, RSkeleton Skeleton)
        {
            List<RBone> children = new List<RBone>();
            foreach(RBone b in Skeleton.Bones)
            {
                if(b.ParentID == bone.ID)
                {
                    children.Add(b);
                }
            }
            return children;
        }

        public static void ExportIOAnimationAsANIM(string fname, IOAnimation animation, RSkeleton Skeleton, bool ordinal = false)
        {
            IO_MayaANIM anim = new IO_MayaANIM();

            anim.header.endTime = animation.FrameCount + 1;

            // get bone order
            List<RBone> BonesInOrder = getBoneTreeOrder(Skeleton);
            
            if (ordinal)
            {
                BonesInOrder = BonesInOrder.OrderBy(f => f.Name, StringComparer.Ordinal).ToList();
            }

            foreach(RBone b in BonesInOrder)
            {
                AnimBone animBone = new AnimBone()
                {
                    name = b.Name
                };
                anim.Bones.Add(animBone);

                // Add Tracks
                if(animation.TryGetNodeByName(b.Name, out IOAnimNode ioAnimNode))
                {
                    AddAnimData(animBone, ioAnimNode, IOTrackType.POSX, ControlType.translate, TrackType.translateX);
                    AddAnimData(animBone, ioAnimNode, IOTrackType.POSY, ControlType.translate, TrackType.translateY);
                    AddAnimData(animBone, ioAnimNode, IOTrackType.POSZ, ControlType.translate, TrackType.translateZ);

                    // rotation
                    if(animation.RotationType == IORotationType.Euler)
                    {
                        // directly
                        AddAnimData(animBone, ioAnimNode, IOTrackType.ROTX, ControlType.rotate, TrackType.rotateX);
                        AddAnimData(animBone, ioAnimNode, IOTrackType.ROTY, ControlType.rotate, TrackType.rotateY);
                        AddAnimData(animBone, ioAnimNode, IOTrackType.ROTZ, ControlType.rotate, TrackType.rotateZ);
                    }
                    else if (animation.RotationType == IORotationType.Quaternion)
                    {
                        // convert to euler

                        AnimData rx = new AnimData(); rx.controlType = ControlType.rotate; rx.type = TrackType.rotateX;
                        AnimData ry = new AnimData(); ry.controlType = ControlType.rotate; ry.type = TrackType.rotateY;
                        AnimData rz = new AnimData(); rz.controlType = ControlType.rotate; rz.type = TrackType.rotateZ;
                        rx.output = OutputType.angular;
                        ry.output = OutputType.angular;
                        rz.output = OutputType.angular;

                        List<float> KeyFrames = new List<float>();
                        foreach (IOAnimKey key in ioAnimNode.GetKeysForTrack(IOTrackType.ROTX))
                        {
                            KeyFrames.Add(key.Frame);
                        }

                        for(int i = 0; i < KeyFrames.Count; i++)
                        {
                            Vector3 EulerAngles = Tools.CrossMath.ToEulerAnglesXYZ(ioAnimNode.GetQuaternionRotation(KeyFrames[i], b.Rotation));
                            rx.keys.Add(new AnimKey()
                            {
                                input = KeyFrames[i] + 1,
                                output = EulerAngles.X,
                            });
                            ry.keys.Add(new AnimKey()
                            {
                                input = KeyFrames[i] + 1,
                                output = EulerAngles.Y,
                            });
                            rz.keys.Add(new AnimKey()
                            {
                                input = KeyFrames[i] + 1,
                                output = EulerAngles.Z,
                            });
                        }

                        if(rx.keys.Count > 0)
                        {
                            animBone.atts.Add(rx);
                            animBone.atts.Add(ry);
                            animBone.atts.Add(rz);
                        }
                    }

                    // scale
                    AddAnimData(animBone, ioAnimNode, IOTrackType.SCAX, ControlType.scale, TrackType.scaleX);
                    AddAnimData(animBone, ioAnimNode, IOTrackType.SCAY, ControlType.scale, TrackType.scaleY);
                    AddAnimData(animBone, ioAnimNode, IOTrackType.SCAZ, ControlType.scale, TrackType.scaleZ);
                }
            }

            anim.Save(fname);
        }

        private static void AddAnimData(AnimBone animBone, IOAnimNode node, IOTrackType type, ControlType ctype, TrackType ttype)
        {
            AnimData d = new AnimData();
            d.controlType = ctype;
            d.type = ttype;
            foreach (IOAnimKey key in node.GetKeysForTrack(type))
            {
                AnimKey animKey = new AnimKey()
                {
                    input = key.Frame + 1,
                    output = key.Value,
                };
                d.keys.Add(animKey);
            }

            if (d.keys.Count > 0)
                animBone.atts.Add(d);
        }
    }
}
