using System;

namespace CrossMod.Nodes
{
    public class FileTypeAttribute : Attribute
    {
        public string Extension;
        public FileTypeAttribute(string Extension)
        {
            this.Extension = Extension;
        }
    }
}
