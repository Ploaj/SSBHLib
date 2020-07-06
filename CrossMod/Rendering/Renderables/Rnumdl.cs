using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using CrossMod.Rendering.Resources;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Linq;

namespace CrossMod.Rendering
{
    public class RNumdl : IRenderableModel
    {
        public Modl MODL;

        public Dictionary<string, Texture> sfTextureByName = new Dictionary<string, Texture>();
        public RSkeleton Skeleton;
        public RModel Model;
        public Matl Material;

        public void UpdateBinds()
        {
            if (Model != null)
            {
                foreach (RMesh m in Model.subMeshes)
                {
                    m.SingleBindIndex = Skeleton.GetBoneIndex(m.SingleBindName);
                }
            }
        }

        public void UpdateMaterials()
        {
            foreach (ModlEntry modlEntry in MODL.ModelEntries)
            {
                // Find the right material and assign it to the render meshes.
                var matlEntry = Material.Entries.Where(e => e.MaterialLabel == modlEntry.MaterialName).FirstOrDefault();
                if (matlEntry == null)
                    continue;

                Material meshMaterial = MatlToMaterial.CreateMaterial(matlEntry, DefaultTextures.Instance, sfTextureByName);
                AssignMaterialToMeshes(modlEntry, meshMaterial);
            }
        }

        private void AssignMaterialToMeshes(ModlEntry modlEntry, Material meshMaterial)
        {
            int subIndex = 0;
            string prevMesh = "";

            if (Model != null)
            {
                foreach (RMesh mesh in Model.subMeshes)
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
            if (Model != null)
            {
                Model.Render(camera, Skeleton);
            }

            // Render skeleton on top.
            if (RenderSettings.Instance.RenderBones)
                Skeleton?.Render(camera);
        }

        public RModel GetModel()
        {
            return Model;
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
