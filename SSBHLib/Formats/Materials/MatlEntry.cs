namespace SSBHLib.Formats.Materials
{
    public class MatlEntry : SsbhFile
    {
        public string MaterialLabel { get; set; }

        public MatlAttribute[] Attributes { get; set; }

        public string MaterialName { get; set; }
    }
}