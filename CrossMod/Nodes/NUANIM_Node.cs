using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".nuanmb")]
    public class NUANIM_Node : FileNode, IRenderableNode
    {
        private ANIM animation;

        public NUANIM_Node()
        {
            ImageKey = "animation";
            SelectedImageKey = "animation";
        }
        
        public override void Open(string Path)
        {
            ISSBH_File SSBHFile;
            if (SSBH.TryParseSSBHFile(Path, out SSBHFile))
            {
                if (SSBHFile is ANIM anim)
                {
                    animation = anim;
                }
            }
        }

        public IRenderable GetRenderableNode()
        {
            return null;
        }
    }
}
