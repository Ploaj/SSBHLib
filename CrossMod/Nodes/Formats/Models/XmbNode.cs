using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.Nodes.Formats.Models
{
    [FileTypeAttribute(".xmb")]
    public class XmbNode : FileNode
    {
        public XMBLib.Xmb Xmb { get; set; }

        public XmbNode(string absolutePath) : base(absolutePath)
        {

        }

        public override void Open()
        {
            Xmb = new XMBLib.Xmb(AbsolutePath);
        }
    }
}
