using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Nodes
{
    // TODO: Rework this class to just handle Modl files?
    public class NumdlbNode : FileNode
    {
        private readonly Modl modl;

        public NumdlbNode(string path) : base(path, "model", true)
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out modl);
        }

        public (RModel?, RSkeleton?) GetModelAndSkeleton()
        {
            NumshbNode? meshNode = null;
            RSkeleton? skeleton = null;
            Matl? matl = null;
            XmbNode? modelXmb = null;
            XmbNode? lodXmb = null;

            var textureByName = new Dictionary<string, RTexture>();
            GetNodesForRendering(modl, Parent, ref meshNode, ref skeleton, ref matl, ref modelXmb, ref lodXmb, textureByName);

            return RNumdl.GetModelAndSkeleton(modl, skeleton, matl, meshNode, modelXmb, lodXmb, textureByName);
        }

        private static void GetNodesForRendering(Modl modl, FileNode? parent, ref NumshbNode? meshNode, ref RSkeleton? skeleton, ref Matl? material, ref XmbNode? modelXmb, ref XmbNode? lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            // TODO: There's probably a cleaner way of doing this.
            if (parent == null)
                return;

            foreach (FileNode fileNode in parent.Nodes)
            {
                if (fileNode is NutexbNode nutexNode)
                {
                    var texture = (RTexture)nutexNode.Renderable.Value;

                    // Use the file name instead of the internal name.
                    // Ignore case.
                    var textureName = System.IO.Path.GetFileNameWithoutExtension(fileNode.Text).ToLower();
                    textureByName[textureName] = texture;
                }
                else if (fileNode.Text.Equals(modl.MeshString))
                {
                    meshNode = (NumshbNode)fileNode;
                }
                else if (fileNode.Text.Equals(modl.SkeletonFileName))
                {
                    skeleton = (RSkeleton)((NusktbNode)fileNode).Renderable.Value;
                }
                else if (fileNode.Text.Equals(modl.MaterialFileNames[0].MaterialFileName))
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
