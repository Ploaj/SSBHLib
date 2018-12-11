using GenericValueEditor;

namespace CrossMod.Rendering
{
    public class RenderSettings
    {
        public enum RenderMode
        {
            Shaded,
            Col,
            Prm,
            Nor,
            Emi,
            BakeLit,
            Gao,
            ColorSet,
            Normals,
            Tangents,
            BakeColor,
            UVPattern,
            ParamID
        }

        public static RenderSettings Instance { get; } = new RenderSettings();

        public bool UseDebugShading { get => renderMode != 0; }

        [EditInfo("Enable Diffuse", ValueEnums.ValueType.Bool, "Lighting")]
        public bool enableDiffuse = true;

        [EditInfo("Enable Specular", ValueEnums.ValueType.Bool, "Lighting")]
        public bool enableSpecular = true;

        [EditInfo("Enable Wireframe", ValueEnums.ValueType.Bool, "Misc")]
        public bool enableWireframe = false;

        [EditInfo("Enable Ditto Form", ValueEnums.ValueType.Bool, "Misc")]
        public bool useDittoForm = false;

        [EditInfo("Render Bones", ValueEnums.ValueType.Bool, "Misc")]
        public bool renderBones = false;

        [EditInfo("Render Mode", ValueEnums.ValueType.Enum, "Debug Shading")]
        public RenderMode renderMode = RenderMode.Shaded;

        [EditInfo("Red", ValueEnums.ValueType.Bool, "Debug Shading")]
        public bool EnableRed
        {
            get => renderChannels.X == 1;
            set => renderChannels.X = value ? 1 : 0;
        }

        [EditInfo("Green", ValueEnums.ValueType.Bool, "Debug Shading")]
        public bool EnableGreen
        {
            get => renderChannels.Y == 1;
            set => renderChannels.Y = value ? 1 : 0;
        }

        [EditInfo("Blue", ValueEnums.ValueType.Bool, "Debug Shading")]
        public bool EnableBlue
        {
            get => renderChannels.Z == 1;
            set => renderChannels.Z = value ? 1 : 0;
        }

        [EditInfo("Alpha", ValueEnums.ValueType.Bool, "Debug Shading")]
        public bool EnableAlpha
        {
            get => renderChannels.W == 1;
            set => renderChannels.W = value ? 1 : 0;
        }

        public OpenTK.Vector4 renderChannels = new OpenTK.Vector4(1);

        // TODO: Replace with proper type.
        [EditInfo("Param ID", ValueEnums.ValueType.UintFlag, "Debug Shading")]
        public uint paramId = 0;

        private RenderSettings()
        {

        }
    }
}
