using CrossMod.Nodes;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Textures;
using SFGraphics.GLObjects.Textures.TextureFormats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CrossMod.Rendering.Resources
{
    public class DefaultTextures
    {
        // Don't initialize yet because an OpenGL context may not be current.
        public static Lazy<DefaultTextures> Instance { get; } = new Lazy<DefaultTextures>(new DefaultTextures());

        // Default textures.
        public Texture2D DefaultWhite { get; } = new Texture2D();
        public Texture2D DefaultNormal { get; } = new Texture2D();
        public Texture2D DefaultBlack { get; } = new Texture2D();
        public Texture2D DefaultDiffuse2 { get; } = new Texture2D();
        public Texture2D DefaultGray { get; } = new Texture2D();
        public Texture2D DefaultMetallicBG { get; } = new Texture2D();
        public Texture2D DefaultSpecular { get; } = new Texture2D();
        public Texture3D ColorGradingLut { get; } = new Texture3D();


        /// <summary>
        /// /common/shader/sfxpbs/fighter/default_params or
        /// </summary>
        public Texture2D DefaultParamsFighter { get; } = new Texture2D();
        public Texture2D DefaultParams2 { get; } = new Texture2D();
        public Texture2D DefaultParams3 { get; } = new Texture2D();
        public Texture2D DefaultParamsR000G025B100 { get; } = new Texture2D();
        public Texture2D DefaultParamsR100G025B100 { get; } = new Texture2D();

        /// <summary>
        /// Similar to <see cref="DefaultParamsFighter"/> but specular is black.
        /// </summary>
        public Texture2D DefaultPrm { get; } = new Texture2D();


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

        private DefaultTextures()
        {
            // TODO: Check for a context.
            // The default cube map sampling has artifacts between cube map faces.
            GL.Enable(EnableCap.TextureCubeMapSeamless);

            LoadBitmap(UvPattern, "DefaultTextures/UVPattern.png");

            LoadBitmap(DefaultWhite, "DefaultTextures/default_white.png");
            LoadBitmap(DefaultParamsFighter, "DefaultTextures/default_params.tif");
            LoadBitmap(DefaultPrm, "DefaultTextures/default_prm.tif");
            LoadBitmap(DefaultNormal, "DefaultTextures/default_normal.tif");
            LoadBitmap(DefaultBlack, "DefaultTextures/default_black.png");

            Load3dCube();

            using (var bmp = new Bitmap("DefaultTextures/default_cube_black.png"))
                BlackCube.LoadImageData(bmp, 8);

            LoadDiffusePbr();

            var cube = SBSurface.FromNutexb("DefaultTextures/reflection_cubemap.nutexb");
            SpecularPbr = (TextureCubeMap)cube.GetRenderTexture();
        }

        private void Load3dCube()
        {
            ColorGradingLut.LoadImageData(16, 16, 16, File.ReadAllBytes("DefaultTextures/neutral.bin"), new TextureFormatUncompressed(PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte));

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
            var mipData = File.ReadAllBytes($"DefaultTextures/irr {surface}.bin");
            surfaceData.Add(new List<byte[]>() { mipData });
        }
    }
}
