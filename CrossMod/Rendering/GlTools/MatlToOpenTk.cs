using OpenTK;
using OpenTK.Graphics.OpenGL;
using SSBHLib.Formats.Materials;
using System;

namespace CrossMod.Rendering.GlTools
{
    public static class MatlToOpenTk
    {
        public static Vector4 ToOpenTk(this MatlAttribute.MatlVector4 value)
        {
            return new Vector4(value.X, value.Y, value.Z, value.W);
        }

        public static TextureMagFilter ToOpenTk(this MatlMagFilter magFilter)
        {
            switch (magFilter)
            {
                case MatlMagFilter.Nearest:
                    return TextureMagFilter.Nearest;
                case MatlMagFilter.Linear:
                case MatlMagFilter.Linear2:
                    return TextureMagFilter.Linear;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {magFilter}");
            }
        }

        public static TextureMinFilter ToOpenTk(this MatlMinFilter minFilter)
        {
            switch (minFilter)
            {
                case MatlMinFilter.Nearest:
                    return TextureMinFilter.Nearest;
                case MatlMinFilter.LinearMipmapLinear:
                case MatlMinFilter.LinearMipmapLinear2:
                    return TextureMinFilter.LinearMipmapLinear;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {minFilter}");
            }
        }

        public static TextureWrapMode ToOpenTk(this MatlWrapMode wrapMode)
        {
            switch (wrapMode)
            {
                case MatlWrapMode.Repeat:
                    return TextureWrapMode.Repeat;
                case MatlWrapMode.ClampToEdge:
                    return TextureWrapMode.ClampToEdge;
                case MatlWrapMode.MirroredRepeat:
                    return TextureWrapMode.MirroredRepeat;
                case MatlWrapMode.ClampToBorder:
                    return TextureWrapMode.ClampToBorder;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {wrapMode}");
            }
        }

        public static CullFaceMode ToOpenTk(this MatlCullMode cullMode)
        {
            switch (cullMode)
            {
                // None requires explicitly disabling culling, so just return back.
                case MatlCullMode.None:
                case MatlCullMode.Back:
                    return CullFaceMode.Back;
                case MatlCullMode.Front:
                    return CullFaceMode.Front;
                default:
                    throw new NotSupportedException($"Unsupported conversion for {cullMode}");
            }
        }
    }
}
