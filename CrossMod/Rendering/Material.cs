using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;

namespace CrossMod.Rendering
{
    public class Material
    {
        public Texture col = null;
        public Texture col2 = null;
        public Texture nor = null;
        public Texture prm = null;
        public Texture emi = null;
        public Texture bakeLit = null;

        public Dictionary<long, SSBHLib.Formats.MTAL_Attribute.MTAL_Vector4> vec4ByParamId = new Dictionary<long, SSBHLib.Formats.MTAL_Attribute.MTAL_Vector4>();
    }
}
