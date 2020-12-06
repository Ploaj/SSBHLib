using SSBHLib.Formats.Materials;

namespace CrossMod.Rendering
{
    public class RenderSettings
    {
        public enum RenderMode
        {
            Shaded,
            Basic,
            Col,
            Prm,
            Nor,
            Emi,
            BakeLit,
            Gao,
            Proj,
            ColorSet,
            Normals,
            Tangents,
            Bitangents,
            BakeUV,
            UVPattern,
            ParamID,
            MaterialID,
            Albedo
        }

        public static RenderSettings Instance { get; } = new RenderSettings();

        public bool EnableBloom { get; set; } = true;
        public float BloomIntensity { get; set; } = 0.05f;

        public bool UseDebugShading { get => ShadingMode != 0; }

        public bool EnableDiffuse { get; set; } = true;

        public bool EnableSpecular { get; set; } = true;

        public bool EnableEmission { get; set; } = true;

        public bool EnableRimLighting { get; set; } = true;

        public bool EnableExperimental { get; set; } = true;

        public float DirectLightIntensity { get; set; } = 1.0f;

        public float IblIntensity { get; set; } = 1.0f;

        public bool RenderNorMaps { get; set; } = true;

        public bool RenderPrmMetalness { get; set; } = true;
        public bool RenderPrmRoughness { get; set; } = true;
        public bool RenderPrmAo { get; set; } = true;
        public bool RenderPrmSpecular { get; set; } = true;

        public bool RenderVertexColor { get; set; } = true;

        public bool EnableWireframe { get; set; } = false;

        public bool RenderBones { get; set; } = false;

        public bool RenderHitCollisions { get; set; } = false;

        public float RenderSpeed { get; set; } = 1;

        public int CliffHangID { get; set; } = -1;

        public float ModelScale { get; set; } = 1f;

        public float FloatTestParam { get; set; } = 1f;

        public OpenTK.Vector4 BoneColor { get; set; } = new OpenTK.Vector4(1);

        public RenderMode ShadingMode { get; set; } = RenderMode.Shaded;

        public bool EnableRed
        {
            get => renderChannels.X == 1;
            set => renderChannels.X = value ? 1 : 0;
        }

        public bool EnableGreen
        {
            get => renderChannels.Y == 1;
            set => renderChannels.Y = value ? 1 : 0;
        }

        public bool EnableBlue
        {
            get => renderChannels.Z == 1;
            set => renderChannels.Z = value ? 1 : 0;
        }

        public bool EnableAlpha
        {
            get => renderChannels.W == 1;
            set => renderChannels.W = value ? 1 : 0;
        }

        public OpenTK.Vector4 renderChannels = new OpenTK.Vector4(1);

        public MatlEnums.ParamId ParamId { get; set; } = MatlEnums.ParamId.CustomVector11;

        private RenderSettings()
        {

        }
    }
}
