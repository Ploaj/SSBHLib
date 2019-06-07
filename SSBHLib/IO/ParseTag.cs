using System;

namespace SSBHLib.IO
{
    public class ParseTag : Attribute
    {
        public bool InLine;
        public bool Ignore;

        public ParseTag(bool inLine = false, bool ignore = false)
        {
            Ignore = ignore;
            InLine = inLine;
        }
    }
}
