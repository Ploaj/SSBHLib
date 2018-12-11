using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Textures;
using System.Collections.Generic;
using System.Drawing;

namespace CrossMod.Rendering.Resources
{
    public class DefaultTextures
    {
        // Default textures.
        public Texture2D defaultWhite = null;
        public Texture2D defaultNormal = null;
        public Texture2D defaultBlack = null;
        public Texture2D defaultPrm = null;

        // Render modes.
        public Texture2D uvPattern = null;

        // PBR image based lighting.
        public Texture2D iblLut = null;
        public TextureCubeMap diffusePbr = null;
        public TextureCubeMap specularPbr = null;

        public DefaultTextures()
        {
            uvPattern = new Texture2D()
            {
                TextureWrapS = TextureWrapMode.Repeat,
                TextureWrapT = TextureWrapMode.Repeat
            };
            LoadBitmap(uvPattern, "DefaultTextures/UVPattern.png");

            defaultWhite = new Texture2D();
            LoadBitmap(defaultWhite, "DefaultTextures/default_White.png");

            defaultPrm = new Texture2D();
            LoadBitmap(defaultPrm, "DefaultTextures/default_Params.tif");

            defaultNormal = new Texture2D();
            LoadBitmap(defaultNormal, "DefaultTextures/default_normal.png");

            defaultBlack = new Texture2D();
            LoadBitmap(defaultBlack, "DefaultTextures/default_black.png");

            iblLut = new Texture2D();
            LoadBitmap(iblLut, "DefaultTextures/ibl_brdf_lut.png");

            LoadDiffusePbr();
            LoadSpecularPbr();
        }

        private void LoadBitmap(Texture2D texture, string path)
        {
            using (var bmp = new Bitmap(path))
            {
                texture.LoadImageData(bmp);
            }
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
