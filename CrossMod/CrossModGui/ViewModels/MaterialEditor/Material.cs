using CrossMod.Rendering.GlTools;
using CrossMod.Tools;
using SSBHLib.Formats.Materials;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class Material : ViewModelBase
    {
        public string Name
        {
            get => entry.MaterialLabel;
            set
            {
                entry.MaterialLabel = value;
                OnPropertyChanged();
            }
        }

        public string ShaderLabel 
        { 
            get => entry.ShaderLabel; 
            set
            {
                entry.ShaderLabel = value;
                OnPropertyChanged();
            }
        }

        public string SelectedRenderPass
        {
            get => ShaderLabelTools.GetRenderPass(entry.ShaderLabel);
            set
            {
                ShaderLabel = ShaderLabelTools.WithRenderPass(entry.ShaderLabel, value);
                OnPropertyChanged();
            }
        }

        public List<string> RenderPasses { get; set; } = new List<string>();

        // This will need to be updated if the shader label changes.
        public List<AttributeError> AttributeErrors { get; set; } = new List<AttributeError>();

        // This will need to be updated if the shader label changes.
        public bool IsNotValidShaderLabel { get; set; }

        public bool HasAlphaTesting { get; set; }

        public SolidColorBrush? MaterialIdColor { get; set; }

        public RasterizerStateParam? RasterizerState0 { get; set; }
        public BlendStateParam? BlendState0 { get; set; }

        public string ShaderAttributeNames { get; set; } = "";
        public string ShaderParameterNames { get; set; } = "";

        public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

        public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

        public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

        public ObservableCollection<TextureSamplerParam> TextureParams { get; } = new ObservableCollection<TextureSamplerParam>();

        private readonly MatlEntry entry;

        public Material(MatlEntry entry)
        {
            this.entry = entry;
        }

        public void SyncToRMaterial(RMaterial rMaterial, Action propertyChangedCallback)
        {
            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(ShaderLabel):
                        rMaterial.ShaderLabel = ShaderLabel;
                        propertyChangedCallback();
                        break;
                    default:
                        break;
                }
            };
            SyncBooleans(rMaterial, propertyChangedCallback);
            SyncFloats(rMaterial, propertyChangedCallback);
            SyncTexturesSamplers(rMaterial, propertyChangedCallback);
            SyncVectors(rMaterial, propertyChangedCallback);
            SyncBlendState(rMaterial, propertyChangedCallback);
            SyncRasterizerState(rMaterial, propertyChangedCallback);
        }

        private void SyncRasterizerState(RMaterial rMaterial, Action propertyChangedCallback)
        {
            if (RasterizerState0 != null)
            {
                RasterizerState0.PropertyChanged += (s, e) =>
                {
                    rMaterial.CullMode = RasterizerState0.CullMode.ToOpenTk();
                    rMaterial.EnableFaceCulling = RasterizerState0.CullMode != MatlCullMode.None;
                    rMaterial.FillMode = RasterizerState0.FillMode.ToOpenTk();
                    propertyChangedCallback();
                };
            }
        }

        private void SyncBlendState(RMaterial rMaterial, Action propertyChangedCallback)
        {
            if (BlendState0 != null)
            {
                BlendState0.PropertyChanged += (s, e) =>
                {
                    rMaterial.SourceColor = BlendState0.SourceColor.ToOpenTk();
                    rMaterial.DestinationColor = BlendState0.DestinationColor.ToOpenTk();
                    rMaterial.EnableAlphaSampleCoverage = BlendState0.EnableAlphaSampleToCoverage;
                    propertyChangedCallback();
                };
            }
        }

        private void SyncBooleans(RMaterial rMaterial, Action propertyChangedCallback)
        {
            foreach (var param in BooleanParams)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateBoolean(paramId, param.Value);
                    propertyChangedCallback();
                };
            }
        }

        private void SyncFloats(RMaterial rMaterial, Action propertyChangedCallback)
        {
            foreach (var param in FloatParams)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateFloat(paramId, param.Value);
                    propertyChangedCallback();
                };
            }
        }

        private void SyncTexturesSamplers(RMaterial rMaterial, Action propertyChangedCallback)
        {
            foreach (var param in TextureParams)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                if (!Enum.TryParse(param.SamplerParamId, out MatlEnums.ParamId samplerParamId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateTexture(paramId, param.Value);
                    rMaterial.UpdateSampler(samplerParamId, GetSamplerData(param));
                    propertyChangedCallback();
                };
            }
        }

        private void SyncVectors(RMaterial rMaterial, Action propertyChangedCallback)
        {
            foreach (var param in Vec4Params)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateVec4(paramId, new OpenTK.Vector4(param.Value1, param.Value2, param.Value3, param.Value4));
                    propertyChangedCallback();
                };
            }
        }

        private static SamplerData GetSamplerData(TextureSamplerParam param)
        {
            return new SamplerData
            {
                MinFilter = param.MinFilter.ToOpenTk(),
                MagFilter = param.MagFilter.ToOpenTk(),
                WrapS = param.WrapS.ToOpenTk(),
                WrapT = param.WrapT.ToOpenTk(),
                WrapR = param.WrapR.ToOpenTk(),
                LodBias = param.LodBias,
                // Disable anisotropic filtering if it's disabled in the material.
                MaxAnisotropy = param.TextureFilteringType == FilteringType.AnisotropicFiltering ? param.MaxAnisotropy : 1,
            };
        }
    }
}
