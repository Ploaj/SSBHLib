using System.Collections.Generic;
using SFGraphics.GLObjects.Textures;
using SFGraphics.GLObjects.Samplers;

namespace CrossMod.Rendering
{
    public class Material
    {
        public Texture col = null;
        public SamplerObject colSampler = null;

        public Texture col2 = null;
        public SamplerObject col2Sampler = null;

        public Texture nor = null;
        public SamplerObject norSampler = null;

        public Texture prm = null;
        public SamplerObject prmSampler = null;

        public Texture emi = null;
        public SamplerObject emiSampler = null;

        public Texture bakeLit = null;
        public SamplerObject bakeLitSampler = null;

        public Texture gao = null;
        public SamplerObject gaoSampler = null;

        public Dictionary<long, SSBHLib.Formats.MTAL_Attribute.MTAL_Vector4> vec4ByParamId = new Dictionary<long, SSBHLib.Formats.MTAL_Attribute.MTAL_Vector4>();
    }
}
