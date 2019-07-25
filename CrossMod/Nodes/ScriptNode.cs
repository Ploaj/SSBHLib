using CrossMod.Rendering;
using CrossMod.Rendering.Shapes;
using CrossMod.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using System;
using System.Collections.Generic;
using System.IO;

namespace CrossMod.Nodes
{
    [FileType(".script")]
    public class ScriptNode : FileNode
    {
        public Dictionary<string, Script> Scripts { get; set; }
        public Dictionary<ulong, int> BoneIDs { get; set; }
        
        public Attack[] Attacks { get; set; }
        public Catch[] Grabs { get; set; }
        public float MotionRate { get; set; } = 1f;

        private static Sphere Sphere { get; set; }
        private static  Capsule Capsule { get; set; }
        private static Line Line { get; set; }
        
        public SkelNode SkelNode { set
            {
                Skel = value.GetRenderableNode() as RSkeleton;
                BoneIDs.Clear();
                BoneIDs.Add(Hash.Hash40("top"), 0);
                for (int i = 0; i < Skel.Bones.Count; i++)
                    BoneIDs.Add(Hash.Hash40(Skel.Bones[i].Name), i);
            } }
        public RSkeleton Skel { get; set; }
        public string CurrentAnimationName { get; set; }

        public ScriptNode(string path) : base(path)
        {
            Scripts = new Dictionary<string, Script>();
            BoneIDs = new Dictionary<ulong, int>();
            Attacks = new Attack[8];//possibly fighter specific value
            Grabs = new Catch[8];
            for (int i = 0; i < Attacks.Length; i++)
                Attacks[i] = Attack.Default();
            for (int i = 0; i < Grabs.Length; i++)
                Grabs[i] = Catch.Default();
            ReadScriptFile();
        }

        public void Update(float frame)
        {
            if (Scripts.ContainsKey(CurrentAnimationName))
                Scripts[CurrentAnimationName].Update(frame);
        }

        public void Render(Camera camera)
        {
            if (Sphere == null)
                Sphere = new Sphere();

            if (Capsule == null)
                Capsule = new Capsule();

            if (Line == null)
                Line = new Line();

            var sphereShader = ShaderContainer.GetShader("Sphere");
            var capsuleShader = ShaderContainer.GetShader("Capsule");
            var lineShader = ShaderContainer.GetShader("Line");

            Matrix4 mvp = camera.MvpMatrix;

            List<Collision> collisions = new List<Collision>();
            collisions.AddRange(Attacks);
            collisions.AddRange(Grabs);
            
            GL.Disable(EnableCap.DepthTest);

            for (int i = 0; i < collisions.Count; i++)
            {
                Collision coll = collisions[i];
                if (!coll.Enabled)
                    continue;
                
                Vector4 collColor = new Vector4(Collision.IDColors[i % Collision.IDColors.Length], 0.5f);

                Matrix4 boneTransform = Skel.GetAnimationSingleBindsTransform(BoneIDs[coll.Bone]);
                Matrix4 boneNoScale =
                    Matrix4.CreateFromQuaternion(boneTransform.ExtractRotation())
                    * Matrix4.CreateTranslation(boneTransform.ExtractTranslation());
                
                if (IsSphere(coll))
                {
                    Sphere.Render(sphereShader, coll.Size, coll.Pos, boneNoScale, mvp, collColor);
                }
                else if (coll.ShapeType == Collision.Shape.capsule)
                {
                    Capsule.Render(capsuleShader, coll.Size, coll.Pos, coll.Pos2, boneNoScale, mvp, collColor);
                }
                //angle marker
                if (coll is Attack attack)
                {
                    int angle = attack.Angle;
                    Vector4 angleColor = new Vector4(1, 1, 1, 1);
                    GL.LineWidth(2f);

                    if (angle < 361)
                    {
                        float radian = angle * (float)Math.PI / 180f;
                        Line.Render(lineShader, radian, coll.Size, coll.Pos, boneNoScale, mvp, angleColor);
                    }
                    else if (angle == 361)
                    {
                        float radian = (float)Math.PI / 2f;
                        for (int j = 0; j < 4; j++)
                            Line.Render(lineShader, radian * j, coll.Size / 2, coll.Pos, boneNoScale, mvp, angleColor);
                    }
                    else if (angle == 368)
                    {
                        //set_vec_target_pos uses a second position to pull opponents toward
                        //this is represented by a line drawn between hitbox center and that point
                        if (attack.VecTargetPos_node != 0)
                        {
                            var attackVecBone = Skel.GetAnimationSingleBindsTransform(BoneIDs[attack.VecTargetPos_node]);
                            var attackVecBoneNoScale =
                                Matrix4.CreateFromQuaternion(attackVecBone.ExtractRotation())
                                * Matrix4.CreateTranslation(attackVecBone.ExtractTranslation());
                            Line.Render(lineShader,
                                boneNoScale, attackVecBoneNoScale,
                                coll.Pos, Vector3.Zero,
                                Vector3.Zero, new Vector3(attack.VecTargetPos_pos.Z, attack.VecTargetPos_pos.Y, 0),
                                mvp, angleColor
                            );
                        }
                    }
                    else
                    {
                        float radian = (float)Math.PI / 2f;
                        float add = (float)Math.PI / 4f;
                        for (int j = 0; j < 4; j++)
                            Line.Render(lineShader, radian * j + add, coll.Size / 2, coll.Pos, boneNoScale, mvp, angleColor);
                    }
                }
            }

            GL.Enable(EnableCap.DepthTest);
        }

