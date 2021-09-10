using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    public class NumdlbNode : FileNode, IRenderableNode
    {
        public Lazy<IRenderable> Renderable { get; }

        private readonly Modl model;

        public NumdlbNode(string path) : base(path, "model", true)
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out model);
            Renderable = new Lazy<IRenderable>(() => GetRenderableNode());
        }

        public RNumdl GetRenderableNode()
        {
            var rnumdl = CreateRnumdl();
            rnumdl.Skeleton?.Reset();

            return rnumdl;
        }

        private RNumdl CreateRnumdl()
        {
            NumshbNode? meshNode = null;
            NuhlpbNode? hlpbNode = null;
            RSkeleton? skeleton = null;
            Matl? material = null;
            XmbNode? modelXmb = null;
            XmbNode? lodXmb = null;

            var textureByName = new Dictionary<string, RTexture>();
            GetNodesForRendering(ref meshNode, ref hlpbNode, ref skeleton, ref material, ref modelXmb, ref lodXmb, textureByName);

            // TODO: Handle nulls.
            return new RNumdl(model, skeleton, material, meshNode, hlpbNode, modelXmb, lodXmb, textureByName);
        }

        private void GetNodesForRendering(ref NumshbNode? meshNode, ref NuhlpbNode? hlpbNode, ref RSkeleton? skeleton, ref Matl? material, ref XmbNode? modelXmb, ref XmbNode? lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            // TODO: There's probably a cleaner way of doing this.
            foreach (FileNode fileNode in Parent.Nodes)
            {
                if (fileNode is NuhlpbNode node)
                {
                    hlpbNode = node;
                }
                else if (fileNode is NutexbNode nutexNode)
                {
                    var texture = (RTexture)nutexNode.Renderable.Value;

                    // Use the file name instead of the internal name.
                    // Ignore case.
                    var textureName = System.IO.Path.GetFileNameWithoutExtension(fileNode.Text).ToLower();
                    textureByName[textureName] = texture;
                }
                else if (fileNode.Text.Equals(model.MeshString))
                {
                    meshNode = (NumshbNode)fileNode;
                }
                else if (fileNode.Text.Equals(model.SkeletonFileName))
                {
                    skeleton = (RSkeleton)((NusktbNode)fileNode).Renderable.Value;
                }
                else if (fileNode.Text.Equals(model.MaterialFileNames[0].MaterialFileName))
                {
                    material = ((NumatbNode)fileNode).Material;
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
    }
}
