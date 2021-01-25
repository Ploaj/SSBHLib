using System.Runtime.InteropServices;

namespace CrossMod.Rendering.Models
{
    [StructLayout(LayoutKind.Sequential)]
    public struct IVec4
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int W { get; set; }
    }
}
