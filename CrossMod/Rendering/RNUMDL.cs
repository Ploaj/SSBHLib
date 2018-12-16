using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats;
using System.Collections.Generic;
using CrossMod.Rendering.Models;

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
            BakeLitMap = 0x65,
            ColSampler = 0x6C,
            NorSampler = 0x70,
            PrmSampler = 0x72,
            EmiSampler = 0x71
        }

        public enum ParamDataType
        {
            Texture = 0xB,
            Vector4 = 0x5,
            Boolean = 0x2,
            Float = 0x1,
            SamplerInfo = 0xE
        }

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

                System.Diagnostics.Debug.WriteLine(e.MeshName);
                Material meshMaterial = GetMaterial(currentEntry);
                System.Diagnostics.Debug.WriteLine("");

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
            // HACK: This is pretty gross.
            // I need to rework the entire texture loading system.
            if (RMesh.defaultTextures == null)
                RMesh.defaultTextures = new Resources.DefaultTextures();

            Material meshMaterial = new Material(RMesh.defaultTextures);

            System.Diagnostics.Debug.WriteLine("Material Attributes:");
            foreach (MTAL_Attribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null)
                    continue;

                System.Diagnostics.Debug.WriteLine($"{a.DataType.ToString("X")} {a.ParamID.ToString("X")} {a.DataObject.ToString()}");

                switch (a.DataType)
                {
                    case (long)ParamDataType.Texture:
                        SetTextureParameter(meshMaterial, a);
                        break;
                    case (long)ParamDataType.Vector4:
                        var vec4 = (MTAL_Attribute.MTAL_Vector4)a.DataObject; 
                        meshMaterial.vec4ByParamId[a.ParamID] = new OpenTK.Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case (long)ParamDataType.Boolean:
                        // Convert to vec4 to use with rendering.
                        // Use cyan to differentiate with no value (blue).
                        ulong boolValue = (ulong)a.DataObject;
                        meshMaterial.boolByParamId[a.ParamID] = boolValue == 1;
                        break;
                    case (long)ParamDataType.Float:
                        float floatValue = (float)a.DataObject;
                        meshMaterial.floatByParamId[a.ParamID] = floatValue;
                        break;
                }
            }

            // HACK: Textures need to be initialized first before we can modify their state.
            foreach (MTAL_Attribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null || a.DataType != 0xE)
                    continue;

                SetSamplerInformation(meshMaterial, a);
            }

            return meshMaterial;
        }

        private void SetSamplerInformation(Material material, MTAL_Attribute a)
        {
            // TODO: Set the appropriate sampler information based on the attribute and param id.
            var samplerStruct = (MTAL_Attribute.MTAL_Unk_0E)a.DataObject;
            var wrapS = GetWrapMode(samplerStruct.WrapS);
            var wrapT = GetWrapMode(samplerStruct.WrapT);

            switch (a.ParamID)
            {
                case (long)ParamId.ColSampler:
                    material.col.TextureWrapS = wrapS;
                    material.col.TextureWrapT = wrapT;
                    break;
                case (long)ParamId.NorSampler:
                    material.nor.TextureWrapS = wrapS;
                    material.nor.TextureWrapT = wrapT;
                    break;
                case (long)ParamId.PrmSampler:
                    material.prm.TextureWrapS = wrapS;
                    material.prm.TextureWrapT = wrapT;
                    break;
                case (long)ParamId.EmiSampler:
                    material.emi.TextureWrapS = wrapS;
                    material.emi.TextureWrapT = wrapT;
                    break;
            }
        }

        private OpenTK.Graphics.OpenGL.TextureWrapMode GetWrapMode(int wrapMode)
        {
            if (wrapMode == 0)
                return OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
            else if (wrapMode == 2)
                return OpenTK.Graphics.OpenGL.TextureWrapMode.MirroredRepeat;
            else
                return OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToEdge;
        }

        private void SetTextureParameter(Material meshMaterial, MTAL_Attribute a)
        {
            // Don't make texture names case sensitive.
            var text = ((MTAL_Attribute.MTAL_String)a.DataObject).Text.ToLower();

            // Create a temp so we don't make the defaults null.
            Texture texture = null;
            switch (a.ParamID)
            {
                case (long)ParamId.ColMap:
                    if (sfTextureByName.TryGetValue(text, out texture))
                        meshMaterial.col = texture;
                    break;
                case (long)ParamId.GaoMap:
                    if (sfTextureByName.TryGetValue(text, out texture))
                        meshMaterial.gao = texture;
                    break;
                case (long)ParamId.ColMap2:
                    if (sfTextureByName.TryGetValue(text, out texture))
                    {
                        meshMaterial.col2 = texture;
                        meshMaterial.HasCol2 = true;
                    }
                    break;
                case (long)ParamId.NorMap:
                    if (sfTextureByName.TryGetValue(text, out texture))
                        meshMaterial.nor = texture;
                    break;
                case (long)ParamId.PrmMap:
                    if (sfTextureByName.TryGetValue(text, out texture))
                        meshMaterial.prm = texture;
                    break;
                case (long)ParamId.EmiMap:
                    if (sfTextureByName.TryGetValue(text, out texture))
                        meshMaterial.emi = texture;
                    break;
                case (long)ParamId.BakeLitMap:
                    if (sfTextureByName.TryGetValue(text, out texture))
                        meshMaterial.bakeLit = texture;
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
            if (RenderSettings.Instance.RenderBones)
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
