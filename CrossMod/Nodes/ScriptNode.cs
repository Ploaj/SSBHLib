using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.Nodes
{
    [FileType(".script")]
    public class ScriptNode : FileNode
    {
        public Dictionary<string, Script> scripts;

        public ScriptNode(string path) : base(path)
        {
            scripts = new Dictionary<string, Script>();
            SetupScripts();
        }

        private void SetupScripts()
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
                    scripts.Add(name, new Script(commands));
                }
            }
        }

        public class Script
        {
            public Command[] commands;

            public Script(List<string> wholeCommands)
            {
                commands = new Command[wholeCommands.Count];
                for (int i = 0; i < commands.Length; i++)
                {
                    string current = wholeCommands[i];
                    int firstSpace = current.IndexOf(' ');
                    if (firstSpace > 0)
                    {
                        Command.CmdType type = (Command.CmdType)Enum.Parse(typeof(Command.CmdType), current.Substring(0, firstSpace));
                        string[] args = current.Substring(firstSpace + 1).Split(' ');
                        commands[i] = new Command(type, args);
                    }
                    else
                    {
                        Command.CmdType type = (Command.CmdType)Enum.Parse(typeof(Command.CmdType), current);
                        string[] args = new string[0];
                        commands[i] = new Command(type, args);
                    }
                }
            }

            public class Command
            {
                CmdType type;
                string[] args;

                public Command(CmdType type, string[] args)
                {
                    this.type = type;
                    this.args = args;
                }

                public enum CmdType
                {
                    frame,
                    wait,
                    attack,
                    attack_clear_all,
                }
            }
        }
    }
}
