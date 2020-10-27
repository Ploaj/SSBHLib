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
            { MatlEnums.ParamId.Texture0, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture1, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture3, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture4, DefaultTextures.Instance.DefaultNormal },
            { MatlEnums.ParamId.Texture5, DefaultTextures.Instance.DefaultBlack },
            { MatlEnums.ParamId.Texture6, DefaultTextures.Instance.DefaultParams },
            { MatlEnums.ParamId.Texture7, DefaultTextures.Instance.BlackCube },
            { MatlEnums.ParamId.Texture8, DefaultTextures.Instance.BlackCube },
            { MatlEnums.ParamId.Texture9, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture10, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture11, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture12, DefaultTextures.Instance.DefaultWhite },
            { MatlEnums.ParamId.Texture13, DefaultTextures.Instance.DefaultBlack },
            { MatlEnums.ParamId.Texture14, DefaultTextures.Instance.DefaultBlack },
            { MatlEnums.ParamId.Texture16, DefaultTextures.Instance.DefaultWhite },
        };

        public static readonly Dictionary<string, Texture> defaultTexturesByName = new Dictionary<string, Texture>
        {
            { "#replace_cubemap", DefaultTextures.Instance.SpecularPbr },
            { "/common/shader/sfxpbs/default_normal", DefaultTextures.Instance.DefaultNormal },
            { "/common/shader/sfxpbs/default_params", DefaultTextures.Instance.DefaultParams },
            { "/common/shader/sfxpbs/default_black", DefaultTextures.Instance.DefaultBlack },
            { "/common/shader/sfxpbs/default_white", DefaultTextures.Instance.DefaultWhite },
            { "/common/shader/sfxpbs/default_color", DefaultTextures.Instance.DefaultWhite },
            { "/common/shader/sfxpbs/fighter/default_params", DefaultTextures.Instance.DefaultParams },
            { "/common/shader/sfxpbs/fighter/default_normal", DefaultTextures.Instance.DefaultNormal }
        };

        public static Texture GetTexture(RMaterial material, MatlEnums.ParamId paramId)
        {
            // Set a default to avoid unnecessary conditionals in the shader.
            if (!material.textureNameByParamId.ContainsKey(paramId))
                return defaultTextureByParamId[paramId];

            var textureName = material.textureNameByParamId[paramId];
            if (material.TextureByName.ContainsKey(textureName))
                return material.TextureByName[textureName].Texture;

            if (defaultTexturesByName.ContainsKey(textureName))
                return defaultTexturesByName[textureName];
            else
                return DefaultTextures.Instance.DefaultWhite;
        }
    }
}
