using CrossMod.Nodes;
using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using OpenTK;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using System.Collections.Generic;
using System.Linq;
using XMBLib;

namespace CrossMod.Rendering
{
    // TODO: Find a way to remove this class by converting to static methods.
    // This is replaced by ModelCollection.
    public class RNumdl : IRenderableModel
    {
        public Modl? Modl { get; }

        public Dictionary<string, RTexture> TextureByName { get; }

        public RSkeleton? Skeleton { get; }

        public RModel? RenderModel { get; }

        // TODO: Why are these saved as is?
        public Matl? Matl { get; }
        public Xmb? ModelXmb { get; }
        public Xmb? LodXmb { get; }

        public Mesh? Mesh { get; }

        public Dictionary<string, RMaterial> MaterialByName { get; set; } = new Dictionary<string, RMaterial>();

        public RNumdl(Modl? modl, RSkeleton? skeleton, Matl? matl, NumshbNode? meshNode, NuhlpbNode? hlpbNode, XmbNode? modelXmb, XmbNode? lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            Modl = modl;
            Skeleton = skeleton;
            Matl = matl;
            ModelXmb = modelXmb?.Xmb;
            LodXmb = lodXmb?.Xmb;
            Mesh = meshNode?.mesh;
            TextureByName = textureByName;

            if (meshNode != null)
                RenderModel = meshNode.GetRenderModel(Skeleton);

            UpdateMaterials(matl, this);
            if (Skeleton != null)
            {
                hlpbNode?.AddToRenderSkeleton(Skeleton);
            }
        }

        public static void UpdateMaterials(Matl? matl, RNumdl rNumdl)
        {
            rNumdl.MaterialByName = InitializeMaterials(matl, rNumdl.TextureByName);
            if (rNumdl.Modl != null)
                AssignMaterials(rNumdl.RenderModel, rNumdl.Modl, rNumdl.MaterialByName);
        }

        private static void AssignMaterials(RModel? renderModel, Modl modl, Dictionary<string, RMaterial> materialByName)
        {
            // Match materials based on the Modl.
            foreach (ModlEntry modlEntry in modl.ModelEntries)
            {
                if (!materialByName.TryGetValue(modlEntry.MaterialLabel, out RMaterial? meshMaterial))
                    continue;

                AssignMaterialToMeshes(renderModel, modlEntry, meshMaterial);
            }

            // Fix any potentially unassigned materials.
            // TODO: Display some sort of error color in the viewport?
            if (renderModel != null)
            {
                foreach (var mesh in renderModel.SubMeshes)
                {
                    if (mesh.Material == null)
                        mesh.Material = new RMaterial("", "", 0);
                }
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
                materialByName.Add(rMaterial.MaterialLabel, rMaterial);
            }

            return materialByName;
        }

        private static void AssignMaterialToMeshes(RModel? renderModel, ModlEntry modlEntry, RMaterial meshMaterial)
        {
            if (renderModel == null)
                return;

            var meshes = renderModel.SubMeshes.Where(m => m.Name == modlEntry.MeshName && m.SubIndex == modlEntry.SubIndex);
            foreach (var mesh in meshes)
            {
                mesh.Material = meshMaterial;
            }
        }

        public void Render(Matrix4 modelView, Matrix4 projection)
        {
            RenderModel?.Render(modelView, projection, Skeleton);

            // Render skeleton on top.
            if (RenderSettings.Instance.RenderBones)
                Skeleton?.Render(modelView, projection);
        }
    }
}
