using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib;
using SSBHLib.Formats;
using CrossMod.Rendering;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NUMDL_Node : FileNode, IRenderableNode
    {
        private MODL _model;

        public NUMDL_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }
        public IRenderable GetRenderableNode()
        {
            RNUMDL Model = new RNUMDL();
            foreach (FileNode n in Parent.Nodes)
            {
                if (n is NUTEX_Node)
                {
                    Model.TextureBank.Add(((NUTEX_Node)n).Text, (RTexture)((NUTEX_Node)n).GetRenderableNode());
                }
                if (n.Text.Equals(_model.MeshString))
                {
                    Model.Model = (RModel)((NUMSHB_Node)n).GetRenderableNode();
                }
                if (n.Text.Equals(_model.SkeletonFileName))
                {
                    Model.Skeleton = (RSkeleton)((SKEL_Node)n).GetRenderableNode();
                }
                // TODO: Materials
                if (n.Text.Equals(_model.MaterialFileNames[0].MaterialFileName))
                {

                }
            }
            return Model;
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
