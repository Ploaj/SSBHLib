using System.Windows.Forms;

namespace CrossModGui.Tools
{
    public class FileTools
    {
        public static bool TryOpenFolderDialog(out string folderName, string title = "")
        {
            using (var dialog = new FolderSelectDialog())
            {
                if (!string.IsNullOrEmpty(title))
                    dialog.Title = title;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    folderName = dialog.SelectedPath;
                    return true;
                }

            }

            folderName = "";
            return false;
        }
        
        public static bool TryOpenFileDialog(out string fileName, string filter = "")
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = filter;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = dialog.FileName;
                    return true;
                }
            }
            fileName = "";
            return false;
        }
        
        public static bool TryOpenSaveFileDialog(out string fileName, string filter = "", string defaultFileName = "")
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = filter;
                dialog.FileName = defaultFileName;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = dialog.FileName;
                    return true;
                }
            }
            fileName = "";
            return false;
        }
    }
}
