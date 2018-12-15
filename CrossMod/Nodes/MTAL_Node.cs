using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib;
using SSBHLib.Formats;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numatb")]
    public class MTAL_Node : FileNode
    {
        public MTAL _material;
        
        public MTAL_Node()
        {
            ImageKey = "material";
            SelectedImageKey = "material";
        }

        public override void Open(string Path)
        {
            if (SSBH.TryParseSSBHFile(Path, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MTAL)
                {
                    _material = (MTAL)ssbhFile;
                }
            }
        }
    }
}
