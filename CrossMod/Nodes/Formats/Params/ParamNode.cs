using paracobNET;

namespace CrossMod.Nodes
{
    public class ParamNode : FileNode
    {
        public ParamFile Param { get; set; }

        public ParamNode(string path) : base(path, "", false)
        {
            try
            {
                // TODO: This lacks necessary error checking.
                Param = new ParamFile(AbsolutePath);
                ParamNodeContainer.AddFile(this);
            }
            catch (System.Exception)
            {

            }
        }
    }
}
