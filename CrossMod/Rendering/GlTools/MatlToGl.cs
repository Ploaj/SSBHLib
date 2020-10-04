using OpenTK.Graphics.OpenGL;
using SSBHLib.Formats.Materials;

namespace CrossMod.Rendering.GlTools
{
    public static class MatlToGl
    {
        public static TextureMagFilter GetMagFilter(int magFilter)
        {
            if (magFilter == 0)
                return TextureMagFilter.Nearest;
            if (magFilter == 1)
                return TextureMagFilter.Linear;
            if (magFilter == 2)
                return TextureMagFilter.Linear;

            return TextureMagFilter.Linear;
        }

        public static TextureMinFilter GetMinFilter(int minFilter)
        {
            if (minFilter == 0)
                return TextureMinFilter.Nearest;
            if (minFilter == 1)
                return TextureMinFilter.LinearMipmapLinear;
            if (minFilter == 2)
                return TextureMinFilter.LinearMipmapLinear;

            return TextureMinFilter.LinearMipmapLinear;
        }

        public static TextureWrapMode GetWrapMode(MatlWrapMode wrapMode)
        {
            if (wrapMode == MatlWrapMode.Repeat)
                return TextureWrapMode.Repeat;

            if (wrapMode == MatlWrapMode.ClampToEdge)
                return TextureWrapMode.ClampToEdge;

            if (wrapMode == MatlWrapMode.MirroredRepeat)
                return TextureWrapMode.MirroredRepeat;

            if (wrapMode == MatlWrapMode.ClampToBorder)
                return TextureWrapMode.ClampToBorder;

            return TextureWrapMode.ClampToEdge;
        }

        public static CullFaceMode GetCullMode(MatlCullMode cullMode)
        {
            if (cullMode == MatlCullMode.Back)
                return CullFaceMode.Back;
            if (cullMode == MatlCullMode.Front)
                return CullFaceMode.Front;

            return CullFaceMode.Back;
        }
    }
}
