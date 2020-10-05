using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    [FileTypeAttribute(".numdlb")]
    public class NumdlNode : FileNode, IRenderableNode
    {
        private Modl model;
        private IRenderable renderableNode = null;

        public NumdlNode(string path) : base(path)
        {
            ImageKey = "model";
        }

        public IRenderable GetRenderableNode()
        {
            // Don't initialize more than once.
            // We'll assume the context isn't destroyed.
            if (renderableNode == null)
                renderableNode = CreateRenderableModel();

            if (renderableNode is RNumdl MDL)
            {
                if (MDL.Skeleton != null)
                {
                    MDL.Skeleton.Reset();
                }
            }

            return renderableNode;
        }

        private IRenderable CreateRenderableModel()
        {
            NumsbhNode meshNode = null;
            NuhlpbNode hlpbNode = null;
            RSkeleton skeleton = null;
            Matl material = null;
            XmbNode modelXmb = null;
            XmbNode lodXmb = null;

            var textureByName = new Dictionary<string, RTexture>();
            GetNodesForRendering(ref meshNode, ref hlpbNode, ref skeleton, ref material, ref modelXmb, ref lodXmb, textureByName);

            return new RNumdl(model, skeleton, material, meshNode, hlpbNode, modelXmb, lodXmb, textureByName);
        }

        private void GetNodesForRendering(ref NumsbhNode meshNode, ref NuhlpbNode hlpbNode, ref RSkeleton skeleton, ref Matl material, ref XmbNode modelXmb, ref XmbNode lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            // TODO: There's probably a cleaner way of doing this.
            foreach (FileNode fileNode in Parent.Nodes)
            {
                if (fileNode is NuhlpbNode node)
                {
                    hlpbNode = node;
                }
                else if (fileNode is NutexNode nutexNode)
                {
                    var texture = (RTexture)nutexNode.GetRenderableNode();
                    textureByName[nutexNode.TexName.ToLower()] = texture;
                }
                else if (fileNode.Text.Equals(model.MeshString))
                {
                    meshNode = (NumsbhNode)fileNode;
                }
                else if (fileNode.Text.Equals(model.SkeletonFileName))
                {
                    skeleton = (RSkeleton)((SkelNode)fileNode).GetRenderableNode();
                }
                else if (fileNode.Text.Equals(model.MaterialFileNames[0].MaterialFileName))
                {
                    material = ((MatlNode)fileNode).Material;
                }
                else if (fileNode.Text.Equals("model.xmb"))
                {
                    modelXmb = (XmbNode)fileNode;
                }
                else if (fileNode.Text.Equals("lod.xmb"))
                {
                    lodXmb = (XmbNode)fileNode;
                }
            }
        }

        public override void Open()
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out model);
        }
    }
}
