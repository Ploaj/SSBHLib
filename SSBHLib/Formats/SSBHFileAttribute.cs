using System;

namespace SSBHLib.Formats
{
    public class SSBHFileAttribute : Attribute
    {
        public string Magic { get; set; }

        public SSBHFileAttribute(string magic)
        {
            Magic = magic;
        }
    }
}
