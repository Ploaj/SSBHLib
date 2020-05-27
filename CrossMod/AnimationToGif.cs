using CrossMod.GUI;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossMod
{
    public static class AnimationToGif
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
    }
}
