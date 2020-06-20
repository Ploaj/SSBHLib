using OpenTK.Graphics.OpenGL;

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

        public static TextureWrapMode GetWrapMode(int wrapMode)
        {
            if (wrapMode == 0)
                return TextureWrapMode.Repeat;
            if (wrapMode == 1)
                return TextureWrapMode.ClampToEdge;
            if (wrapMode == 2)
                return TextureWrapMode.MirroredRepeat;
            if (wrapMode == 3)
                return TextureWrapMode.ClampToBorder;

            return TextureWrapMode.ClampToEdge;
        }

        public static CullFaceMode GetCullMode(int cullMode)
        {
            if (cullMode == 0)
                return CullFaceMode.Back;
            if (cullMode == 1)
                return CullFaceMode.Front;
            if (cullMode == 2)
                return CullFaceMode.FrontAndBack;

            return CullFaceMode.Back;
        }
    }
}
