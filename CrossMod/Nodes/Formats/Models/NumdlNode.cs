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

            // TODO: Add remaining default textures.
            // The other default textures may still work, even if they aren't used in game.
            // TODO: Move these defaults to another class.
            var textureByName = new Dictionary<string, SFGraphics.GLObjects.Textures.Texture> 
            { 
                { "#replace_cubemap", Rendering.Resources.DefaultTextures.Instance.SpecularPbr },
                { "/common/shader/sfxpbs/default_normal", Rendering.Resources.DefaultTextures.Instance.DefaultNormal },
                { "/common/shader/sfxpbs/default_params", Rendering.Resources.DefaultTextures.Instance.DefaultParams },
                { "/common/shader/sfxpbs/default_black", Rendering.Resources.DefaultTextures.Instance.DefaultBlack },
                { "/common/shader/sfxpbs/default_white", Rendering.Resources.DefaultTextures.Instance.DefaultWhite },
                { "/common/shader/sfxpbs/default_color", Rendering.Resources.DefaultTextures.Instance.DefaultWhite },
                { "/common/shader/sfxpbs/fighter/default_params", Rendering.Resources.DefaultTextures.Instance.DefaultParams },
                { "/common/shader/sfxpbs/fighter/default_normal", Rendering.Resources.DefaultTextures.Instance.DefaultNormal }
            };

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
