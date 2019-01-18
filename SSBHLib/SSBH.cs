using SSBHLib.IO;
using System.IO;

namespace SSBHLib
{
    public abstract class SSBH
    {
        /// <summary>
        /// Attempts to save the SSBH supported file to given filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        public static void TrySaveSSBHFile(string filePath, ISSBH_File file)
        {
            SSBHExporter.WriteSSBHFile(filePath, file, true);
        }

        /// <summary>
        /// Trys to parse an SSBH file from a filepath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="hbssFile"></param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSSBHFile(string filePath, out ISSBH_File hbssFile)
        {
            return TryParseSSBHFile(File.ReadAllBytes(filePath), out hbssFile);
        }
        
        /// <summary>
        /// Trys to parse an SSBH file from a byte array
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="hbssFile"></param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSSBHFile(byte[] fileData, out ISSBH_File hbssFile)
        {
            hbssFile = null;
            using (var parser = new SSBHParser(new MemoryStream(fileData)))
            {
                return parser.TryParse(out hbssFile);
            }
        }
    }
}
