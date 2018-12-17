using System;

namespace SSBHLib.IO
{
    public class ParseTag : Attribute
    {
        public string IF;
        public bool InLine;

        public ParseTag(string IF = "", bool InLine = false)
        {
            this.IF = IF;
            this.InLine = InLine;
        }
    }
}
