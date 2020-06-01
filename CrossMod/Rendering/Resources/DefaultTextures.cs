using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Textures;
using System.Collections.Generic;
using System.Drawing;
using SFGraphics.GLObjects.Textures.TextureFormats;

namespace CrossMod.Rendering.Resources
{
    public class DefaultTextures
    {
        // Default textures.
        public Texture2D defaultWhite = new Texture2D();
        public Texture2D defaultNormal = new Texture2D();
        public Texture2D defaultBlack = new Texture2D();
        public Texture2D defaultPrm = new Texture2D();

        // Render modes.
        public Texture2D uvPattern = new Texture2D()
        {
            TextureWrapS = TextureWrapMode.Repeat,
            TextureWrapT = TextureWrapMode.Repeat
        };

        // PBR image based lighting.
        public TextureCubeMap diffusePbr = new TextureCubeMap();
        public TextureCubeMap specularPbr = new TextureCubeMap();
        public TextureCubeMap blackCube = new TextureCubeMap();

        public DefaultTextures()
        {
            LoadBitmap(uvPattern, "DefaultTextures/UVPattern.png");

            LoadBitmap(defaultWhite, "DefaultTextures/default_White.png");
            LoadBitmap(defaultPrm, "DefaultTextures/default_Params.tif");
            LoadBitmap(defaultNormal, "DefaultTextures/default_normal.tif");
            LoadBitmap(defaultBlack, "DefaultTextures/default_black.png");

            using (var bmp = new Bitmap("DefaultTextures/default_cube_black.png"))
                blackCube.LoadImageData(bmp, 8);

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

            AddIrrFace(surfaceData, "x+");
            AddIrrFace(surfaceData, "x-");
            AddIrrFace(surfaceData, "y+");
            AddIrrFace(surfaceData, "y-");
            AddIrrFace(surfaceData, "z+");
            AddIrrFace(surfaceData, "z-");


            var format = new TextureFormatUncompressed(PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            diffusePbr.LoadImageData(64, format, surfaceData[0], surfaceData[1], surfaceData[2], surfaceData[3], surfaceData[4], surfaceData[5]);

            // Don't Use mipmaps.
            diffusePbr.MagFilter = TextureMagFilter.Linear;
            diffusePbr.MinFilter = TextureMinFilter.Linear;
        }

        private static void AddIrrFace(List<List<byte[]>> surfaceData, string surface)
        {
            var mipData = System.IO.File.ReadAllBytes($"DefaultTextures/irr {surface}.bin");
            surfaceData.Add(new List<byte[]>() { mipData });
        }

        private void LoadSpecularPbr()
        {
            specularPbr = new TextureCubeMap();
            var surfaceData = new List<List<byte[]>>();

            AddCubeMipmaps(surfaceData, "x+");
            AddCubeMipmaps(surfaceData, "x-");
            AddCubeMipmaps(surfaceData, "y+");
            AddCubeMipmaps(surfaceData, "y-");
            AddCubeMipmaps(surfaceData, "z+");
            AddCubeMipmaps(surfaceData, "z-");

            var format = new TextureFormatUncompressed(PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            specularPbr.LoadImageData(64, format, surfaceData[0], surfaceData[1], surfaceData[2], surfaceData[3], surfaceData[4], surfaceData[5]);
        }

        private static void AddCubeMipmaps(List<List<byte[]>> surfaceData, string surface)
        {
            var mipmaps = new List<byte[]>();
            for (int mip = 0; mip < 7; mip++)
            {
                var mipData = System.IO.File.ReadAllBytes($"DefaultTextures/spec {surface} {mip}.bin");
                mipmaps.Add(mipData);
            }
            surfaceData.Add(mipmaps);
        }
    }
}
