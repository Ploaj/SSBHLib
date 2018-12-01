using System;

namespace SSBHLib.Formats
{
    public class SSBHFileAttribute : Attribute
    {
        public string Magic { get; set; }

        public SSBHFileAttribute(string Magic)
        {
            this.Magic = Magic;
        }
    }
}
