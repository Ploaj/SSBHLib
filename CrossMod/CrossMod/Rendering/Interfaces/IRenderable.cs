using SFGraphics.Cameras;

namespace CrossMod.Rendering
{
    public interface IRenderable
    {
        void Render(OpenTK.Matrix4 modelView, OpenTK.Matrix4 projection);
    }
}
