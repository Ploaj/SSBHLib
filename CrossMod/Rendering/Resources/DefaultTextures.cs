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
        public Texture2D iblLut = new Texture2D();
        public TextureCubeMap diffusePbr = new TextureCubeMap();
        public TextureCubeMap specularPbr = new TextureCubeMap();
        public TextureCubeMap blackCube = new TextureCubeMap();

        public Texture2D stipplePattern = new Texture2D();

        public DefaultTextures()
        {
            LoadStipplePattern();

            LoadBitmap(uvPattern, "DefaultTextures/UVPattern.png");

            LoadBitmap(defaultWhite, "DefaultTextures/default_White.png");
            LoadBitmap(defaultPrm, "DefaultTextures/default_Params.tif");
            LoadBitmap(defaultNormal, "DefaultTextures/default_normal.tif");
            LoadBitmap(defaultBlack, "DefaultTextures/default_black.png");

            LoadBitmap(iblLut, "DefaultTextures/ibl_brdf_lut.png");

            using (var bmp = new Bitmap("DefaultTextures/default_cube_black.png"))
                blackCube.LoadImageData(bmp, 8);

            LoadDiffusePbr();
            LoadSpecularPbr();
        }

        private void LoadStipplePattern()
        {
            // Matrix for ordered dithering.
            // https://community.khronos.org/t/screen-door-transparency/621/5
            var values = new float[256]
            {
                192, 11,183,125, 26,145, 44,244,  8,168,139, 38,174, 27,141, 43,
                115,211,150, 68,194, 88,177,131, 61,222, 87,238, 74,224,100,235,
                 59, 33, 96,239, 51,232, 16,210,117, 32,187,  1,157,121, 14,165,
                248,128,217,  2,163,105,154, 81,247,149, 97,205, 52,182,209, 84,
                 20,172, 80,140,202, 41,185, 55, 24,197, 65,129,252, 35, 70,147,
                201, 63,189, 28, 90,254,116,219,137,107,231, 17,144,119,228,109,
                 46,245,103,229,134, 13, 67,162,  6,170, 47,178, 76,193,  4,167,
                133,  9,159, 54,175,124,225, 93,242, 79,214, 99,241, 56,221, 92,
                186,218, 78,208, 37,196, 25,188, 42,142, 29,158, 21,130,156, 40,
                102, 31,148,111,234, 85,151,120,207,113,255, 86,184,212, 69,236,
                176, 73,253,  0,138, 58,249, 71, 10,173, 62,200, 50,114, 12,123,
                 23,204,118,191, 91,181, 19,164,216,101,233,  3,135,169,246,152,
                223, 60,143, 48,240, 34,220, 82,132, 36,146,106,227, 30, 95, 49,
                 83,166, 18,199, 98,155,122, 53,237,179, 57,190, 77,195,127,180,
                230,108,215, 64,171,  5,206,161, 22, 94,251, 15,153, 45,243,  7,
                 72,136, 39,250,104,226, 75,112,198,126, 66,213,110,203, 89,160
            };
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = values[i] / 256.0f;
            }
            stipplePattern.LoadImageData(16, 16, values, new TextureFormatUncompressed(PixelInternalFormat.R8, PixelFormat.Red, PixelType.Float));
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
