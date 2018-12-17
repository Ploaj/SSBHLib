using SSBHLib.IO;
using SSBHLib.Formats;
using System.IO;

namespace SSBHLib
{
    public abstract class SSBH
    {
        public static void TrySaveSSBHFile(string FilePath, ISSBH_File File)
        {
            SSBHExporter.WriteSSBHFile(FilePath, File, true);
        }

        public static bool TryParseSSBHFile(string FilePath, out ISSBH_File HBSSFile)
        {
            return TryParseSSBHFile(File.ReadAllBytes(FilePath), out HBSSFile);
        }
        
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
