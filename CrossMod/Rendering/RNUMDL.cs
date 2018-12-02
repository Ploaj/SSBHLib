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

                Material meshMaterial = new Material();
                foreach (MTAL_Attribute a in currentEntry.MaterialData)
                {
                    if (a.DataObject == null) continue;
                    if (a.ParamID == 0x5C)
                    {
                        if(!sfTextureByName.TryGetValue(((MTAL_Attribute.MTAL_String)a.DataObject).Text, out meshMaterial.COL))
                        {
                        }
                    }
                }

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
