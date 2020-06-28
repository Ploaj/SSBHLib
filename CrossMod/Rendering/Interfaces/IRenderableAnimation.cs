
namespace CrossMod.Rendering
{
    public interface IRenderableAnimation
    {
        int GetFrameCount();

        void SetFrameSkeleton(RSkeleton Skeleton, float Frame);

        void SetFrameModel(Models.RModel Model, float Frame);
    }
}
