using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RNUMDL : IRenderable
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
            EmiMap = 0x61
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
            foreach(RMesh m in Model.Mesh)
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
                foreach (RMesh mesh in Model.Mesh)
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
                    // Use red to differentiate with no value (black).
                    ulong value = (ulong)a.DataObject;
                    meshMaterial.vec4ByParamId[a.ParamID] = new MTAL_Attribute.MTAL_Vector4() { X = value, Y = 0, Z = 0, W = 0 };
                }
            }

            return meshMaterial;
        }

        private void SetTextureParameter(Material meshMaterial, MTAL_Attribute a)
        {
            var text = ((MTAL_Attribute.MTAL_String)a.DataObject).Text.ToLower();
            if (colParamIds.Contains(a.ParamID))
                sfTextureByName.TryGetValue(text, out meshMaterial.col);
            else if (a.ParamID == (long)ParamId.NorMap)
                sfTextureByName.TryGetValue(text, out meshMaterial.nor);
            else if (a.ParamID == (long)ParamId.PrmMap)
                sfTextureByName.TryGetValue(text, out meshMaterial.prm);
            else if (a.ParamID == (long)ParamId.EmiMap)
                sfTextureByName.TryGetValue(text, out meshMaterial.emi);
        }

        public void Render(Camera Camera)
        {
            if (Model != null)
            {
                Model.Render(Camera, Skeleton);
            }

            //Render Skeleton with no depth buffer
        }
    }
}
