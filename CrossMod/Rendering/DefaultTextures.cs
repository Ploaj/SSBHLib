using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.GLObjects.Textures;
using System.Drawing;

namespace CrossMod.Rendering
{
    public class DefaultTextures
    {
        public Texture2D defaultWhite = null;
        public Texture2D defaultNormal = null;

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
        }
    }
}
