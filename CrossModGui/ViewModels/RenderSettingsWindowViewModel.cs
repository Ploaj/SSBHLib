using CrossMod.Rendering;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;

namespace CrossModGui.ViewModels
{
    public class RenderSettingsWindowViewModel : ViewModelBase
    {
        public Dictionary<RenderSettings.RenderMode, string> DescriptionByRenderMode { get; } = new Dictionary<RenderSettings.RenderMode, string>()
        {
            { RenderSettings.RenderMode.Shaded, "Shaded" },
            { RenderSettings.RenderMode.Basic, "Basic" },
            { RenderSettings.RenderMode.Col, "Col" },
            { RenderSettings.RenderMode.Albedo, "Albedo (Generated)" },
            { RenderSettings.RenderMode.Prm, "PRM" },
            { RenderSettings.RenderMode.Nor, "NOR" },
            { RenderSettings.RenderMode.Emi, "Emi" },
            { RenderSettings.RenderMode.BakedLighting, "Baked Lighting" },
            { RenderSettings.RenderMode.Gao, "GAO" },
            { RenderSettings.RenderMode.Proj, "Proj Maps" },
            { RenderSettings.RenderMode.ColorSet1, "ColorSet1" },
            { RenderSettings.RenderMode.Normals, "Normals" },
            { RenderSettings.RenderMode.Tangent0, "Tangents" },
            { RenderSettings.RenderMode.Bitangents, "Bitangents (Generated)" },
            { RenderSettings.RenderMode.Bake1, "bake1" },
            { RenderSettings.RenderMode.UVPattern, "UV Test Pattern" },
            { RenderSettings.RenderMode.AnisotropyLines, "Anisotropic Highlight Direction" },
            { RenderSettings.RenderMode.ParamID, "Param Values" },
            { RenderSettings.RenderMode.MaterialID, "Material ID" }
        };

        public class EnumItem
        {
            public RenderSettings.RenderMode Value { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
        }

        private static readonly List<EnumItem> renderModeItems = new List<EnumItem>
        {
            new EnumItem { Value = RenderSettings.RenderMode.Shaded, Description = "Shaded", Category = "Shading" },
            new EnumItem { Value = RenderSettings.RenderMode.Basic, Description = "Basic", Category = "Shading"  },
            new EnumItem { Value = RenderSettings.RenderMode.Normals, Description = "Normals", Category = "Shading" },
            new EnumItem { Value = RenderSettings.RenderMode.Bitangents, Description = "Bitangents", Category = "Shading" },
            new EnumItem { Value = RenderSettings.RenderMode.Albedo, Description = "Albedo (Generated)", Category = "Shading"  },
            new EnumItem { Value = RenderSettings.RenderMode.Col, Description = "Col", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.Prm, Description = "PRM", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.Nor, Description = "NOR", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.Emi, Description = "Emi", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.BakedLighting, Description = "Baked Lighting", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.Gao, Description = "GAO", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.Proj, Description = "Proj", Category = "Textures" },
            new EnumItem { Value = RenderSettings.RenderMode.ColorSet1, Description = "colorSet1", Category = "Vertex Attributes" },
            new EnumItem { Value = RenderSettings.RenderMode.Tangent0, Description = "Tangent0", Category = "Vertex Attributes" },
            new EnumItem { Value = RenderSettings.RenderMode.Bake1, Description = "bake1", Category = "Vertex Attributes" },
            new EnumItem { Value = RenderSettings.RenderMode.UVPattern, Description = "UV Test Pattern", Category = "Debug" },
            new EnumItem { Value = RenderSettings.RenderMode.AnisotropyLines, Description = "Anisotropic Highlight Direction", Category = "Debug" },
            new EnumItem { Value = RenderSettings.RenderMode.ParamID, Description = "Param Values", Category = "Debug" },
            new EnumItem { Value = RenderSettings.RenderMode.MaterialID, Description = "Material ID", Category = "Debug" }
        };

        public ListCollectionView RenderModes { get; } = new ListCollectionView(renderModeItems);

        public EnumItem SelectedRenderMode { get; set; } = renderModeItems[0];

        public bool ShowParamControls => SelectedRenderMode.Value == RenderSettings.RenderMode.ParamID;

        public bool ShowChannelControls => SelectedRenderMode.Value != RenderSettings.RenderMode.Shaded;

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

        public bool EnableWireframe { get; set; }

        // TODO: This should use an enum and combobox.
        // The available items should be restricted to used material params (ex: not DiffuseTexture).
        public string ParamName { get; set; } = RenderSettings.Instance.ParamId.ToString();

        public RenderSettingsWindowViewModel(RenderSettings renderSettings)
        {
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

            settings.DirectLightIntensity = DirectLightIntensity;
            settings.IblIntensity = IndirectLightIntensity;
            if (System.Enum.TryParse(ParamName, out MatlEnums.ParamId paramId))
                settings.ParamId = paramId;
            settings.BloomIntensity = BloomIntensity;
            settings.EnableBloom = EnableBloom;
        }
    }
}