        private bool IsSphere(Collision coll)
        {
            if (coll.ShapeType == Collision.Shape.sphere)
                return true;
            if (coll.ShapeType == Collision.Shape.capsule)
                return coll.Pos == coll.Pos2;
            return false;
        }

        private void ReadScriptFile()
        {
            string[] lines = File.ReadAllLines(AbsolutePath);
            List<string> commands = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#begin "))
                {
                    string name = lines[i++].Substring("#begin ".Length);
                    commands.Clear();
                    for (int j = i; (i = j) < lines.Length; j++)
                    {
                        if (lines[j].StartsWith("#end"))
                        {
                            i++;
                            break;
                        }
                        commands.Add(lines[j]);
                    }
                    Scripts.Add(name, new Script(commands, this));
                }
            }
        }

        public void SetAnimation(string name)
        {
            CurrentAnimationName = name;
            if (Scripts.ContainsKey(CurrentAnimationName))
                Scripts[CurrentAnimationName].Start();
        }

        public void Start()
        {
            if (CurrentAnimationName == null)
                return;

            for (int i = 0; i < Attacks.Length; i++)
                Attacks[i] = Attack.Default();
            for (int i = 0; i < Grabs.Length; i++)
                Grabs[i] = Catch.Default();
            MotionRate = 1f;

            if (Scripts.ContainsKey(CurrentAnimationName))
                Scripts[CurrentAnimationName].Start();
        }

        public class Script
        {
            public Command[] Commands;

            public int Position { get; set; }
            public float ACMDFrame { get; set; }
            public float WaitUntil { get; set; }

            private ScriptNode Observer { get; set; }

            public Script(List<string> wholeCommands, ScriptNode observer)
            {
                Observer = observer;
                Commands = new Command[wholeCommands.Count];
                for (int i = 0; i < Commands.Length; i++)
                {
                    string current = wholeCommands[i];
                    int firstSpace = current.IndexOf(' ');
                    if (firstSpace > 0)//if there are spaces
                    {
                        Command.CmdType type = (Command.CmdType)Enum.Parse(typeof(Command.CmdType), current.Substring(0, firstSpace));
                        string[] args = current.Substring(firstSpace + 1).Split(' ');
                        Commands[i] = new Command(type, args, this);
                    }
                    else//if there are no spaces
                    {
                        Command.CmdType type = (Command.CmdType)Enum.Parse(typeof(Command.CmdType), current);
                        string[] args = new string[0];
                        Commands[i] = new Command(type, args, this);
                    }
                }
            }

            public void Start()
            {
                Position = 0;
                WaitUntil = 0;
            }

            public void Update(float frame)
            {
                ACMDFrame = frame + 1;//frame in ACMD is indexed at 1 (for now...)
                while (Position < Commands.Length)
                {
                    if (ACMDFrame < WaitUntil)
                        return;
                    Commands[Position++].Interpret();
                }
            }

