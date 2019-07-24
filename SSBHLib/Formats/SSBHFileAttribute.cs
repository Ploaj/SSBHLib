using System;

namespace SSBHLib.Formats
{
    public class SsbhFileAttribute : Attribute
    {
        public string Magic { get; set; }

        public SsbhFileAttribute(string magic)
        {
            Magic = magic;
        }
    }
}
