namespace SSBHLib.Formats
{
    /// <summary>
    /// A null terminated string that uses the same alignment as other SSBH formats.
    /// </summary>
    public class SsbhString : SsbhFile
    {
        public string Text { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
