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
