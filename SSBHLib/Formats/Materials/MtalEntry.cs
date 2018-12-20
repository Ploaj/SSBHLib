namespace SSBHLib.Formats.Materials
{
    public class MtalEntry : ISSBH_File
    {
        public string MaterialLabel { get; set; }

        public MtalAttribute[] Attributes { get; set; }

        public string MaterialName { get; set; }
    }
}