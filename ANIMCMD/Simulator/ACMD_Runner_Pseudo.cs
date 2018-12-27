using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ANIMCMD.Simulator
{
    public class ACMD_Runner_Pseudo : ACMD_Runner
    {
        public string[] Lines;

        public ACMD_Runner_Pseudo() : base()
        {
            Lines = new string[0];
        }

        public string GetScript()
        {
            return string.Join("\n", Lines);
        }

        public void SetScript(string PseudoCode)
        {
            Lines = PseudoCode.Split('\n');
            for(int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = Regex.Replace(Lines[i], @"\s+", "");
            }
        }

        private bool isIf(string srccode)
        {
            return Regex.Match(srccode, "if(.*)").Success;
        }

        private bool isFunction(string srccode)
        {
            return Regex.Match(srccode, "::[^:]*\\(*\\)").Success;
        }

        private bool ProcessIF(string iffunction)
        {
            string func = Regex.Match(iffunction, "\\((.*?)\\)").Value.Substring(1);
            if (isFunction(func))
            {
                return (bool)RunFunction(GetFunctionName(func), GetParameters(func));
            }
            return false;
        }

        private string GetFunctionName(string srccode)
        {
            return Regex.Match(srccode, "::[^::^(]*\\(").Value.Substring(2).Replace("(", "");
        }

        private object[] GetParameters(string srccode)
        {
            return Regex.Match(srccode, "\\(([^)]+)\\)").Value.Replace("(", "").Replace(")", "").Split(',');
        }

        public override void ProcessFrame()
        {
            Parse(0, Lines.Length);
        }
        
        private void Parse(int start, int length)
        {
            for (int i = start; i < start + length; i++)
            {
                string line = Lines[i];
                
                // check "if" section
                if (isIf(line))
                {
                    // gather code within brackets
                    int j;
                    for (j = i+1; j < Lines.Length; j++)
                    {
                        if (Lines[j].Contains("}"))
                            break;
                    }
                    if(ProcessIF(line))
                        Parse(i + 2, j - i - 2);
                    i = j;
                }
                if (isFunction(line))
                {
                    RunFunction(GetFunctionName(line), GetParameters(line));
                }
            }
        }
    }
}
