using System.Collections.Generic;
using System.Diagnostics;
using CrossMod.Rendering.Models;
using CrossMod.Rendering.Resources;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Samplers;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;

namespace CrossMod.Rendering
{
    public class Rnumdl : IRenderableModel
    {
        public Modl MODL;

        public Dictionary<string, Texture> sfTextureByName = new Dictionary<string, Texture>();
        public RSkeleton Skeleton;
        public RModel Model;
        public Matl Material;

        public enum ParamId
        {
            ColMap = 0x5C,
            ColMap2 = 0x5D,
            GaoMap = 0x5F,
            NorMap = 0x60,
            EmiMap = 0x61,
            EmiMap2 = 0x6A,
            PrmMap = 0x62,
            SpecularCubeMap = 0x63,
            DifCubeMap = 0x64,
            BakeLitMap = 0x65,
            DiffuseMap = 0x66,
            DiffuseMap2 = 0x67,
            DiffuseMap3 = 0x68,
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
            foreach (ModlEntry e in MODL.ModelEntries)
            {
                MatlEntry currentEntry = null;
                foreach (MatlEntry entry in Material.Entries)
                {
                    if (entry.MaterialLabel.Equals(e.MaterialName))
                    {
                        currentEntry = entry;
                        break;
                    }
                }
                if (currentEntry == null)
                    continue;

                Debug.WriteLine(e.MeshName);
                Material meshMaterial = GetMaterial(currentEntry);
                Debug.WriteLine("");

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
                        if (subIndex == e.SubIndex && mesh.Name.Equals(e.MeshName))
                        {
                            mesh.Material = meshMaterial;
                            break;
                        }
                    }
                }
            }
        }

        private Material GetMaterial(MatlEntry currentEntry)
        {
            // HACK: This is pretty gross.
            // I need to rework the entire texture loading system.
            if (RMesh.defaultTextures == null)
                RMesh.defaultTextures = new DefaultTextures();

            Material meshMaterial = new Material(RMesh.defaultTextures)
            {
                Name = currentEntry.MaterialLabel
            };

            Debug.WriteLine($"{currentEntry.MaterialName} {currentEntry.MaterialLabel}");
            foreach (MatlAttribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null)
                    continue;

                Debug.WriteLine($"{a.DataType} {a.ParamId} {a.DataObject}");

                switch (a.DataType)
                {
                    case MatlEnums.ParamDataType.String:
                        SetTextureParameter(meshMaterial, a);
                        // HACK: Just render as white if texture is present.
                        meshMaterial.floatByParamId[a.ParamId] = 1;
                        break;
                    case MatlEnums.ParamDataType.Vector4:
                        var vec4 = (MatlAttribute.MatlVector4)a.DataObject; 
                        meshMaterial.vec4ByParamId[a.ParamId] = new Vector4(vec4.X, vec4.Y, vec4.Z, vec4.W);
                        break;
                    case MatlEnums.ParamDataType.Boolean:
                        // Convert to vec4 to use with rendering.
                        // Use cyan to differentiate with no value (blue).
                        bool boolValue = (bool)a.DataObject;
                        meshMaterial.boolByParamId[a.ParamId] = boolValue;
                        break;
                    case MatlEnums.ParamDataType.Float:
                        float floatValue = (float)a.DataObject;
                        meshMaterial.floatByParamId[a.ParamId] = floatValue;
                        break;
                    case MatlEnums.ParamDataType.BlendState:
                        SetBlendState(meshMaterial, a);
                        break;
                    case MatlEnums.ParamDataType.RasterizerState:
                        SetRasterizerState(meshMaterial, a);
                        break;
                }
            }

            // HACK: Textures need to be initialized first before we can modify their state.
            foreach (MatlAttribute a in currentEntry.Attributes)
            {
                if (a.DataObject == null || a.DataType != MatlEnums.ParamDataType.Sampler)
                    continue;

                SetSamplerInformation(meshMaterial, a);
            }

            return meshMaterial;
        }

        private void SetSamplerInformation(Material material, MatlAttribute a)
        {
            // TODO: This could be cleaner.
            SamplerObject sampler = null;
            switch ((long)a.ParamId)
            {
                case (long)ParamId.ColSampler:
                    sampler = material.colSampler;
                    break;
                case (long)ParamId.NorSampler:
                    sampler = material.norSampler;
                    break;
                case (long)ParamId.PrmSampler:
                    sampler = material.prmSampler;
                    break;
                case (long)ParamId.EmiSampler:
                    sampler = material.emiSampler;
                    break;
            }

            if (sampler != null)
            {
                var samplerStruct = (MatlAttribute.MatlSampler)a.DataObject;
                sampler.TextureWrapS = MatlToGl.GetWrapMode(samplerStruct.WrapS);
                sampler.TextureWrapT = MatlToGl.GetWrapMode(samplerStruct.WrapT);
                sampler.TextureWrapR = MatlToGl.GetWrapMode(samplerStruct.WrapR);
                sampler.MagFilter = MatlToGl.GetMagFilter(samplerStruct.MagFilter);
                sampler.MinFilter = MatlToGl.GetMinFilter(samplerStruct.MinFilter);

                GL.SamplerParameter(sampler.Id, SamplerParameterName.TextureLodBias, samplerStruct.LodBias);

                if (samplerStruct.Unk6 == 2)
                    GL.SamplerParameter(sampler.Id, SamplerParameterName.TextureMaxAnisotropyExt, samplerStruct.MaxAnisotropy);
                else
                    GL.SamplerParameter(sampler.Id, SamplerParameterName.TextureMaxAnisotropyExt, 1.0f);
            }
        }

        private void SetRasterizerState(Material meshMaterial, MatlAttribute a)
        {
            // TODO: There's a cleaner way to do this.
            var rasterizerState = (MatlAttribute.MatlRasterizerState)a.DataObject;

            meshMaterial.DepthBias = rasterizerState.DepthBias;
            meshMaterial.CullMode = MatlToGl.GetCullMode(rasterizerState.CullMode);
        }
        private void SetBlendState(Material meshMaterial, MatlAttribute a)
        {
            // TODO: There's a cleaner way to do this.
            var blendState = (MatlAttribute.MatlBlendState)a.DataObject;

            // TODO: Does "src factor" toggle something in the shader?
            meshMaterial.BlendSrc = BlendingFactor.One;
            if (blendState.Unk1 == 0)
                meshMaterial.BlendSrc = BlendingFactor.Zero;

            if (blendState.BlendFactor2 == 1)
                meshMaterial.BlendDst = BlendingFactor.One;
            else if (blendState.BlendFactor2 == 2)
                meshMaterial.BlendDst = BlendingFactor.SrcAlpha;
            else if (blendState.BlendFactor2 == 6)
                meshMaterial.BlendDst = BlendingFactor.OneMinusSrcAlpha;

            meshMaterial.IsTransparent = blendState.BlendFactor1 != 0 || blendState.BlendFactor2 != 0;

            // TODO: Do both need to be set?
            meshMaterial.UseAlphaSampleCoverage = blendState.Unk7 == 1 || blendState.Unk8 == 1;
        }

        private void SetTextureParameter(Material meshMaterial, MatlAttribute a)
        {
            // Don't make texture names case sensitive.
            var text = ((MatlAttribute.MatlString)a.DataObject).Text.ToLower();

            if (sfTextureByName.TryGetValue(text, out Texture texture))
            {
                switch ((long)a.ParamId)
                {
                    case (long)ParamId.ColMap:
                        meshMaterial.HasCol = true;
                        meshMaterial.col = texture;
                        break;
                    case (long)ParamId.SpecularCubeMap:
                        meshMaterial.specularCubeMap = texture;
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
                    case (long)ParamId.DifCubeMap:
                        meshMaterial.difCube = texture;
                        meshMaterial.HasDifCube = true;
                        break;
                    case (long)ParamId.PrmMap:
                        meshMaterial.prm = texture;
                        break;
                    case (long)ParamId.EmiMap:
                        meshMaterial.emi = texture;
                        meshMaterial.HasEmi = true;
                        break;
                    case (long)ParamId.EmiMap2:
                        meshMaterial.emi2 = texture;
                        meshMaterial.HasEmi2 = true;
                        break;
                    case (long)ParamId.BakeLitMap:
                        meshMaterial.bakeLit = texture;
                        break;
                    case (long)ParamId.InkNorMap:
                        meshMaterial.HasInkNorMap = true;
                        meshMaterial.inkNor = texture;
                        break;
                    case (long)ParamId.DiffuseMap:
                        meshMaterial.dif = texture;
                        meshMaterial.HasDiffuse = true;
                        break;
                    case (long)ParamId.DiffuseMap2:
                        meshMaterial.dif2 = texture;
                        meshMaterial.HasDiffuse2 = true;
                        break;
                    case (long)ParamId.DiffuseMap3:
                        meshMaterial.dif3 = texture;
                        meshMaterial.HasDiffuse3 = true;
                        break;
                }
            }

            // TODO: Find a better way to handle this case.
            if (((long)a.ParamId == (long)ParamId.SpecularCubeMap) && text == "#replace_cubemap")
                meshMaterial.specularCubeMap = meshMaterial.defaultTextures.specularPbr;
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
