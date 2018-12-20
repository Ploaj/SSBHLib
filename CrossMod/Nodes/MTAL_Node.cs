﻿using SSBHLib;
using SSBHLib.Formats.Materials;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numatb")]
    public class MTAL_Node : FileNode
    {
        public MTAL Material { get; set; }
        
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
                    Material = (MTAL)ssbhFile;
                }
            }
        }
    }
}
