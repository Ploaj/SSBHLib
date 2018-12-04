
namespace CrossMod.Rendering
{
    public interface IRenderableAnimation : IRenderable
    {
        int GetFrameCount();

        void SetFrameSkeleton(RSkeleton Skeleton, float Frame);

        void SetFrameModel(RModel Model, float Frame);
    }
}
