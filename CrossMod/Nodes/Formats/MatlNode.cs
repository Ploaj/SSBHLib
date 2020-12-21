using SSBHLib;
using SSBHLib.Formats.Materials;

namespace CrossMod.Nodes
{
    public class MatlNode : FileNode
    {
        public Matl? Material { get; set; }

        public MatlNode(string path) : base(path, "material", false)
        {
            if (Ssbh.TryParseSsbhFile(AbsolutePath, out Matl newMaterial))
                Material = newMaterial;
        }
    }
}
