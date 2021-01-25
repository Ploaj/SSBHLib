using CrossMod.Nodes;

namespace CrossMod.Tools
{
    public static class WorkSpaceTools
    {
        public static DirectoryNode CreateDirectoryNodeAndExpand(string folderPath)
        {
            // Create the node and add child nodes.
            var mainNode = new DirectoryNode(folderPath)
            {
                IsExpanded = true
            };
            return mainNode;
        }
    }
}
