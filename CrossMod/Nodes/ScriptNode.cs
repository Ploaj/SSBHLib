﻿using CrossMod.Rendering;
using CrossMod.Tools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
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

        public Shader SphereShader { get; set; }
        public Shader CapsuleShader { get; set; }

        //Needs separate initialization
        public SKEL_Node SkelNode { set
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
            var vertices = SFShapes.ShapeGenerator.GetSpherePositions(Vector3.Zero, 1, 30).Item1;
            Collision.UnitSphere = GenerateSphere4(vertices);
            Collision.UnitCapsule = GenerateCapsule(vertices);

            Scripts = new Dictionary<string, Script>();
            BoneIDs = new Dictionary<ulong, int>();
            Attacks = new Attack[8];//possibly fighter specific value
            for (int i = 0; i < Attacks.Length; i++)
            {
                Attacks[i] = Attack.Default();
            }
            ReadScriptFile();

            SphereShader = new Shader();
            SphereShader.LoadShader(File.ReadAllText("Shaders/Sphere.frag"), ShaderType.FragmentShader);
            SphereShader.LoadShader(File.ReadAllText("Shaders/Sphere.vert"), ShaderType.VertexShader);
            CapsuleShader = new Shader();
            CapsuleShader.LoadShader(File.ReadAllText("Shaders/Capsule.frag"), ShaderType.FragmentShader);
            CapsuleShader.LoadShader(File.ReadAllText("Shaders/Capsule.vert"), ShaderType.VertexShader);
        }

        public void Update(float frame)
        {
            if (Scripts.ContainsKey(CurrentAnimationName))
                Scripts[CurrentAnimationName].Update(frame);
        }

        public void Render(Camera camera)
        {
            Matrix4 mvp = camera.MvpMatrix;

            for (int i = 0; i < Attacks.Length; i++)
            {
                Attack attack = Attacks[i];
                if (!attack.Enabled)
                    continue;

                Vector4 Color = new Vector4(Attack.AttackColors[i], 0.6f);
                Matrix4 boneTransform = Skel.GetAnimationSingleBindsTransform(BoneIDs[attack.Bone]).ClearScale();

                if (IsSphere(attack))
                {
                    SphereShader.UseProgram();
                    SphereShader.SetMatrix4x4("mvp", ref mvp);
                    SphereShader.SetVector4("color", Color);
                    SphereShader.SetMatrix4x4("bone", ref boneTransform);
                    SphereShader.SetVector3("offset", attack.Pos);
                    SphereShader.SetFloat("size", attack.Size);
                    attack.Draw(SphereShader, camera);
                }
                else if (attack.ShapeType == Collision.Shape.capsule)
                {
                    CapsuleShader.UseProgram();
                    CapsuleShader.SetMatrix4x4("mvp", ref mvp);
                    CapsuleShader.SetVector4("color", Color);

                    Vector3 position1 = Vector3.TransformPosition(attack.Pos, boneTransform);
                    Vector3 position2 = Vector3.TransformPosition(attack.Pos2, boneTransform);

                    Vector3 to = position2 - position1;
                    to.NormalizeFast();
                    Vector3 axis = Vector3.Cross(Vector3.UnitY, to);
                    float omega = (float)System.Math.Acos(Vector3.Dot(Vector3.UnitY, to));
                    Matrix4 rotation = Matrix4.CreateFromAxisAngle(axis, omega);

                    Matrix4 transform1 = rotation * Matrix4.CreateTranslation(position1);
                    Matrix4 transform2 = rotation * Matrix4.CreateTranslation(position2);

                    CapsuleShader.SetMatrix4x4("transform1", ref transform1);
                    CapsuleShader.SetMatrix4x4("transform2", ref transform2);

                    CapsuleShader.SetFloat("size", attack.Size);
                    attack.Draw(CapsuleShader, camera);
                }
            }
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
            for (int i = 0; i < Attacks.Length; i++)
            {
                Attacks[i] = Attack.Default();
            }
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
                            Parent.WaitUntil = float.Parse(args[0]);
                            break;
                        case CmdType.wait:
                            Parent.WaitUntil = Parent.ACMDFrame + float.Parse(args[0]);
                            break;
                        case CmdType.attack:
                            {
                                Parent.Observer.Attacks[IntParse(args[0])] = new Attack(
                                    UlongParse(args[1]),
                                    IntParse(args[2]),
                                    IntParse(args[3]),
                                    float.Parse(args[4]),
                                    new OpenTK.Vector3(
                                        float.Parse(args[5]),
                                        float.Parse(args[6]),
                                        float.Parse(args[7])));
                            }
                            break;
                        case CmdType.attack_capsule:
                            {
                                Parent.Observer.Attacks[IntParse(args[0])] = new Attack(
                                    UlongParse(args[1]),
                                    IntParse(args[2]),
                                    IntParse(args[3]),
                                    float.Parse(args[4]),
                                    new OpenTK.Vector3(
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
                    }
                }

                private int IntParse(string word)
                {
                    if (int.TryParse(word, out int result))
                        return result;
                    return int.Parse(word.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }

                private ulong UlongParse(string word)
                {
                    if (ulong.TryParse(word, out ulong result))
                        return result;
                    return ulong.Parse(word.Substring(2), System.Globalization.NumberStyles.HexNumber);
                }

                public enum CmdType
                {
                    frame,
                    wait,
                    attack,
                    attack_capsule,
                    atk_clear,
                    atk_clear_all,
                }
            }
        }

        private List<Vector4> GenerateSphere4(List<Vector3> sphere)
        {
            var sphere4 = new List<Vector4>();

            foreach (var v in sphere)
            {
                sphere4.Add(new Vector4(v, 0));
            }

            return sphere4;
        }

        private List<Vector4> GenerateCapsule(List<Vector3> sphere)
        {
            var capsule = new List<Vector4>();

            foreach (var v in sphere)
            {
                Vector4 value = new Vector4();
                value.Xyz = v;
                if (value.Y > 0)
                    value.W = 1;
                capsule.Add(value);
            }

            return capsule;
        }
    }
}
