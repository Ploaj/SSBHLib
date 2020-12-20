using CrossMod.Rendering;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;

namespace CrossModGui.ViewModels
{
    public class RenderSettingsWindowViewModel : ViewModelBase
    {
        public class RenderModeItem
        {
            public RenderSettings.RenderMode Value { get; set; }
            public string Description { get; set; } = "";
            public string Category { get; set; } = "";
        }

        private static readonly List<RenderModeItem> renderModeItems = new List<RenderModeItem>
        {
            new RenderModeItem { Value = RenderSettings.RenderMode.Shaded, Description = "Shaded", Category = "Shading" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Basic, Description = "Basic", Category = "Shading"  },
            new RenderModeItem { Value = RenderSettings.RenderMode.Normals, Description = "Normals", Category = "Shading" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Bitangents, Description = "Bitangents", Category = "Shading" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Albedo, Description = "Albedo", Category = "Shading"  },
            new RenderModeItem { Value = RenderSettings.RenderMode.Col, Description = "Col (Texture0)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Gao, Description = "GAO (Texture3)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Nor, Description = "NOR (Texture4)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Emi, Description = "Emi (Texture5)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Prm, Description = "PRM (Texture6)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Proj, Description = "Specular Cube (Texture7)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.BakedLighting, Description = "Baked Lighting (Texture9)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Proj, Description = "Proj (Texture13)", Category = "Textures" },
            new RenderModeItem { Value = RenderSettings.RenderMode.ColorSet1, Description = "colorSet1", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.ColorSet2, Description = "colorSet2", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.ColorSet3, Description = "colorSet3", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Tangent0, Description = "Tangent0", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Map1, Description = "map1", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.Bake1, Description = "bake1", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.UvSet, Description = "uvSet", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.UvSet1, Description = "uvSet1", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.UvSet2, Description = "uvSet2", Category = "Vertex Attributes" },
            new RenderModeItem { Value = RenderSettings.RenderMode.AnisotropyLines, Description = "Anisotropic Highlight Direction", Category = "Debug" },
            new RenderModeItem { Value = RenderSettings.RenderMode.ParamID, Description = "Param Values", Category = "Debug" },
            new RenderModeItem { Value = RenderSettings.RenderMode.MaterialID, Description = "Material ID", Category = "Debug" }
        };

        private static readonly List<RenderSettings.RenderMode> uvModes = new List<RenderSettings.RenderMode>() 
        {
            RenderSettings.RenderMode.Map1,
            RenderSettings.RenderMode.Bake1,
            RenderSettings.RenderMode.UvSet,
            RenderSettings.RenderMode.UvSet1,
            RenderSettings.RenderMode.UvSet2,
        };

        public ListCollectionView RenderModes { get; } = new ListCollectionView(renderModeItems);

        public RenderModeItem SelectedRenderMode { get; set; } = renderModeItems[0];

        public bool ShowParamControls => SelectedRenderMode.Value == RenderSettings.RenderMode.ParamID;

        public bool ShowChannelControls => SelectedRenderMode.Value != RenderSettings.RenderMode.Shaded;

        public bool ShowUvDisplayMode => uvModes.Contains(SelectedRenderMode.Value);

        public bool EnableBloom { get; set; }
        public float BloomIntensity { get; set; }

        public bool EnableDiffuse { get; set; }

        public bool EnableSpecular { get; set; }

        public bool EnableEmission { get; set; }

        public float DirectLightIntensity { get; set; }

        public float IndirectLightIntensity { get; set; }

        public bool EnableNorMaps { get; set; }

        public bool EnablePrmMetalness { get; set; }
        public bool EnablePrmRoughness { get; set; }
        public bool EnablePrmAo { get; set; }
        public bool EnablePrmSpecular { get; set; }


        public bool EnableVertexColor { get; set; }

        public bool EnableRimLighting { get; set; }

        public bool EnableRed { get; set; }

        public bool EnableGreen { get; set; }

        public bool EnableBlue { get; set; }

        public bool EnableAlpha { get; set; }

        public bool UseUvPattern { get; set; }

        public bool EnableWireframe { get; set; }

        // TODO: This should use an enum and combobox.
        // The available items should be restricted to used material params (ex: not DiffuseTexture).
        public string ParamName { get; set; } = RenderSettings.Instance.ParamId.ToString();

        public RenderSettingsWindowViewModel(RenderSettings renderSettings)
        {
            UseUvPattern = renderSettings.UseUvPattern;
            EnableRed = renderSettings.EnableRed;
            EnableGreen = renderSettings.EnableGreen;
            EnableBlue = renderSettings.EnableBlue;
            EnableAlpha = renderSettings.EnableAlpha;
            EnableDiffuse = renderSettings.EnableDiffuse;
            EnableEmission = renderSettings.EnableEmission;
            EnableRimLighting = renderSettings.EnableRimLighting;
            EnableSpecular = renderSettings.EnableSpecular;
            SelectedRenderMode = renderModeItems.FirstOrDefault(i => i.Value == renderSettings.ShadingMode);
            EnableVertexColor = renderSettings.RenderVertexColor;
            EnableNorMaps = renderSettings.RenderNorMaps;
            EnablePrmMetalness = renderSettings.RenderPrmMetalness;
            EnablePrmRoughness = renderSettings.RenderPrmRoughness;
            EnablePrmAo = renderSettings.RenderPrmAo;
            EnablePrmSpecular = renderSettings.RenderPrmSpecular;
            DirectLightIntensity = renderSettings.DirectLightIntensity;
            IndirectLightIntensity = renderSettings.DirectLightIntensity;
            EnableBloom = renderSettings.EnableBloom;
            BloomIntensity = renderSettings.BloomIntensity;
            EnableWireframe = renderSettings.EnableWireframe;

            RenderModes.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
        }

        public void SetValues(RenderSettings settings)
        {
            settings.EnableRed = EnableRed;
            settings.EnableGreen = EnableGreen;
            settings.EnableBlue = EnableBlue;
            settings.EnableAlpha = EnableAlpha;
            settings.EnableDiffuse = EnableDiffuse;
            settings.EnableEmission = EnableEmission;
            settings.EnableRimLighting = EnableRimLighting;
            settings.EnableSpecular = EnableSpecular;
            settings.ShadingMode = SelectedRenderMode.Value;
            settings.RenderVertexColor = EnableVertexColor;
            settings.RenderNorMaps = EnableNorMaps;
            settings.RenderPrmMetalness = EnablePrmMetalness;
            settings.RenderPrmRoughness = EnablePrmRoughness;
            settings.RenderPrmAo = EnablePrmAo;
            settings.RenderPrmSpecular = EnablePrmSpecular;
            settings.EnableWireframe = EnableWireframe;
            settings.UseUvPattern = UseUvPattern;

            settings.DirectLightIntensity = DirectLightIntensity;
            settings.IblIntensity = IndirectLightIntensity;
            if (System.Enum.TryParse(ParamName, out MatlEnums.ParamId paramId))
                settings.ParamId = paramId;
            settings.BloomIntensity = BloomIntensity;
            settings.EnableBloom = EnableBloom;
        }
    }
}
