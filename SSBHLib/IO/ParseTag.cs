using System;

namespace SSBHLib.IO
{
    public class ParseTag : Attribute
    {
        public string IF;
        public bool InLine;
        public bool Ignore;

        public ParseTag(string IF = "", bool InLine = false, bool Ignore = false)
        {
            this.IF = IF;
            this.Ignore = Ignore;
            this.InLine = InLine;
        }
    }
}
