namespace CrossMod.Rendering
{
    public interface IRenderableModel : IRenderable
    {
        Models.RModel GetModel();
        RSkeleton GetSkeleton();
        RTexture[] GetTextures();
    }
}
