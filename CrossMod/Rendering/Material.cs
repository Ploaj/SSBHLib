using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public class Material
    {
        public Texture col = null;

        public Texture col2 = null;
        public bool HasCol2 { get; set; } = false;

        public Texture nor = null;

        public Texture prm = null;

        public Texture emi = null;

        public Texture bakeLit = null;

        public Texture gao = null;

        public Dictionary<long, SSBHLib.Formats.MTAL_Attribute.MTAL_Vector4> vec4ByParamId = new Dictionary<long, SSBHLib.Formats.MTAL_Attribute.MTAL_Vector4>();

        public Material(Resources.DefaultTextures defaultTextures)
        {
            // Ensure the textures are never null, so we can modify their state later.
            col = defaultTextures.defaultBlack;
            col2 = defaultTextures.defaultBlack;
            nor = defaultTextures.defaultNormal;
            prm = defaultTextures.defaultPrm;
            emi = defaultTextures.defaultBlack;
            bakeLit = defaultTextures.defaultBlack;
            gao = defaultTextures.defaultWhite;
        }
    }
}
