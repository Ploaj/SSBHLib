using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSBHLib;
using SSBHLib.Formats;
using CrossMod.Rendering;
using CrossMod.IO;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NUMDL_Node : FileNode, IRenderableNode, IExportableModelNode
    {
        private MODL _model;

        public NUMDL_Node()
        {
            ImageKey = "model";
            SelectedImageKey = "model";
        }

        public IRenderable GetRenderableNode()
        {
            RNUMDL Model = new RNUMDL
            {
                MODL = _model
            };

            foreach (FileNode fileNode in Parent.Nodes)
            {
                if (fileNode is NUTEX_Node)
                {
                    Model.sfTextureByName.Add(((NUTEX_Node)fileNode).TexName.ToLower(), ((RTexture)((NUTEX_Node)fileNode).GetRenderableNode()).renderTexture);
                }
                if (fileNode.Text.Equals(_model.MeshString))
                {
                    Model.Model = (RModel)((NUMSHB_Node)fileNode).GetRenderableNode();
                }
                if (fileNode.Text.Equals(_model.SkeletonFileName))
                {
                    Model.Skeleton = (RSkeleton)((SKEL_Node)fileNode).GetRenderableNode();
                }
                if (fileNode.Text.Equals(_model.MaterialFileNames[0].MaterialFileName))
                {
                    Model.Material = ((MTAL_Node)fileNode)._material;
                }
            }
            if (Model.Material != null)
                Model.UpdateMaterial();
            if (Model.Skeleton != null)
                Model.UpdateBinds();
            return Model;
        }

        public override void Open(string Path)
        {
            if (SSBH.TryParseSSBHFile(Path, out ISSBH_File ssbhFile))
            {
                if (ssbhFile is MODL)
                {
                    _model = (MODL)ssbhFile;
                }
            }
        }

        
        public IOModel GetIOModel()
        {
            return new IOModel();
        }
    }
}
