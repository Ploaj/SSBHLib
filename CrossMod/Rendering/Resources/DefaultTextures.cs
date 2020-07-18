using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Textures;
using System.Collections.Generic;
using System.Drawing;
using SFGraphics.GLObjects.Textures.TextureFormats;

namespace CrossMod.Rendering.Resources
{
    public class DefaultTextures
    {
        // Don't initialize yet because an OpenGL context may not be current.
        public static DefaultTextures Instance { get; private set; }

        // Default textures.
        public Texture2D DefaultWhite { get; } = new Texture2D();
        public Texture2D DefaultNormal { get; } = new Texture2D();
        public Texture2D DefaultBlack { get; } = new Texture2D();
        public Texture2D DefaultParams { get; } = new Texture2D();

        // Render modes.
        public Texture2D UvPattern { get; } = new Texture2D()
        {
            TextureWrapS = TextureWrapMode.Repeat,
            TextureWrapT = TextureWrapMode.Repeat
        };

        // PBR image based lighting.
        public TextureCubeMap DiffusePbr { get; } = new TextureCubeMap();
        public TextureCubeMap SpecularPbr { get; } = new TextureCubeMap();
        public TextureCubeMap BlackCube { get; } = new TextureCubeMap();

        public static void Initialize()
        {
            Instance = new DefaultTextures();
        }

        private DefaultTextures()
        {
            LoadBitmap(UvPattern, "DefaultTextures/UVPattern.png");

            LoadBitmap(DefaultWhite, "DefaultTextures/default_white.png");
            LoadBitmap(DefaultParams, "DefaultTextures/default_Params.tif");
            LoadBitmap(DefaultNormal, "DefaultTextures/default_normal.tif");
            LoadBitmap(DefaultBlack, "DefaultTextures/default_black.png");

            using (var bmp = new Bitmap("DefaultTextures/default_cube_black.png"))
                BlackCube.LoadImageData(bmp, 8);

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
            var surfaceData = new List<List<byte[]>>();

            AddIrrFace(surfaceData, "x+");
            AddIrrFace(surfaceData, "x-");
            AddIrrFace(surfaceData, "y+");
            AddIrrFace(surfaceData, "y-");
            AddIrrFace(surfaceData, "z+");
            AddIrrFace(surfaceData, "z-");


            var format = new TextureFormatUncompressed(PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            DiffusePbr.LoadImageData(64, format, surfaceData[0], surfaceData[1], surfaceData[2], surfaceData[3], surfaceData[4], surfaceData[5]);

            // Don't Use mipmaps.
            DiffusePbr.MagFilter = TextureMagFilter.Linear;
            DiffusePbr.MinFilter = TextureMinFilter.Linear;
        }

        private static void AddIrrFace(List<List<byte[]>> surfaceData, string surface)
        {
            var mipData = System.IO.File.ReadAllBytes($"DefaultTextures/irr {surface}.bin");
            surfaceData.Add(new List<byte[]>() { mipData });
        }

        private void LoadSpecularPbr()
        {
            var surfaceData = new List<List<byte[]>>();

            AddCubeMipmaps(surfaceData, "x+");
            AddCubeMipmaps(surfaceData, "x-");
            AddCubeMipmaps(surfaceData, "y+");
            AddCubeMipmaps(surfaceData, "y-");
            AddCubeMipmaps(surfaceData, "z+");
            AddCubeMipmaps(surfaceData, "z-");

            var format = new TextureFormatUncompressed(PixelInternalFormat.Rgba32f, PixelFormat.Rgba, PixelType.Float);
            SpecularPbr.LoadImageData(64, format, surfaceData[0], surfaceData[1], surfaceData[2], surfaceData[3], surfaceData[4], surfaceData[5]);
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
