namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute
    {
        public class MatlString : ISSBH_File
        {
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}