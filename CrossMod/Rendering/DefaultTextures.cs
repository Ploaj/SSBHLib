using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Textures;
using System.Collections.Generic;
using System.Drawing;

namespace CrossMod.Rendering
{
    public class DefaultTextures
    {
        public Texture2D defaultWhite = null;
        public Texture2D defaultNormal = null;
        public TextureCubeMap diffusePbr = null;
        public TextureCubeMap specularPbr = null;

        public DefaultTextures()
        {
            defaultWhite = new Texture2D();
            using (var whiteBmp = new Bitmap("DefaultTextures/default_White.png"))
            {
                defaultWhite.LoadImageData(whiteBmp);
            }

            defaultNormal = new Texture2D();
            using (var nrmBmp = new Bitmap("DefaultTextures/default_normal.png"))
            {
                defaultNormal.LoadImageData(nrmBmp);
            }

            LoadDiffusePbr();
            LoadSpecularPbr();
        }

        private void LoadDiffusePbr()
        {
            diffusePbr = new TextureCubeMap();

            var surfaceData = new List<List<byte[]>>();
            for (int surface = 0; surface < 6; surface++)
            {
                var mipData = System.IO.File.ReadAllBytes($"DefaultTextures/diffuseSdr{surface}{0}.bin");
                surfaceData.Add(new List<byte[]>() { mipData });
            }
            diffusePbr.LoadImageData(128, InternalFormat.CompressedRgbaS3tcDxt1Ext, surfaceData[0], surfaceData[1], surfaceData[2], surfaceData[3], surfaceData[4], surfaceData[5]);

            // Don't Use mipmaps.
            diffusePbr.MagFilter = TextureMagFilter.Linear;
            diffusePbr.MinFilter = TextureMinFilter.Linear;
        }

        private void LoadSpecularPbr()
        {
            specularPbr = new TextureCubeMap();
            var surfaceData = new List<List<byte[]>>();
            for (int surface = 0; surface < 6; surface++)
            {
                var mipmaps = new List<byte[]>();
                for (int mip = 0; mip < 10; mip++)
                {
                    var mipData = System.IO.File.ReadAllBytes($"DefaultTextures/specularSdr{surface}{mip}.bin");
                    mipmaps.Add(mipData);
                }
                surfaceData.Add(mipmaps);
            }
            specularPbr.LoadImageData(512, InternalFormat.CompressedRgbaS3tcDxt1Ext, surfaceData[0], surfaceData[1], surfaceData[2], surfaceData[3], surfaceData[4], surfaceData[5]);
        }
    }
}
