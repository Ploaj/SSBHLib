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

        public Dictionary<long, OpenTK.Vector4> vec4ByParamId = new Dictionary<long, OpenTK.Vector4>();
        public Dictionary<long, bool> boolByParamId = new Dictionary<long, bool>();
        public Dictionary<long, float> floatByParamId = new Dictionary<long, float>();

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
