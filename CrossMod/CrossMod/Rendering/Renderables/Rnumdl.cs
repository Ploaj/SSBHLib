using CrossMod.Nodes;
using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using OpenTK;
using SFGraphics.Cameras;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using System.Collections.Generic;
using System.Linq;
using XMBLib;

namespace CrossMod.Rendering
{
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

            UpdateMaterials(matl);
            if (Skeleton != null)
            {
                hlpbNode?.AddToRenderSkeleton(Skeleton);
            }
        }

        public void UpdateMaterials(Matl? matl)
        {
            InitializeMaterials(matl);
            AssignMaterials();
        }

        private void AssignMaterials()
        {
            // Match materials based on the Modl.
            foreach (ModlEntry modlEntry in Modl.ModelEntries)
            {
                if (!MaterialByName.TryGetValue(modlEntry.MaterialLabel, out RMaterial? meshMaterial))
                    continue;

                AssignMaterialToMeshes(modlEntry, meshMaterial);
            }

            // Fix any potentially unassigned materials.
            // TODO: Display some sort of error color in the viewport?
            if (RenderModel != null)
            {
                foreach (var mesh in RenderModel.SubMeshes)
                {
                    if (mesh.Material == null)
                        mesh.Material = new RMaterial("", "", 0);
                }
            }
        }

        private void InitializeMaterials(Matl? matl)
        {
            if (matl == null)
                return;

            MaterialByName.Clear();
            for (int i = 0; i < matl.Entries.Length; i++)
            {
                var entry = matl.Entries[i];
                var rMaterial = MatlToMaterial.CreateMaterial(entry, i, TextureByName);
                MaterialByName.Add(rMaterial.MaterialLabel, rMaterial);
            }
        }

        private void AssignMaterialToMeshes(ModlEntry modlEntry, RMaterial meshMaterial)
        {
            if (RenderModel == null)
                return;

            var meshes = RenderModel.SubMeshes.Where(m => m.Name == modlEntry.MeshName && m.SubIndex == modlEntry.SubIndex);
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
