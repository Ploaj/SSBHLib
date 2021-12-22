using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering;
using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.Nodes
{
    // TODO: Rework this class to just handle Modl files?
    public class NumdlbNode : FileNode
    {
        public readonly Modl modl;

        public NumdlbNode(string path) : base(path, "model", true)
        {
            Ssbh.TryParseSsbhFile(AbsolutePath, out modl);
        }

        // TODO: These functions should be part of a separate class.
        public (RModel?, RSkeleton?, Dictionary<string, RTexture>) GetModelSkeletonTextures()
        {
            NumshbNode? meshNode = null;
            RSkeleton? skeleton = null;
            Matl? matl = null;
            XmbNode? modelXmb = null;
            XmbNode? lodXmb = null;

            var textureByName = new Dictionary<string, RTexture>();
            GetNodesForRendering(modl, Parent, ref meshNode, ref skeleton, ref matl, ref modelXmb, ref lodXmb, textureByName);

            return GetModelSkeletonTextures(modl, skeleton, matl, meshNode, modelXmb, lodXmb, textureByName);
        }

        private static void GetNodesForRendering(Modl modl, FileNode? directory, ref NumshbNode? meshNode, ref RSkeleton? skeleton, ref Matl? material, ref XmbNode? modelXmb, ref XmbNode? lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            // TODO: There's probably a cleaner way of doing this.
            if (directory == null)
                return;

            // Find rendering related files in the given directory.
            foreach (FileNode fileNode in directory.Nodes)
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

        private static (RModel?, RSkeleton?, Dictionary<string, RTexture>) GetModelSkeletonTextures(Modl? modl, RSkeleton? skeleton, Matl? matl, NumshbNode? meshNode, XmbNode? modelXmb, XmbNode? lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            var renderModel = meshNode?.GetRenderModel(skeleton);

            InitializeAndAssignMaterials(renderModel?.SubMeshes ?? new List<RMesh>(), matl, textureByName, modl);

            return (renderModel, skeleton, textureByName);
        }

        public static Dictionary<string, RMaterial> InitializeAndAssignMaterials(IEnumerable<RMesh> meshes, Matl? matl, Dictionary<string, RTexture> textureByName, Modl? modl)
        {
            var materialByName = InitializeMaterials(matl, textureByName);
            if (modl != null)
                AssignMaterials(meshes, modl, materialByName);

            return materialByName;
        }

        private static void AssignMaterials(IEnumerable<RMesh> meshes, Modl modl, Dictionary<string, RMaterial> materialByName)
        {
            // Match materials based on the Modl.
            foreach (ModlEntry modlEntry in modl.ModelEntries)
            {
                if (!materialByName.TryGetValue(modlEntry.MaterialLabel, out RMaterial? meshMaterial))
                    continue;

                AssignMaterialToMeshes(meshes, modlEntry, meshMaterial);
            }

            // Fix any potentially unassigned materials.
            // TODO: Display some sort of error color in the viewport?
            foreach (var mesh in meshes)
            {
                if (mesh.Material == null)
                    mesh.Material = new RMaterial("", "", 0);
            }
        }

        private static Dictionary<string, RMaterial> InitializeMaterials(Matl? matl, Dictionary<string, RTexture> textureByName)
        {
            if (matl == null)
                return new Dictionary<string, RMaterial>();

            var materialByName = new Dictionary<string, RMaterial>();
            for (int i = 0; i < matl.Entries.Length; i++)
            {
                var entry = matl.Entries[i];
                var rMaterial = MatlToMaterial.CreateMaterial(entry, i, textureByName);
                // There may be duplicate keys, so just keep the most recent material.
                materialByName[rMaterial.MaterialLabel] = rMaterial;
            }

            return materialByName;
        }

        private static void AssignMaterialToMeshes(IEnumerable<RMesh> meshes, ModlEntry modlEntry, RMaterial meshMaterial)
        {
            foreach (var mesh in meshes.Where(m => m.Name == modlEntry.MeshName && m.SubIndex == modlEntry.SubIndex))
            {
                mesh.Material = meshMaterial;
            }
        }
    }
}
