using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CrossModGui.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public class BoneTreeItem : ViewModelBase
        {
            public string Name { get; set; }

            public List<BoneTreeItem> Children { get; set;  } = new List<BoneTreeItem>();
        }

        public class MeshListItem : ViewModelBase
        {
            public string Name { get; set; }

            public bool IsChecked { get; set; }
        }

        public float CurrentFrame
        {
            get => Renderer.CurrentFrame;
            set 
            {
                if (value < 0)
                    Renderer.CurrentFrame = 0;
                else if (value > TotalFrames)
                    Renderer.CurrentFrame = TotalFrames;
                else
                    Renderer.CurrentFrame = value; 
            }
        }

        public float TotalFrames { get; set; }

        public ViewportRenderer Renderer { get; set; }

        public ObservableCollection<FileNode> FileTreeItems { get; } = new ObservableCollection<FileNode>();

        public ObservableCollection<BoneTreeItem> BoneTreeItems { get; } = new ObservableCollection<BoneTreeItem>();

        public ObservableCollection<MeshListItem> MeshListItems { get; } = new ObservableCollection<MeshListItem>();

        public bool IsPlayingAnimation { get; set; }

        public string PlayAnimationText => IsPlayingAnimation ? "Pause" : "Play";

        public void Clear()
        {
            FileTreeItems.Clear();
            BoneTreeItems.Clear();
            MeshListItems.Clear();
        }

        public void PopulateFileTree(string folderPath)
        {
            // TODO: Populate subnodes after expanding the directory node.
            var rootNode = new DirectoryNode(folderPath);

            // TODO: Combine these two methods?
            rootNode.Open();
            rootNode.OpenChildNodes();

            FileTreeItems.Clear();
            FileTreeItems.Add(rootNode);
        }

        public void UpdateMeshesAndBones(IRenderable newNode)
        {
            if (newNode == null)
                return;

            // Duplicate nodes should still update the mesh list.
            if (newNode is RSkeleton skeleton)
            {
                AddSkeletonToGui(skeleton);
            }
            else if (newNode is IRenderableModel renderableModel)
            {
                AddMeshesToGui(renderableModel.GetModel());
                AddSkeletonToGui(renderableModel.GetSkeleton());
            }
        }

        public void UpdateCurrentRenderableNode(FileNode item)
        {
            BoneTreeItems.Clear();
            MeshListItems.Clear();
            ResetAnimation();

            if (item is IRenderableNode node)
            {
                UpdateMeshesAndBones(node.GetRenderableNode());
                Renderer.ItemToRender = node.GetRenderableNode();
            }

            if (item is NuanimNode animation)
            {
                Renderer.RenderableAnimation = (IRenderableAnimation)animation.GetRenderableNode();
                TotalFrames = Renderer.RenderableAnimation.GetFrameCount();
            }
        }

        private void ResetAnimation()
        {
            Renderer.RenderableAnimation = null;
            CurrentFrame = 0;
            TotalFrames = 0;
        }

        private void AddSkeletonToGui(RSkeleton skeleton)
        {
            if (skeleton == null)
                return;

            var root = CreateBoneTreeGetRoot(skeleton.Bones);
            BoneTreeItems.Add(root);
        }

        private void AddMeshesToGui(RModel model)
        {
            if (model == null)
                return;

            foreach (var mesh in model.subMeshes)
            {
                var newItem = new MeshListItem
                {
                    Name = mesh.Name,
                    IsChecked = mesh.Visible
                };
                // TODO: Sync in the other direction to support animations/expression hiding?
                newItem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MeshListItem.IsChecked))
                        mesh.Visible = newItem.IsChecked;
                    else if (e.PropertyName == nameof(MeshListItem.Name))
                        mesh.Name = newItem.Name;
                };

                MeshListItems.Add(newItem);
            }
        }

        private static BoneTreeItem CreateBoneTreeGetRoot(IEnumerable<RBone> bones)
        {
            // The root bone has no parent.
            var boneItemById = bones.ToDictionary(b => b.Id, b => new BoneTreeItem { Name = b.Name });

            // Add each bone to its parent.
            BoneTreeItem root = null;
            foreach (var bone in bones)
            {
                if (bone.ParentId != -1)
                    boneItemById[bone.ParentId].Children.Add(boneItemById[bone.Id]);
                else
                    root = boneItemById[bone.Id];
            }

            return root;
        }
    }
}
