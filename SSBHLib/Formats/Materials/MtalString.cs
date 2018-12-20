namespace SSBHLib.Formats.Materials
{
    public partial class MtalAttribute
    {
        public class MtalString : ISSBH_File
        {
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}