using CrossMod.GUI;
using CrossMod.Nodes;
using System.Windows.Forms;

namespace CrossMod.Tools
{
    public static class WorkSpaceTools
    {

        /// <summary>
        /// Loads a directory and all sub-directories into the filetree.
        /// </summary>
        /// <param name="fileTree"></param>
        /// <param name="modelViewport"></param>
        /// <param name="folderPath"></param>
        public static void LoadWorkspace(TreeView fileTree, ModelViewport modelViewport, string folderPath)
        {
            var mainNode = CreateDirectoryNodeAndExpand(folderPath);
            AssignNodesAndSelectNumdlb(fileTree, modelViewport, mainNode);
        }

        public static void ClearWorkspace(TreeView fileTree, ModelViewport viewport)
        {
            fileTree.Nodes.Clear();
            ParamNodeContainer.Unload();
            viewport.ClearFiles();
        }
        public static DirectoryNode CreateDirectoryNodeAndExpand(string folderPath)
        {
            // Create the node and add child nodes.
            var mainNode = new DirectoryNode(folderPath);
            mainNode.Open();
            mainNode.Expand();
            return mainNode;
        }

        private static void AssignNodesAndSelectNumdlb(TreeView fileTree, ModelViewport modelViewport, DirectoryNode mainNode)
        {
            // Enable rendering of the model if we have directly selected a model file.
            // Nested ones won't render a model
            fileTree.Nodes.Add(mainNode);
            SkelNode skelNode = null;
            foreach (FileNode node in mainNode.Nodes)
            {
                if (node.Text.EndsWith("numdlb"))
                {
                    fileTree.SelectedNode = node;
                    modelViewport.HideExpressionMeshes();
                }
                else if (skelNode == null && node is SkelNode)
                {
                    skelNode = node as SkelNode;
                }
            }

            if (skelNode == null)
                return;
            foreach (FileNode node in mainNode.Nodes)
            {
                if (node is ScriptNode scriptNode)
                {
                    scriptNode.SkelNode = skelNode;
                    modelViewport.ScriptNode = scriptNode;
                    //only do this once, there should only be one anyway
                    break;
                }
            }

            ParamNodeContainer.SkelNode = skelNode;
        }
    }
}
