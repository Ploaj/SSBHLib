using CrossMod.Nodes;
using CrossMod.Nodes.Formats.Models;
using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using SFGraphics.Cameras;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Linq;
using XMBLib;

namespace CrossMod.Rendering
{
    public class RNumdl : IRenderableModel
    {
        public Modl Modl { get; }

        public Dictionary<string, RTexture> TextureByName { get; }

        public RSkeleton Skeleton { get; }

        public RModel RenderModel { get; }

        public Matl Material { get; }

        public Xmb ModelXmb { get; }
        public Xmb LodXmb { get; }

        public Dictionary<string, RMaterial> MaterialByName { get; set; } = new Dictionary<string, RMaterial>();

        public RNumdl(Modl modl, RSkeleton skeleton, Matl material, NumsbhNode meshNode, NuhlpbNode hlpbNode, XmbNode modelXmb, XmbNode lodXmb,
            Dictionary<string, RTexture> textureByName)
        {
            Modl = modl;
            Skeleton = skeleton;
            Material = material;
            ModelXmb = modelXmb?.Xmb;
            LodXmb = lodXmb?.Xmb;
            TextureByName = textureByName;

            if (meshNode != null)
                RenderModel = meshNode.GetRenderModel(Skeleton);
            if (Material != null)
                UpdateMaterials();
            if (Skeleton != null)
            {
                hlpbNode?.AddToRenderSkeleton(Skeleton);
                UpdateBinds();
            }
        }

        private void UpdateBinds()
        {
            if (RenderModel != null)
            {
                foreach (RMesh m in RenderModel.SubMeshes)
                {
                    m.SingleBindIndex = Skeleton.GetBoneIndex(m.SingleBindName);
                }
            }
        }

        private void UpdateMaterials()
        {
            InitializeMaterials();
            AssignMaterials();
        }

        private void AssignMaterials()
        {
            foreach (ModlEntry modlEntry in Modl.ModelEntries)
            {
                if (!MaterialByName.TryGetValue(modlEntry.MaterialLabel, out RMaterial meshMaterial))
                    continue;

                AssignMaterialToMeshes(modlEntry, meshMaterial);
            }
        }

        private void InitializeMaterials()
        {
            for (int i = 0; i < Material.Entries.Length; i++)
            {
                var entry = Material.Entries[i];
                var rMaterial = MatlToMaterial.CreateMaterial(entry, i, TextureByName);
                MaterialByName.Add(rMaterial.MaterialLabel, rMaterial);
            }
        }

        private void AssignMaterialToMeshes(ModlEntry modlEntry, RMaterial meshMaterial)
        {
            var meshes = RenderModel.SubMeshes.Where(m => m.Name == modlEntry.MeshName && m.SubIndex == modlEntry.SubIndex);
            foreach (var mesh in meshes)
            {
                mesh.Material = meshMaterial;
            }
        }

        public void Render(Camera camera)
        {
            RenderModel?.Render(camera, Skeleton);

            // Render skeleton on top.
            if (RenderSettings.Instance.RenderBones)
                Skeleton?.Render(camera);
        }
    }
}
