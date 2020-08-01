﻿using CrossMod.Nodes;
using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using CrossMod.Rendering.Resources;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using SSBHLib;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.Rendering
{
    public class RNumdl : IRenderableModel
    {

        public Modl Modl { get; }

        public Dictionary<string, RTexture> TextureByName { get; }

        public RSkeleton Skeleton { get; }

        public RModel RenderModel { get; }

        public Matl Material { get; }

        public Dictionary<string, RMaterial> MaterialByName { get; set; } = new Dictionary<string, RMaterial>();

        public RNumdl(Modl modl, RSkeleton skeleton, Matl material, NumsbhNode meshNode, NuhlpbNode hlpbNode, Dictionary<string, RTexture> textureByName)
        {
            Modl = modl;
            Skeleton = skeleton;
            Material = material;
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
            foreach (ModlEntry modlEntry in Modl.ModelEntries)
            {
                // Find the right material and assign it to the render meshes.
                if (!MaterialByName.TryGetValue(modlEntry.MaterialLabel, out RMaterial meshMaterial))
                {
                    var matlEntry = Material.Entries.Where(e => e.MaterialLabel == modlEntry.MaterialLabel).FirstOrDefault();
                    if (matlEntry == null)
                        continue;
                    meshMaterial = MatlToMaterial.CreateMaterial(matlEntry, TextureByName);
                    MaterialByName.Add(meshMaterial.MaterialLabel, meshMaterial);
                }

                AssignMaterialToMeshes(modlEntry, meshMaterial);
            }
        }

        private void AssignMaterialToMeshes(ModlEntry modlEntry, RMaterial meshMaterial)
        {
            int subIndex = 0;
            string prevMesh = "";

            if (RenderModel != null)
            {
                foreach (RMesh mesh in RenderModel.SubMeshes)
                {
                    if (prevMesh.Equals(mesh.Name))
                        subIndex++;
                    else
                        subIndex = 0;
                    prevMesh = mesh.Name;
                    if (subIndex == modlEntry.SubIndex && mesh.Name.Equals(modlEntry.MeshName))
                    {
                        mesh.Material = meshMaterial;
                        break;
                    }
                }
            }
        }

        public void Render(Camera camera)
        {
            if (RenderModel != null)
            {
                RenderModel.Render(camera, Skeleton);
            }

            // Render skeleton on top.
            if (RenderSettings.Instance.RenderBones)
                Skeleton?.Render(camera);
        }

        public RModel GetModel()
        {
            return RenderModel;
        }

        public RSkeleton GetSkeleton()
        {
            return Skeleton;
        }

        public RTexture[] GetTextures()
        {
            return null;
        }
    }
}
