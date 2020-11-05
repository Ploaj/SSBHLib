using CrossMod.Rendering;
using CrossMod.Rendering.GlTools;
using CrossMod.Tools;
using CrossModGui.Tools;
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

        public MaterialEditorWindowViewModel(Matl? matl, IEnumerable<string>? textureNames)
        {
            if (textureNames != null)
                PossibleTextureNames.AddRange(textureNames);

            // TODO: Restrict the textures used for cube maps.
            foreach (var name in TextureAssignment.defaultTexturesByName.Keys)
                PossibleTextureNames.Add(name);

            if (matl != null)
            {
                for (int i = 0; i < matl.Entries.Length; i++)
                {
                    var material = CreateMaterial(matl.Entries[i], i);
                    Materials.Add(material);
                }
            }
        }

        private Material CreateMaterial(MatlEntry entry, int index)
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

            // TODO: Sync changes with the render materials?

            return material;
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
