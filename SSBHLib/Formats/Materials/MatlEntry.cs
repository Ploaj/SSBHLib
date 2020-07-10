namespace SSBHLib.Formats.Materials
{
    public class MatlEntry : SsbhFile
    {
        /// <summary>
        /// The unique identifier for this entry.
        /// </summary>
        public string MaterialLabel { get; set; }

        public MatlAttribute[] Attributes { get; set; }

        /// <summary>
        /// Determines what shader is used to render this material.
        /// </summary>
        public string ShaderLabel { get; set; }
    }
}