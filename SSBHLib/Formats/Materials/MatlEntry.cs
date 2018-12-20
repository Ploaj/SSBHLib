namespace SSBHLib.Formats.Materials
{
    public class MatlEntry : ISSBH_File
    {
        public string MaterialLabel { get; set; }

        public MatlAttribute[] Attributes { get; set; }

        public string MaterialName { get; set; }
    }
}