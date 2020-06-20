using SSBHLib;
using SSBHLib.Formats.Materials;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numatb")]
    public class MatlNode : FileNode
    {
        public Matl Material { get; set; }
        
        public MatlNode(string path) : base(path)
        {
            ImageKey = "material";
            SelectedImageKey = "material";
        }

        public override void Open()
        {
            if (Ssbh.TryParseSsbhFile(AbsolutePath, out SsbhFile ssbhFile))
            {
                if (ssbhFile is Matl)
                {
                    Material = (Matl)ssbhFile;
                }
            }
        }
    }
}
