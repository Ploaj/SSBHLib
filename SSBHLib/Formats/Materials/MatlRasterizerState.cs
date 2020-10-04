namespace SSBHLib.Formats.Materials
{
    public enum MatlFillMode
    {
        Line = 0,
        Solid = 1
    }

    public enum MatlCullMode
    {
        Back = 0,
        Front = 1,
        None = 2
    }

    public partial class MatlAttribute
    {
        public class MatlRasterizerState : SsbhFile
        {
            public MatlFillMode FillMode { get; set; }

            public MatlCullMode CullMode { get; set; }

            public float DepthBias { get; set; }

            public float Unk4 { get; set; }

            public float Unk5 { get; set; }

            public int Unk6 { get; set; }

            public int Unk7 { get; set; }

            public float Unk8 { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }
    }
}