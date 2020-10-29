using SFGraphics.Cameras;

namespace CrossModGui.ViewModels
{
    public class CameraSettingsWindowViewModel : ViewModelBase
    {
        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public float PositionZ { get; set; }

        public float RotationXDegrees { get; set; }

        public float RotationYDegrees { get; set; }

        public CameraSettingsWindowViewModel(Camera camera)
        {
            PositionX = camera.Translation.X;
            PositionY = camera.Translation.Y;
            PositionZ = camera.Translation.Z;
            RotationXDegrees = camera.RotationXDegrees;
            RotationYDegrees = camera.RotationYDegrees;
        }

        public void SetValues(Camera camera)
        {
            camera.Translation = new OpenTK.Vector3(PositionX, PositionY, PositionZ);
            camera.RotationXDegrees = RotationXDegrees;
            camera.RotationYDegrees = RotationYDegrees;
        }
    }
}
