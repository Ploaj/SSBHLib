namespace CrossMod.Rendering
{
    public interface IRenderableModel : IRenderable
    {
        Models.RModel? RenderModel { get; }
        RSkeleton? Skeleton { get; }
    }
}
