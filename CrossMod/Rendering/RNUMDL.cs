using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RNUMDL : IRenderableModel
    {
        public MODL MODL;

        public Dictionary<string, Texture> sfTextureByName = new Dictionary<string, Texture>();
        public RSkeleton Skeleton;
        public RModel Model;
        public MTAL Material;

        public enum ParamId
        {
            PrmMap = 0x62,
            NorMap = 0x60,
            ColMap = 0x5C,
            GaoMap = 0x5F,
            ColMap2 = 0x5D,
            EmiMap = 0x61,
            BakeLitMap = 0x65
        }

        public enum ParamDataType
        {
            Texture = 0xB,
            Vector4 = 0x5,
            Boolean = 0x2
        }

        private HashSet<long> colParamIds = new HashSet<long>()
        {
            0x5C, 0x5D, 0x64, 0x66, 0x67, 0x68, 0x69, 0x6A 
        };

        public void UpdateBinds()
        {
            if(Model!=null)
            foreach(RMesh m in Model.subMeshes)
            {
                m.SingleBindIndex = Skeleton.GetBoneIndex(m.SingleBindName);
            }
        }

        public void UpdateMaterial()
        {
            foreach (MODL_Entry e in MODL.ModelEntries)
            {
                MTAL_Entry currentEntry = null;
                foreach (MTAL_Entry entry in Material.MaterialEntries)
                {
                    if (entry.MaterialLabel.Equals(e.MaterialName))
                    {
                        currentEntry = entry;
                        break;
                    }
                }
                if (currentEntry == null)
                    continue;

                Material meshMaterial = GetMaterial(currentEntry);

                int subindex = 0;
                string prevMesh = "";
                if(Model != null)
                foreach (RMesh mesh in Model.subMeshes)
                {
                    if (prevMesh.Equals(mesh.Name))
                        subindex++;
                    else
                        subindex = 0;
                    prevMesh = mesh.Name;
                    if (subindex == e.SubIndex && mesh.Name.Equals(e.MeshName))
                    {
                        mesh.Material = meshMaterial;
                        break;
                    }
                }
            }
        }

        private Material GetMaterial(MTAL_Entry currentEntry)
        {
            System.Diagnostics.Debug.WriteLine("Material Attributes:");
            Material meshMaterial = new Material();

            foreach (MTAL_Attribute a in currentEntry.MaterialData)
            {
                if (a.DataObject == null)
                    continue;

                System.Diagnostics.Debug.WriteLine($"{a.DataType.ToString("X")} {a.ParamID.ToString("X")} {a.DataObject.ToString()}");
                if (a.DataType == (long)ParamDataType.Texture)
                {
                    SetTextureParameter(meshMaterial, a);
                }
                else if (a.DataType == (long)ParamDataType.Vector4)
                {
                    meshMaterial.vec4ByParamId[a.ParamID] = (MTAL_Attribute.MTAL_Vector4)a.DataObject;
                }
                else if (a.DataType == (long)ParamDataType.Boolean)
                {
                    // Convert to vec4 to use with rendering.
                    // Use cyan to differentiate with no value (blue).
                    ulong value = (ulong)a.DataObject;
                    meshMaterial.vec4ByParamId[a.ParamID] = new MTAL_Attribute.MTAL_Vector4() { X = value, Y = 0, Z = 1, W = 0 };
                }
            }

            return meshMaterial;
        }

        private void SetTextureParameter(Material meshMaterial, MTAL_Attribute a)
        {
            // Don't make texture names case sensitive.
            var text = ((MTAL_Attribute.MTAL_String)a.DataObject).Text.ToLower();

            switch (a.ParamID)
            {
                case (long)ParamId.ColMap:
                    sfTextureByName.TryGetValue(text, out meshMaterial.col);
                    break;
                case (long)ParamId.GaoMap:
                    sfTextureByName.TryGetValue(text, out meshMaterial.gao);
                    break;
                case (long)ParamId.ColMap2:
                    sfTextureByName.TryGetValue(text, out meshMaterial.col2);
                    break;
                case (long)ParamId.NorMap:
                    sfTextureByName.TryGetValue(text, out meshMaterial.nor);
                    break;
                case (long)ParamId.PrmMap:
                    sfTextureByName.TryGetValue(text, out meshMaterial.prm);
                    break;
                case (long)ParamId.EmiMap:
                    sfTextureByName.TryGetValue(text, out meshMaterial.emi);
                    break;
                case (long)ParamId.BakeLitMap:
                    sfTextureByName.TryGetValue(text, out meshMaterial.bakeLit);
                    break;
            }

        }

        public void Render(Camera Camera)
        {
            if (Model != null)
            {
                Model.Render(Camera, Skeleton);
            }

            // Render skeleton on top.
            if (RenderSettings.Instance.renderBones)
                Skeleton?.Render(Camera);
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
