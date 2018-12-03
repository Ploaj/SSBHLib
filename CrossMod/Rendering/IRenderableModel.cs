using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossMod.Rendering
{
    public interface IRenderableModel : IRenderable
    {
        RModel GetModel();
        RSkeleton GetSkeleton();
        RTexture[] GetTextures();
    }
}
