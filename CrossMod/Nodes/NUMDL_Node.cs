using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib;
using SSBHLib.Formats;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NUMDL_Node : FileNode
    {
        private MODL _model;

        public NUMDL_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public override void Open(string Path)
        {
            ISSBH_File SSBHFile;
            if (SSBH.TryParseSSBHFile(Path, out SSBHFile))
            {
                if (SSBHFile is MODL)
                {
                    _model = (MODL)SSBHFile;
                }
            }
        }
    }
}
