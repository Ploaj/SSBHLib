using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFGraphics.GLObjects.Shaders;
using SFGraphics.Cameras;

namespace CrossMod.Rendering
{
    public interface IRenderable
    {
        void Render(Camera Camera);
    }
}