            public class Command
            {
                public CmdType type;
                public string[] args;

                private Script Parent { get; set; }

                public Command(CmdType type, string[] args, Script parent)
                {
                    this.type = type;
                    this.args = args;
                    Parent = parent;
                }

                public void Interpret()
                {
                    switch (type)
                    {
                        case CmdType.frame:
                            Parent.WaitUntil = IntParse(args[0]);
                            break;
                        case CmdType.wait:
                            Parent.WaitUntil = Parent.ACMDFrame + IntParse(args[0]);
                            break;
                        case CmdType.attack:
                            {
                                Parent.Observer.Attacks[IntParse(args[0])] = new Attack(
                                    UlongParse(args[1]),
                                    float.Parse(args[2]),
                                    IntParse(args[3]),
                                    float.Parse(args[4]),
                                    new Vector3(
                                        float.Parse(args[5]),
                                        float.Parse(args[6]),
                                        float.Parse(args[7])));
                            }
                            break;
                        case CmdType.attack_capsule:
                            {
                                Parent.Observer.Attacks[IntParse(args[0])] = new Attack(
                                    UlongParse(args[1]),
                                    float.Parse(args[2]),
                                    IntParse(args[3]),
                                    float.Parse(args[4]),
                                    new Vector3(
                                        float.Parse(args[5]),
                                        float.Parse(args[6]),
                                        float.Parse(args[7])),
                                    new Vector3(
                                        float.Parse(args[8]),
                                        float.Parse(args[9]),
                                        float.Parse(args[10])));
                            }
                            break;
                        case CmdType.atk_clear:
                            Parent.Observer.Attacks[IntParse(args[0])].Enabled = false;
                            break;
                        case CmdType.atk_clear_all:
                            foreach (var attack in Parent.Observer.Attacks)
                                attack.Enabled = false;
                            break;
                        case CmdType.@catch:
                            {
                                Parent.Observer.Grabs[int.Parse(args[0])] = new Catch(
                                    UlongParse(args[1]),
                                    float.Parse(args[2]),
                                    new Vector3(
                                        float.Parse(args[3]),
                                        float.Parse(args[4]),
                                        float.Parse(args[5])));
                            }
                            break;
                        case CmdType.catch_capsule:
                            {
                                Parent.Observer.Grabs[int.Parse(args[0])] = new Catch(
                                    UlongParse(args[1]),
                                    float.Parse(args[2]),
                                    new Vector3(
                                        float.Parse(args[3]),
                                        float.Parse(args[4]),
                                        float.Parse(args[5])),
                                    new Vector3(
                                        float.Parse(args[6]),
                                        float.Parse(args[7]),
                                        float.Parse(args[8])));
                            }
                            break;
                        case CmdType.catch_clear:
                            Parent.Observer.Grabs[IntParse(args[0])].Enabled = false;
                            break;
                        case CmdType.catch_clear_all:
                            foreach (var grab in Parent.Observer.Grabs)
                                grab.Enabled = false;
                            break;
                        case CmdType.ft_motion_rate:
                            Parent.Observer.MotionRate = 1f / float.Parse(args[0]);
                            break;
                        case CmdType.set_vec_target_pos:
                            Parent.Observer.Attacks[IntParse(args[0])].SetVecTargetPos(
                                UlongParse(args[1]),
                                new Vector2(
                                    float.Parse(args[2]),
                                    float.Parse(args[3])
                                    ),
                                float.Parse(args[4]));
                            break;
                    }
                }

                public enum CmdType
                {
                    frame,
                    wait,
                    attack,
                    attack_capsule,
                    atk_clear,
                    atk_clear_all,
                    @catch,
                    catch_capsule,
                    catch_clear,
                    catch_clear_all,
                    ft_motion_rate,
                    set_vec_target_pos
                }

                private int IntParse(string word)
                {
                    if (word.StartsWith("0x"))
                        return int.Parse(word.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    return int.Parse(word);
                }

                private ulong UlongParse(string word)
                {
                    if (word.StartsWith("0x"))
                        return ulong.Parse(word.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    return ulong.Parse(word);
                }
            }
        }
    }
}
