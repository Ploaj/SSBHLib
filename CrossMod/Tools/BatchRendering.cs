using CrossMod.GUI;
using System;
using System.IO;
using System.Windows.Forms;

namespace CrossMod.Tools
{
    public static class BatchRendering
    {
        public static void RenderModels(ModelViewport modelViewport, TreeView fileTree)
        {
            if (!FileTools.TryOpenFolderDialog(out string folderPath, "Select Source Directory"))
                return;

            if (!FileTools.TryOpenFolderDialog(out string outputPath, "Select PNG Output Directory"))
                return;

            modelViewport.BeginBatchRenderMode();
            fileTree.BeginUpdate();

            foreach (var file in Directory.EnumerateFiles(folderPath, "*model.numdlb", SearchOption.AllDirectories))
            {
                string sourceFolder = Directory.GetParent(file).FullName;

                try
                {
                    WorkSpaceTools.LoadWorkspace(fileTree, modelViewport, sourceFolder);
                    modelViewport.RenderFrame();

                    // Save screenshot.
                    using (var bmp = modelViewport.GetScreenshot())
                    {
                        string condensedName = GetCondensedPathName(folderPath, file);
                        bmp.Save(Path.Combine(outputPath, $"{condensedName}.png"));
                    }
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

            fileTree.EndUpdate();
            modelViewport.EndBatchRenderMode();
        }

        private static string GetCondensedPathName(string folderPath, string file)
        {
            string condensedName = file.Replace(folderPath, "");
            condensedName = condensedName.Replace(Path.DirectorySeparatorChar, '_');
            condensedName = condensedName.Substring(1); // remove leading underscore
            return condensedName;
        }
    }
}
