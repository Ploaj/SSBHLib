using SsbhLib.MatlXml;
using SSBHLib.Formats.Materials;
using System;
using System.IO;

namespace CrossModGui.MaterialPresets
{
    public static class MaterialPresets
    {
        public static Lazy<Matl> Presets = new Lazy<Matl>(() => LoadPresets());

        private static Matl LoadPresets()
        {
            // TODO: This may fail to read the file successfully.
            using var reader = new StringReader(File.ReadAllText("MaterialPresets/MaterialPresets.xml"));
            return MatlSerialization.DeserializeMatl(reader);
        }
    }
}
