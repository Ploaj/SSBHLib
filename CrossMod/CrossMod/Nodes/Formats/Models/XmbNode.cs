namespace CrossMod.Nodes.Formats.Models
{
    public class XmbNode : FileNode
    {
        public XMBLib.Xmb Xmb { get; set; }

        public XmbNode(string absolutePath) : base(absolutePath, "", false)
        {
            Open();
        }

        private void Open()
        {
            Xmb = new XMBLib.Xmb(AbsolutePath);
        }
    }
}
