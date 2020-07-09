using SFGraphics.Cameras;
using SSBHLib.Formats.Materials;

namespace CrossModGui.ViewModels
{
    public static class ViewModelConversion
    {
        public static void SetValues(this CrossMod.Rendering.RenderSettings settings, RenderSettingsWindowViewModel vm)
        {
            settings.EnableRed = vm.EnableRed;
            settings.EnableGreen = vm.EnableGreen;
            settings.EnableBlue = vm.EnableBlue;
            settings.EnableAlpha = vm.EnableAlpha;
            settings.EnableDiffuse = vm.EnableDiffuse;
            settings.EnableEmission = vm.EnableEmission;
            settings.EnableRimLighting = vm.EnableEdgeTint;
            settings.EnableSpecular = vm.EnableSpecular;
            settings.ShadingMode = vm.SelectedRenderMode;
            settings.RenderVertexColor = vm.EnableVertexColor;
            settings.RenderNormalMaps = vm.EnableNormalMaps;
            settings.DirectLightIntensity = vm.DirectLightIntensity;
            settings.IblIntensity = vm.IndirectLightIntensity;
            if (System.Enum.TryParse(vm.ParamName, out MatlEnums.ParamId paramId))
                settings.ParamId = (uint)paramId;
        }

        public static void SetValues(this Camera camera, CameraSettingsWindowViewModel vm)
        {
            camera.Translation = new OpenTK.Vector3(vm.PositionX, vm.PositionY, vm.PositionZ);
            camera.RotationXDegrees = vm.RotationXDegrees;
            camera.RotationYDegrees = vm.RotationYDegrees;
        }
    }
}
