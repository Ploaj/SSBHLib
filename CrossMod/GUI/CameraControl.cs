using OpenTK;
using SFGraphics.Cameras;
using System;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    public partial class CameraControl : Form
    {
        private readonly Camera camera;

        public CameraControl(Camera camera)
        {
            InitializeComponent();
            this.camera = camera;
            transX_tb.Text = camera.Translation.X.ToString();
            transY_tb.Text = camera.Translation.Y.ToString();
            transZ_tb.Text = camera.Translation.Z.ToString();
            rotX_tb.Text = camera.RotationXDegrees.ToString();
            rotY_tb.Text = camera.RotationYDegrees.ToString();
        }

        private void update_button_Click(object sender, EventArgs e)
        {
            float x = camera.Translation.X;
            float y = camera.Translation.Y;
            float z = camera.Translation.Z;
            float rot_x = camera.RotationXDegrees;
            float rot_y = camera.RotationYDegrees;
            if (!float.TryParse(transX_tb.Text, out x))
                transX_tb.Text = x.ToString();
            if (!float.TryParse(transY_tb.Text, out y))
                transY_tb.Text = y.ToString();
            if (!float.TryParse(transZ_tb.Text, out z))
                transZ_tb.Text = z.ToString();
            if (!float.TryParse(rotX_tb.Text, out rot_x))
                rotX_tb.Text = rot_x.ToString();
            if (!float.TryParse(rotY_tb.Text, out rot_y))
                rotY_tb.Text = rot_y.ToString();
            
            camera.Translation = new Vector3(x, y, z);
            camera.RotationXDegrees = rot_x;
            camera.RotationYDegrees = rot_y;
        }
    }
}
