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
            using (var exporter = new SsbhExporter(filePath))
            {
                exporter.WriteSsbhFile(file);
            }
        }

        /// <summary>
        /// Tries to parse an SSBH file from a filepath
        /// </summary>
        /// <param name="filePath">The file to parse</param>
        /// <param name="hbssFile">The parsed file result or <c>null</c> if parsing failed</param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSsbhFile<T>(string filePath, out T hbssFile) where T : SsbhFile
        {
            return TryParseSsbhFile(File.ReadAllBytes(filePath), out hbssFile);
        }

        /// <summary>
        /// Tries to parse an SSBH file from a byte array
        /// </summary>
        /// <param name="fileData">The file to parse</param>
        /// <param name="hbssFile">The parsed file result or <c>null</c> if parsing failed</param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSsbhFile<T>(byte[] fileData, out T hbssFile) where T : SsbhFile
        {
            hbssFile = null;
            using (var parser = new SsbhParser(new MemoryStream(fileData)))
            {
                return parser.TryParse(out hbssFile);
            }
        }
    }
}
