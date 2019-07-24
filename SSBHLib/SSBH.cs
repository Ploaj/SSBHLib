using SSBHLib.IO;
using System.IO;

namespace SSBHLib
{
    public abstract class Ssbh
    {
        /// <summary>
        /// Attempts to save the SSBH supported file to given filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        public static void TrySaveSsbhFile(string filePath, SsbhFile file)
        {
            SsbhExporter.WriteSsbhFile(filePath, file, true);
        }

        /// <summary>
        /// Tries to parse an SSBH file from a filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hbssFile"></param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSsbhFile(string filePath, out SsbhFile hbssFile)
        {
            return TryParseSsbhFile(File.ReadAllBytes(filePath), out hbssFile);
        }
        
        /// <summary>
        /// Tries to parse an SSBH file from a byte array
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="hbssFile"></param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSsbhFile(byte[] fileData, out SsbhFile hbssFile)
        {
            hbssFile = null;
            using (var parser = new SsbhParser(new MemoryStream(fileData)))
            {
                return parser.TryParse(out hbssFile);
            }
        }
    }
}
