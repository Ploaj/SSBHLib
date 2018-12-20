using CrossMod.Rendering.Models;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
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
            ColMap = 0x5C,
            ColMap2 = 0x5D,
            GaoMap = 0x5F,
            NorMap = 0x60,
            EmiMap = 0x61,
            PrmMap = 0x62,
            SpecularCubeMap = 0x63,
            DifCubemap = 0x64,
            BakeLitMap = 0x65,
            DiffuseMap = 0x66,
            ProjMap = 0x69,
            InkNorMap = 0x133,
            ColSampler = 0x6C,
            NorSampler = 0x70,
            PrmSampler = 0x72,
            EmiSampler = 0x71
        }

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

        public void UpdateMaterial()
        {
            foreach (MODL_Entry e in MODL.ModelEntries)
            {
                MtalEntry currentEntry = null;
                foreach (MtalEntry entry in Material.Entries)
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

        private Material GetMaterial(MtalEntry currentEntry)
        {
            // HACK: This is pretty gross.
            // I need to rework the entire texture loading system.
            if (RMesh.defaultTextures == null)
                RMesh.defaultTextures = new Resources.DefaultTextures();

            Material meshMaterial = new Material(RMesh.defaultTextures);

            System.Diagnostics.Debug.WriteLine("Material Attributes:");
            foreach (MtalAttribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null)
                    continue;

                System.Diagnostics.Debug.WriteLine($"{(MatlEnums.ParamDataType) a.DataType} {a.ParamID} {a.DataObject}");

                switch (a.DataType)
                {
                    case (long)MatlEnums.ParamDataType.String:
                        SetTextureParameter(meshMaterial, a);
                        // HACK: Just render as white if texture is present.
                        meshMaterial.floatByParamId[(int)a.ParamID] = 1;
                        break;
                    case (long)MatlEnums.ParamDataType.Vector4:
                        var vec4 = (MtalAttribute.MtalVector4)a.DataObject; 
                        meshMaterial.vec4ByParamId[(int)a.ParamID] = new OpenTK.Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case (long)MatlEnums.ParamDataType.Boolean:
                        // Convert to vec4 to use with rendering.
                        // Use cyan to differentiate with no value (blue).
                        bool boolValue = (bool)a.DataObject;
                        meshMaterial.boolByParamId[(int)a.ParamID] = boolValue;
                        break;
                    case (long)MatlEnums.ParamDataType.Float:
                        float floatValue = (float)a.DataObject;
                        meshMaterial.floatByParamId[(int)a.ParamID] = floatValue;
                        break;
                }
            }

            // HACK: Textures need to be initialized first before we can modify their state.
            foreach (MtalAttribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null || a.DataType != 0xE)
                    continue;

                SetSamplerInformation(meshMaterial, a);
            }

            return meshMaterial;
        }

        private void SetSamplerInformation(Material material, MtalAttribute a)
        {
            // TODO: Set the appropriate sampler information based on the attribute and param id.
            var samplerStruct = (MtalAttribute.MtalSampler)a.DataObject;
            var wrapS = GetWrapMode(samplerStruct.WrapS);
            var wrapT = GetWrapMode(samplerStruct.WrapT);

            switch ((long)a.ParamID)
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

        private void SetTextureParameter(Material meshMaterial, MtalAttribute a)
        {
            // Don't make texture names case sensitive.
            var text = ((MtalAttribute.MtalString)a.DataObject).Text.ToLower();

            // Create a temp so we don't make the defaults null.
            if (sfTextureByName.TryGetValue(text, out Texture texture))
            {
                switch ((long)a.ParamID)
                {
                    case (long)ParamId.ColMap:
                        meshMaterial.col = texture;
                        break;
                    case (long)ParamId.GaoMap:
                        meshMaterial.gao = texture;
                        break;
                    case (long)ParamId.ColMap2:
                        meshMaterial.col2 = texture;
                        meshMaterial.HasCol2 = true;
                        break;
                    case (long)ParamId.NorMap:
                        meshMaterial.nor = texture;
                        break;
                    case (long)ParamId.ProjMap:
                        meshMaterial.proj = texture;
                        break;
                    case (long)ParamId.DifCubemap:
                        meshMaterial.difCube = texture;
                        break;
                    case (long)ParamId.PrmMap:
                        meshMaterial.prm = texture;
                        break;
                    case (long)ParamId.EmiMap:
                        meshMaterial.emi = texture;
                        break;
                    case (long)ParamId.BakeLitMap:
                        meshMaterial.bakeLit = texture;
                        break;
                    case (long)ParamId.InkNorMap:
                        meshMaterial.inkNor = texture;
                        break;
                    case (long)ParamId.DiffuseMap:
                        meshMaterial.dif = texture;
                        meshMaterial.HasDiffuse = true;
                        break;
                }
            }

            // TODO: Cube map reading doesn't work yet, so we need to assign it separately.
            if ((long)a.ParamID == (long)ParamId.SpecularCubeMap)
                meshMaterial.specularIbl = meshMaterial.defaultTextures.specularPbr;
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
