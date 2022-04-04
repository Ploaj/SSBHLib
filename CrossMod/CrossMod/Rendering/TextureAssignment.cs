using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Resources;
using SFGraphics.GLObjects.Textures;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public static class TextureAssignment
    {
        private static readonly Dictionary<MatlEnums.ParamId, Texture> defaultTextureByParamId = new Dictionary<MatlEnums.ParamId, Texture>
        {
            { MatlEnums.ParamId.Texture0, DefaultTextures.Instance.Value.DefaultWhite },
            { MatlEnums.ParamId.Texture1, DefaultTextures.Instance.Value.DefaultWhite },
            { MatlEnums.ParamId.Texture3, DefaultTextures.Instance.Value.DefaultWhite },
            { MatlEnums.ParamId.Texture4, DefaultTextures.Instance.Value.DefaultNormal },
            { MatlEnums.ParamId.Texture5, DefaultTextures.Instance.Value.DefaultBlack },
            { MatlEnums.ParamId.Texture6, DefaultTextures.Instance.Value.DefaultPrm },
            { MatlEnums.ParamId.Texture7, DefaultTextures.Instance.Value.BlackCube },
            { MatlEnums.ParamId.Texture8, DefaultTextures.Instance.Value.BlackCube },
            { MatlEnums.ParamId.Texture9, DefaultTextures.Instance.Value.DefaultBlack },
            { MatlEnums.ParamId.Texture10, DefaultTextures.Instance.Value.DefaultWhite },
            { MatlEnums.ParamId.Texture11, DefaultTextures.Instance.Value.DefaultWhite },
            { MatlEnums.ParamId.Texture12, DefaultTextures.Instance.Value.DefaultWhite },
            { MatlEnums.ParamId.Texture13, DefaultTextures.Instance.Value.DefaultBlack },
            { MatlEnums.ParamId.Texture14, DefaultTextures.Instance.Value.DefaultBlack },
            { MatlEnums.ParamId.Texture16, DefaultTextures.Instance.Value.DefaultWhite },
        };

        public static readonly Dictionary<string, Texture> defaultTexturesByName = new Dictionary<string, Texture>
        {
            { "#replace_cubemap", DefaultTextures.Instance.Value.SpecularPbr },
            { "/common/shader/sfxpbs/default_normal", DefaultTextures.Instance.Value.DefaultNormal },
            { "/common/shader/sfxpbs/default_params", DefaultTextures.Instance.Value.DefaultParamsFighter },
            { "/common/shader/sfxpbs/default_params2", DefaultTextures.Instance.Value.DefaultParams2 },
            { "/common/shader/sfxpbs/default_params3", DefaultTextures.Instance.Value.DefaultParams3 },
            { "/common/shader/sfxpbs/default_params_r000_g025_b100", DefaultTextures.Instance.Value.DefaultParamsR000G025B100 },
            { "/common/shader/sfxpbs/default_params_r100_g025_b100", DefaultTextures.Instance.Value.DefaultParamsR100G025B100 },
            { "/common/shader/sfxpbs/default_black", DefaultTextures.Instance.Value.DefaultBlack },
            { "/common/shader/sfxpbs/default_white", DefaultTextures.Instance.Value.DefaultWhite },
            { "/common/shader/sfxpbs/default_color", DefaultTextures.Instance.Value.DefaultWhite },
            { "/common/shader/sfxpbs/default_color2", DefaultTextures.Instance.Value.DefaultWhite },
            { "/common/shader/sfxpbs/default_color3", DefaultTextures.Instance.Value.DefaultWhite },
            { "/common/shader/sfxpbs/default_color4", DefaultTextures.Instance.Value.DefaultWhite },
            { "/common/shader/sfxpbs/default_diffuse2", DefaultTextures.Instance.Value.DefaultDiffuse2 },
            { "/common/shader/sfxpbs/default_gray", DefaultTextures.Instance.Value.DefaultGray },
            { "/common/shader/sfxpbs/default_metallicbg", DefaultTextures.Instance.Value.DefaultMetallicBG },
            { "/common/shader/sfxpbs/default_specular", DefaultTextures.Instance.Value.DefaultSpecular },
            { "/common/shader/sfxpbs/fighter/default_params", DefaultTextures.Instance.Value.DefaultParamsFighter },
            { "/common/shader/sfxpbs/fighter/default_normal", DefaultTextures.Instance.Value.DefaultNormal }
        };

        public static Texture GetTexture(RMaterial material, MatlEnums.ParamId paramId)
        {
            // Set a default to avoid unnecessary conditionals in the shader.
            if (!material.HasTexture(paramId))
                return defaultTextureByParamId[paramId];

            var textureName = material.GetTextureName(paramId);
            if (material.TextureByName.ContainsKey(textureName))
                return material.TextureByName[textureName].Texture;

            if (defaultTexturesByName.ContainsKey(textureName))
                return defaultTexturesByName[textureName];
            else
                return DefaultTextures.Instance.Value.DefaultWhite;
        }
    }
}
