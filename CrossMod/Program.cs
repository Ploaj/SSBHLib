using System;
using System.Threading;
using System.Windows.Forms;

namespace CrossMod
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
                Application.Run(new MainForm());
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error.ToString());
                MessageBox.Show(error.Message, "Error while running");
            }
        }
    }
}
