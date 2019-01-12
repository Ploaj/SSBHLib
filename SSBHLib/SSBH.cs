using SSBHLib.IO;
using SSBHLib.Formats;
using System.IO;

namespace SSBHLib
{
    public abstract class SSBH
    {
        /// <summary>
        /// Attempts to save the SSBH supported file to given filepath
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="File"></param>
        public static void TrySaveSSBHFile(string FilePath, ISSBH_File File)
        {
            SSBHExporter.WriteSSBHFile(FilePath, File, true);
        }

        /// <summary>
        /// Trys to parse an SSBH file from a filepath
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="HBSSFile"></param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSSBHFile(string FilePath, out ISSBH_File HBSSFile)
        {
            return TryParseSSBHFile(File.ReadAllBytes(FilePath), out HBSSFile);
        }
        
        /// <summary>
        /// Trys to parse an SSBH file from a byte array
        /// </summary>
        /// <param name="FileData"></param>
        /// <param name="HBSSFile"></param>
        /// <returns>true if parsing was successful</returns>
        public static bool TryParseSSBHFile(byte[] FileData, out ISSBH_File HBSSFile)
        {
            HBSSFile = null;
            using (SSBHParser R = new SSBHParser(new MemoryStream(FileData)))
            {
                return R.TryParse(out HBSSFile);
            }
        }
    }
}
