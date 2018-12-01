using System;
using System.Windows.Forms;

namespace CrossMod.Nodes
{
    public class IFileNode : TreeNode
    {
        public virtual void Open(string Path)
        {

        }
    }

    public class FileTypeAttribute : Attribute
    {
        public string Extension;
        public FileTypeAttribute(string Extension)
        {
            this.Extension = Extension;
        }
    }
}
