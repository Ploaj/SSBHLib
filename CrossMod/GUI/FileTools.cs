using CrossMod.GUI;
using System.Windows.Forms;

namespace CrossMod
{
    public class FileTools
    {
        public static string TryOpenFolder()
        {
            FolderSelectDialog Dialog = new FolderSelectDialog();

            if (Dialog.ShowDialog() == DialogResult.OK)
            {
                return Dialog.SelectedPath;
            }

            return "";
        }

    }
}
