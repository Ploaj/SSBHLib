using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats.Meshes;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using SSBHLib.Tools;
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
            SelectedImageKey = "model";
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

            var textureByName = new Dictionary<string, SFGraphics.GLObjects.Textures.Texture>();
            GetNodesForRendering(ref meshNode, ref hlpbNode, ref skeleton, ref material, textureByName);

            return new RNumdl(model, skeleton, material, meshNode, hlpbNode, textureByName);
        }

        private void GetNodesForRendering(ref NumsbhNode meshNode, ref NuhlpbNode hlpbNode, ref RSkeleton skeleton, ref Matl material, Dictionary<string, SFGraphics.GLObjects.Textures.Texture> textureByName)
        {
            foreach (FileNode fileNode in Parent.Nodes)
            {
                if (fileNode is NuhlpbNode node)
                {
                    hlpbNode = node;
                }
                else if (fileNode is NutexNode nutexNode)
                {
                    var texture = (RTexture)nutexNode.GetRenderableNode();
                    textureByName[nutexNode.TexName.ToLower()] = texture.RenderTexture;
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
            }
        }

        public override void Open()
        {
            if (Ssbh.TryParseSsbhFile(AbsolutePath, out SsbhFile ssbhFile))
            {
                if (ssbhFile is Modl modl)
                {
                    model = modl;
                }
            }
        }
    }
}
