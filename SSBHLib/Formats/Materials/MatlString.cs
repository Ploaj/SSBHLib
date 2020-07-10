namespace SSBHLib.Formats.Materials
{
    public partial class MatlAttribute
    {
        /// <summary>
        /// Stores texture names or text values.
        /// </summary>
        public class MatlString : SsbhFile
        {
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}