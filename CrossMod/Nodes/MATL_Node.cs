using SSBHLib;
using SSBHLib.Formats.Materials;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numatb")]
    public class MATL_Node : FileNode
    {
        public MATL Material { get; set; }
        
        public MATL_Node(string path) : base(path)
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
