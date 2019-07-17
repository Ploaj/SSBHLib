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
            posX_tb.Text = camera.Translation.X.ToString();
            posY_tb.Text = camera.Translation.Y.ToString();
            posZ_tb.Text = camera.Translation.Z.ToString();
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
            if (!float.TryParse(posX_tb.Text, out x))
                posX_tb.Text = x.ToString();
            if (!float.TryParse(posY_tb.Text, out y))
                posY_tb.Text = y.ToString();
            if (!float.TryParse(posZ_tb.Text, out z))
                posZ_tb.Text = z.ToString();
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
