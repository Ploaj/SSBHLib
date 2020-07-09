namespace SSBHLib.Formats.Materials
{
    public class MatlEntry : SsbhFile
    {
        public string MaterialLabel { get; set; }

        public MatlAttribute[] Attributes { get; set; }

        // TODO: This name is confusing and should probably be changed to ShaderName/ShaderLabel
        public string MaterialName { get; set; }
    }
}