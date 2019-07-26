using System.Windows.Forms;

namespace CrossMod.GUI
{
    public partial class ProgressViewer : Form
    {
        public ProgressViewer()
        {
            InitializeComponent();
        }

        public void SetProgress(int value)
        {
            progressBar1.Value = value;
        }
    }
}
