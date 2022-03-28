
using OpenTK;
using SFGraphics.Cameras;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public interface IRenderableAnimation
    {
        int GetFrameCount();

        void SetFrameSkeleton(IEnumerable<RBone> bones, float frame);

        void SetFrameModel(IEnumerable<Models.RMesh> subMeshes, float frame);

        void SetCameraTransforms(float frame, ref Matrix4 modelView, ref Matrix4 projection, float aspectRatio);
    }
}
