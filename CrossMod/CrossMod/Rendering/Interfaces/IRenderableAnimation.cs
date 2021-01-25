
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public interface IRenderableAnimation
    {
        int GetFrameCount();

        void SetFrameSkeleton(IEnumerable<RBone> bones, float frame);

        void SetFrameModel(IEnumerable<Models.RMesh> subMeshes, float frame);
    }
}
