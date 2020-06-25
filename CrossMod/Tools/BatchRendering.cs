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
        private static bool isRendering = false;

        public static void RenderModels(ModelViewport modelViewport, TreeView fileTree)
        {
            if (!FileTools.TryOpenFolderDialog(out string folderPath, "Select Source Directory"))
                return;

            if (!FileTools.TryOpenFolderDialog(out string outputPath, "Select PNG Output Directory"))
                return;

            modelViewport.BeginBatchRenderMode();
            fileTree.BeginUpdate();

            isRendering = true;
            var saveImages = Task.Run(SaveImagesFromQueue);

            foreach (var file in Directory.EnumerateFiles(folderPath, "*model.numdlb", SearchOption.AllDirectories))
            {
                string sourceFolder = Directory.GetParent(file).FullName;

                try
                {
                    var nodes = GetRenderableNodes(sourceFolder);
                    RenderModel(modelViewport, nodes);

                    // Screenshots will be saved and disposed later to improve performance.
                    imagesToSave.Enqueue(new SaveImageWorkItem(modelViewport.Renderer.GetScreenshot(), folderPath, file, outputPath));
                }
                catch (Exception)
                {
                    continue;
                }
                finally
                {
                    WorkSpaceTools.ClearWorkspace(fileTree, modelViewport);
                }

                System.Diagnostics.Debug.WriteLine($"Rendered {sourceFolder}");
            }

            isRendering = false;
            saveImages.Wait();

            fileTree.EndUpdate();
            modelViewport.EndBatchRenderMode();
        }

        private static void RenderModel(ModelViewport modelViewport, List<IRenderableNode> nodes)
        {
            if (nodes.Count == 0)
                return;

            var rnumdl = nodes[0] as Rnumdl;
            if (rnumdl?.Model != null)
                modelViewport.Renderer.Camera.FrameBoundingSphere(rnumdl.Model.BoundingSphere);

            modelViewport.Renderer.AddRenderableNode("model", nodes[0]);
            modelViewport.Renderer.RenderNodes(null);
            modelViewport.SwapBuffers();
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
            while (isRendering)
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
