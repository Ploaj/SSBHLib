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

        public static implicit operator CameraSettingsWindowViewModel(Camera rhs)
        {
            return new CameraSettingsWindowViewModel
            {
                PositionX = rhs.Translation.X,
                PositionY = rhs.Translation.Y,
                PositionZ = rhs.Translation.Z,
                RotationXDegrees = rhs.RotationXDegrees,
                RotationYDegrees = rhs.RotationYDegrees
            };
        }

        public void SetValues(Camera camera)
        {
            PositionX = camera.Translation.X;
            PositionY = camera.Translation.Y;
            PositionZ = camera.Translation.Z;
            RotationXDegrees = camera.RotationXDegrees;
            RotationYDegrees = camera.RotationYDegrees;
        }
    }
}
