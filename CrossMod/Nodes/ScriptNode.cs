using System;
using System.Collections.Generic;
using System.IO;
using CrossMod.Rendering;

namespace CrossMod.Nodes
{
    [FileType(".script")]
    public class ScriptNode : FileNode
    {
        //Initialized with ScriptNode
        public Dictionary<string, Script> Scripts { get; set; }
        public Attack[] Attacks { get; set; }

        //Needs separate initialization
        public SKEL_Node SkelNode { set { RSkel = value.GetRenderableNode() as RSkeleton; } }
        public RSkeleton RSkel { get; set; }
        public string CurrentAnimationName { get; set; }

        public ScriptNode(string path) : base(path)
        {
            Scripts = new Dictionary<string, Script>();
            Attacks = new Attack[8];
            ReadScriptFile();
        }

        private void ReadScriptFile()
        {
            string[] lines = File.ReadAllLines(AbsolutePath);
            List<string> commands = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("#begin"))
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
            if (Scripts.ContainsKey(CurrentAnimationName))
                Scripts[CurrentAnimationName].Start();
        }

        public void Update(float frame)
        {
            if (Scripts.ContainsKey(CurrentAnimationName))
                Scripts[CurrentAnimationName].Update(frame);
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
                    else
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
                                Attack attack = new Attack(
                                    UlongParse(args[1]),
                                    IntParse(args[2]),
                                    IntParse(args[3]),
                                    float.Parse(args[4]),
                                    new OpenTK.Vector3(
                                        float.Parse(args[5]),
                                        float.Parse(args[6]),
                                        float.Parse(args[7])));
                                if (bool.Parse(args[8]))
                                {
                                    attack.ShapeType = Collision.Shape.capsule;
                                    attack.Pos2 = new OpenTK.Vector3(float.Parse(args[9]), float.Parse(args[10]), float.Parse(args[11]));
                                }
                                Parent.Observer.Attacks[IntParse(args[0])] = attack;
                            }
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
                    atk_clear_all,
                }
            }
        }
    }
}
