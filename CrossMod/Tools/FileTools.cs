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

            Dialog.Dispose();

            return "";
        }
        
        public static bool TryOpenFile(out string FileName, string Filter = "")
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Filter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = dialog.FileName;
                    return true;
                }
            }
            FileName = "";
            return false;
        }
        
        public static bool TrySaveFile(out string FileName, string Filter = "")
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = Filter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = dialog.FileName;
                    return true;
                }
            }
            FileName = "";
            return false;
        }
    }
}
