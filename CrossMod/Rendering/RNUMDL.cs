using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.Cameras;

namespace CrossMod.Rendering
{
    public class RNUMDL : IRenderable
    {

        public Dictionary<string, RTexture> TextureBank = new Dictionary<string, RTexture>();
        public RSkeleton Skeleton;
        public RModel Model;

        public void Render(Camera Camera)
        {
            if (Model != null)
            {
                Model.Render(Camera, Skeleton);
            }

            //Render Skeleton with no depth buffer
        }
    }
}
