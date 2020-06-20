using CrossMod.GUI;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossMod.Tools
{
    public static class AnimationExport
    {
        public static async Task ConvertAnimationToGif(ModelViewport viewport)
        {
            if (!viewport.HasAnimation)
            {
                MessageBox.Show("Please open an animation file and select an animation.", "No animation selected");
                return;
            }

            FileTools.TryOpenSaveFileDialog(out string fileName, "GIF|*.gif", "animation");

            var progressViewer = new ProgressViewer { Text = "Processing GIF" };
            progressViewer.Show();
            var progress = new Progress<int>(percent =>
                progressViewer.SetProgress(percent));

            await viewport.RenderAnimationToGifAsync(fileName, progress);

            progressViewer.Close();
        }

        public static void ExportFramesToFolder(ModelViewport viewport)
        {
            if (!viewport.HasAnimation)
            {
                MessageBox.Show("Please open an animation file and select an animation.", "No animation selected");
                return;
            }

            FileTools.TryOpenFolderDialog(out string folderPath, "animation");

            var progressViewer = new ProgressViewer { Text = "Processing Animation" };
            progressViewer.Show();
            var progress = new Progress<int>(percent =>
                progressViewer.SetProgress(percent));

            viewport.RenderAnimationToFolder(folderPath);

            progressViewer.Close();
        }
    }
}
