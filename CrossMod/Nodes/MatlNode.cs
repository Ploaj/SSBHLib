using SSBHLib;
using SSBHLib.Formats.Materials;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numatb")]
    public class MatlNode : FileNode
    {
        public MATL Material { get; set; }
        
        public MatlNode(string path) : base(path)
        {
            ImageKey = "material";
            SelectedImageKey = "material";
        }

        public override void Open()
        {
            if (SSBH.TryParseSSBHFile(AbsolutePath, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MATL)
                {
                    Material = (MATL)ssbhFile;
                }
            }
        }
    }
}
