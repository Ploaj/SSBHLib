using System;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    public partial class PseudocodeBox : Form
    {
        public static string Code = "";

        public PseudocodeBox()
        {
            InitializeComponent();
            richTextBox1.Text = Code;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Code = richTextBox1.Text;
            ModelViewport.SetCode(Code);
        }
    }
}
