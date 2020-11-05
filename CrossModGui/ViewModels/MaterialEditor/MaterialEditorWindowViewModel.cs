using CrossMod.Rendering;
using CrossMod.Rendering.GlTools;
using CrossMod.Tools;
using CrossModGui.Tools;
using SFGraphics.GLObjects.Samplers;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public partial class MaterialEditorWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();

        public Material? CurrentMaterial { get; set; }

        public ObservableCollection<string> PossibleTextureNames { get; } = new ObservableCollection<string>();

        public Dictionary<MatlCullMode, string> DescriptionByCullMode { get; } = new Dictionary<MatlCullMode, string>
        {
            { MatlCullMode.Back, "Back" },
            { MatlCullMode.Front, "Front" },
            { MatlCullMode.None, "None" },
        };

        public Dictionary<MatlFillMode, string> DescriptionByFillMode { get; } = new Dictionary<MatlFillMode, string>
        {
            { MatlFillMode.Solid, "Solid" },
            { MatlFillMode.Line, "Line" },
        };

        public Dictionary<MatlMagFilter, string> DescriptionByMagFilter { get; } = new Dictionary<MatlMagFilter, string>
        {
            { MatlMagFilter.Nearest, "Nearest" },
            { MatlMagFilter.Linear, "Linear" },
            { MatlMagFilter.Linear2, "Linear + ???" },
        };

        public Dictionary<MatlMinFilter, string> DescriptionByMinFilter { get; } = new Dictionary<MatlMinFilter, string>
        {
            { MatlMinFilter.Nearest, "Nearest" },
            { MatlMinFilter.LinearMipmapLinear, "LinearMipmapLinear" },
            { MatlMinFilter.LinearMipmapLinear2, "LinearMipmapLinear2" },
        };

        public Dictionary<MatlWrapMode, string> DescriptionByWrapMode { get; } = new Dictionary<MatlWrapMode, string>
        {
            { MatlWrapMode.Repeat, "Repeat" },
            { MatlWrapMode.ClampToEdge, "ClampToEdge" },
            { MatlWrapMode.MirroredRepeat, "MirroredRepeat" },
            { MatlWrapMode.ClampToBorder, "ClampToBorder" },
        };

        public MaterialEditorWindowViewModel(RNumdl? rnumdl)
        {
            if (rnumdl == null)
                return;

            PossibleTextureNames.AddRange(rnumdl.TextureByName.Keys);

            // TODO: Restrict the textures used for cube maps.
            foreach (var name in TextureAssignment.defaultTexturesByName.Keys)
                PossibleTextureNames.Add(name);

            if (rnumdl.Matl != null)
            {
                for (int i = 0; i < rnumdl.Matl.Entries.Length; i++)
                {
                    // Pass a reference to the render material to enable real time updates.
                    rnumdl.MaterialByName.TryGetValue(rnumdl.Matl.Entries[i].MaterialLabel, out RMaterial? rMaterial);

                    var material = CreateMaterial(rnumdl.Matl.Entries[i], i, rMaterial);
                    Materials.Add(material);
                }
            }
        }

        private Material CreateMaterial(MatlEntry entry, int index, RMaterial? rMaterial)
        {
            var idColor = UniqueColors.IndexToColor(index);

            var material = new Material
            {
                Name = entry.MaterialLabel,
                ShaderLabel = entry.ShaderLabel,
                MaterialIdColor = new SolidColorBrush(Color.FromArgb(255,
                    (byte)idColor.X,
                    (byte)idColor.Y,
                    (byte)idColor.Z)),
            };

            UpdateMaterialFromEntry(entry, material);

            // Enable real time viewport updates.
            if (rMaterial !=  null)
            {
                SyncBooleans(rMaterial, material);
                SyncFloats(rMaterial, material);
                SyncTexturesSamplers(rMaterial, material);
                SyncVectors(rMaterial, material);
            }

            return material;
        }

        private static void SyncBooleans(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.BooleanParams)
            {
                if (!System.Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) => rMaterial.UpdateBoolean(paramId, param.Value);
            }
        }

        private static void SyncFloats(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.FloatParams)
            {
                if (!System.Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) => rMaterial.UpdateFloat(paramId, param.Value);
            }
        }

        private static void SyncTexturesSamplers(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.TextureParams)
            {
                if (!System.Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                // TODO: Store the sampler paramId as well?
                param.PropertyChanged += (s, e) => 
                { 
                    rMaterial.UpdateTexture(paramId, param.Value);
                    rMaterial.UpdateSampler(ParamIdExtensions.GetSampler(paramId), CreateSampler(param));
                };
            }
        }

        private static void SyncVectors(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.Vec4Params)
            {
                if (!System.Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) => rMaterial.UpdateVec4(paramId, new OpenTK.Vector4(param.Value1, param.Value2, param.Value3, param.Value4));
            }
        }

        private static SamplerData CreateSampler(TextureParam param)
        {
            return new SamplerData
            {
                MinFilter = param.MinFilter.ToOpenTk(),
                MagFilter = param.MagFilter.ToOpenTk(),
                WrapS = param.WrapS.ToOpenTk(),
                WrapT = param.WrapT.ToOpenTk(),
                WrapR = param.WrapR.ToOpenTk(),
                LodBias = param.LodBias,
                MaxAnisotropy = param.MaxAnisotropy
            };
        }

        private static void UpdateMaterialFromEntry(MatlEntry entry, Material material)
        {
            // There should only be a single rasterizer state in each material.
            if (entry.GetRasterizerStates().TryGetValue(MatlEnums.ParamId.RasterizerState0, out MatlAttribute.MatlRasterizerState? rasterizerState))
            {
                material.CullMode = rasterizerState.CullMode;
                material.FillMode = rasterizerState.FillMode;
            }

            material.BooleanParams.AddRange(entry.GetBools()
                .Select(b => new BooleanParam { ParamId = b.Key.ToString(), Value = b.Value }));

            material.FloatParams.AddRange(entry.GetFloats()
                .Select(f => new FloatParam { ParamId = f.Key.ToString(), Value = f.Value }));

            material.Vec4Params.AddRange(entry.GetVectors()
                .Select(v => new Vec4Param
                {
                    ParamId = v.Key.ToString(),
                    Value1 = v.Value.X,
                    Value2 = v.Value.Y,
                    Value3 = v.Value.Z,
                    Value4 = v.Value.W
                }));

            // Set descriptions and GUI info.
            foreach (var param in material.Vec4Params)
            {
                TryAssignValuesFromDescription(param);
            }

            material.TextureParams.AddRange(entry.GetTextures()
                .Select(t => new TextureParam { ParamId = t.Key.ToString(), Value = t.Value }));

            UpdateTextureParamsFromSamplers(entry, material);
        }

        private static void UpdateTextureParamsFromSamplers(MatlEntry entry, Material material)
        {
            var entrySamplers = entry.GetSamplers();
            foreach (var param in material.TextureParams)
            {
                if (!System.Enum.TryParse(param.ParamId, out MatlEnums.ParamId textureId))
                    continue;

                if (!entrySamplers.TryGetValue(ParamIdExtensions.GetSampler(textureId), out MatlAttribute.MatlSampler? sampler))
                    continue;

                param.WrapS = sampler.WrapS;
                param.WrapT = sampler.WrapT;
                param.WrapR = sampler.WrapR;
                param.MinFilter = sampler.MinFilter;
                param.MagFilter = sampler.MagFilter;
                param.LodBias = sampler.LodBias;
                param.MaxAnisotropy = sampler.MaxAnisotropy;
            }
        }

        public void SaveMatl(string outputPath)
        {
            // TODO: Completely recreate the Matl from the view model.
        }


        private static bool TryAssignValuesFromDescription(Vec4Param vec4Param)
        {
            if (MaterialParamDescriptions.Instance.ParamDescriptionsByName.TryGetValue(vec4Param.ParamId,
                out MaterialParamDescriptions.ParamDescription? description))
            {
                vec4Param.Label1 = description.Label1 ?? "Unused";
                vec4Param.Min1 = description.Min1.GetValueOrDefault(0);
                vec4Param.Max1 = description.Max1.GetValueOrDefault(1);

                vec4Param.Label2 = description.Label2 ?? "Unused";
                vec4Param.Min2 = description.Min2.GetValueOrDefault(0);
                vec4Param.Max2 = description.Max2.GetValueOrDefault(1);

                vec4Param.Label3 = description.Label3 ?? "Unused";
                vec4Param.Min3 = description.Min3.GetValueOrDefault(0);
                vec4Param.Max3 = description.Max3.GetValueOrDefault(1);

                vec4Param.Label4 = description.Label4 ?? "Unused";
                vec4Param.Min4 = description.Min4.GetValueOrDefault(0);
                vec4Param.Max4 = description.Max4.GetValueOrDefault(1);
                return true;
            }

            return false;
        }
    }
}
