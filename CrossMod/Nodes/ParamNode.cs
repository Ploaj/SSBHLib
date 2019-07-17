using paracobNET;

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
