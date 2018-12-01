using CrossMod.GUI;

namespace CrossMod
{
    public class FileTools
    {
        public static string TryOpenFolder()
        {
            OpenFolderDialog Dialog = new OpenFolderDialog();

            if ((bool)Dialog.ShowDialog())
            {
                return Dialog.Folder;
            }

            return "";
        }

    }
}
