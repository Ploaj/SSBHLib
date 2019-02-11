using System;
using paracobNET;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.Nodes
{
    [FileType(".prc")]
    public class ParamNode : FileNode
    {
        public ParamFile Param { get; set; }

        public ParamNode(string path) : base(path)
        {
            Param = new ParamFile(AbsolutePath);
            ParamNodeContainer.AddFile(this);
        }
    }
}
