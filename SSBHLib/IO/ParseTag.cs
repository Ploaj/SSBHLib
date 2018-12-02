using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSBHLib.IO
{
    public class ParseTag : Attribute
    {
        public string IF;

        public ParseTag(string IF = "")
        {
            this.IF = IF;
        }
    }
}
