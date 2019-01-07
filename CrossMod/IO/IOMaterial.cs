using System;
namespace CrossMod.IO
{
    public enum IOWrapMode
    {
        WRAP,
        MIRROR,
        CLAMP,
        BORDER,
        NONE
    }

    public class IOMaterial
    {
        public string Name;

        public IOTexture DiffuseTexture;
        public IOWrapMode WrapS = IOWrapMode.WRAP;
        public IOWrapMode WrapT = IOWrapMode.WRAP;

    }

    public class IOTexture
    {
        public string Name;

    }
}
