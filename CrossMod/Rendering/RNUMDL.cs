using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.Cameras;
using SSBHLib.Formats;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public class RNUMDL : IRenderable
    {
        public MODL MODL;

        public Dictionary<string, Texture> TextureBank = new Dictionary<string, Texture>();
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
                MTAL_Entry Entry = null;
                foreach (MTAL_Entry entry in Material.MaterialEntries)
                {
                    if (entry.MaterialLabel.Equals(e.MaterialName))
                    {
                        Entry = entry;
                        break;
                    }
                }
                if (Entry == null) continue;
                Material MeshMaterial = new Rendering.Material();
                foreach (MTAL_Attribute a in Entry.MaterialData)
                {
                    if (a.DataObject == null) continue;
                    if (a.ParamID == 0x5C)
                    {
                        if(!TextureBank.TryGetValue(((MTAL_Attribute.MTAL_String)a.DataObject).Text, out MeshMaterial.COL))
                        {
                        }
                    }
                }
                int subindex = 0;
                string PrevMesh = "";
                foreach (RMesh mesh in Model.Mesh)
                {
                    if (PrevMesh.Equals(mesh.Name))
                        subindex++;
                    else
                        subindex = 0;
                    PrevMesh = mesh.Name;
                    if (subindex == e.SubIndex && mesh.Name.Equals(e.MeshName))
                    {
                        mesh.Material = MeshMaterial;
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
