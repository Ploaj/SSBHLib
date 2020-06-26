using CrossMod.GUI;
using CrossMod.Nodes;
using CrossMod.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossMod.Tools
{
    public static class BatchRendering
    {
        private class SaveImageWorkItem
        {
            public Image Image { get; }

            public string FolderPath { get; }

            public string FileName { get; }

            public string OutputPath { get; }

            public SaveImageWorkItem(Image image, string folderPath, string fileName, string outputPath)
            {
                Image = image;
                FolderPath = folderPath;
                FileName = fileName;
                OutputPath = outputPath;
            }
        }

        private static readonly ConcurrentQueue<SaveImageWorkItem> imagesToSave = new ConcurrentQueue<SaveImageWorkItem>();
        private static bool isBatchRendering = false;

        public static void RenderModels(ViewportRenderer renderer)
        {
            if (!FileTools.TryOpenFolderDialog(out string folderPath, "Select Source Directory"))
                return;

            if (!FileTools.TryOpenFolderDialog(out string outputPath, "Select PNG Output Directory"))
                return;

            var viewportWasRendering = renderer.IsRendering;
            renderer.PauseRendering();

            isBatchRendering = true;
            var saveImages = Task.Run(SaveImagesFromQueue);

            foreach (var file in Directory.EnumerateFiles(folderPath, "*model.numdlb", SearchOption.AllDirectories))
            {
                string sourceFolder = Directory.GetParent(file).FullName;

                try
                {
                    var nodes = GetRenderableNodes(sourceFolder);
                    RenderModel(renderer, nodes);

                    // Screenshots will be saved and disposed later to improve performance.
                    imagesToSave.Enqueue(new SaveImageWorkItem(renderer.GetScreenshot(), folderPath, file, outputPath));
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    renderer.ClearRenderableNodes();
                }

                System.Diagnostics.Debug.WriteLine($"Rendered {sourceFolder}");
            }

            isBatchRendering = false;
            saveImages.Wait();

            if (viewportWasRendering)
                renderer.RestartRendering();
        }

        private static void RenderModel(ViewportRenderer renderer, List<IRenderableNode> nodes)
        {
            if (nodes.Count == 0)
                return;

            var rnumdl = nodes[0] as RNumdl;
            if (rnumdl?.Model != null)
                renderer.Camera.FrameBoundingSphere(rnumdl.Model.BoundingSphere);

            renderer.ItemToRender = nodes[0].GetRenderableNode();
            renderer.RenderNodes(null);
            renderer.SwapBuffers();
        }

        private static List<IRenderableNode> GetRenderableNodes(string sourceFolder)
        {
            // Loading the numdlb node will load the texture as well.
            var mainNode = WorkSpaceTools.CreateDirectoryNodeAndExpand(sourceFolder);
            mainNode.OpenChildNodes();

            foreach (FileNode node in mainNode.Nodes)
            {
                if (node.Text.EndsWith("numdlb"))
                {
                    var renderable = (node as NumdlNode);
                    if (renderable != null)
                        return new List<IRenderableNode>() { renderable };
                }
            }

            return null;
        }

        private static string GetCondensedPathName(string folderPath, string file)
        {
            string condensedName = file.Replace(folderPath, "");
            condensedName = condensedName.Replace(Path.DirectorySeparatorChar, '_');
            condensedName = condensedName.Substring(1); // remove leading underscore
            return condensedName;
        }

        private static void SaveImagesFromQueue()
        {
            while (isBatchRendering)
            {
                if (imagesToSave.TryDequeue(out SaveImageWorkItem item))
                {
                    string condensedName = GetCondensedPathName(item.FolderPath, item.FileName);
                    item.Image.Save(Path.Combine(item.OutputPath, $"{condensedName}.png"));
                    item.Image.Dispose();
                }
            }
        }
    }
}
