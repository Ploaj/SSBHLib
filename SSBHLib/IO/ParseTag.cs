using System;

namespace SSBHLib.IO
{
    public class ParseTag : Attribute
    {
        public string IF;
        public bool InLine;
        public bool Ignore;

        public ParseTag(string IF = "", bool inLine = false, bool ignore = false)
        {
            this.IF = IF;
            Ignore = ignore;
            InLine = inLine;
        }
    }
}
