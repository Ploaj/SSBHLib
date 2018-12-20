using SSBHLib;
using SSBHLib.Formats.Materials;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numatb")]
    public class MATL_Node : FileNode
    {
        public MATL Material { get; set; }
        
        public MATL_Node()
        {
            ImageKey = "material";
            SelectedImageKey = "material";
        }

        public override void Open(string Path)
        {
            if (SSBH.TryParseSSBHFile(Path, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MATL)
                {
                    Material = (MATL)ssbhFile;
                }
            }
        }
    }
}
