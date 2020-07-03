using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using SFGraphics.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

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
            get => currentFrame;
            set 
            {
                if (value < 0)
                    currentFrame = 0;
                else if (value > TotalFrames)
                    currentFrame = TotalFrames;
                else
                    currentFrame = value; 
            }
        }
        private float currentFrame;

        public float TotalFrames { get; set; }

        // TODO: This doesn't need to be public or set more than once.
        public ViewportRenderer Renderer { get; set; }

        public ObservableCollection<FileNode> FileTreeItems { get; } = new ObservableCollection<FileNode>();

        public ObservableCollection<BoneTreeItem> BoneTreeItems { get; } = new ObservableCollection<BoneTreeItem>();

        public ObservableCollection<MeshListItem> MeshListItems { get; } = new ObservableCollection<MeshListItem>();

        public bool IsPlayingAnimation
        {
            get => isPlayingAnimation;
            set 
            { 
                isPlayingAnimation = value;
                Renderer.IsPlayingAnimation = value;
            }
        }
        private bool isPlayingAnimation;

        public string PlayAnimationText => IsPlayingAnimation ? "Pause" : "Play";

        public void Clear()
        {
            FileTreeItems.Clear();
            BoneTreeItems.Clear();
            MeshListItems.Clear();
        }

        public void PopulateFileTree(string folderPath)
        {
            var rootNode = new DirectoryNode(folderPath);

            rootNode.Open();
            rootNode.OpenFileNodes();

            FileTreeItems.Clear();
            FileTreeItems.Add(rootNode);
        }

        public void UpdateMeshesAndBones(IRenderable newNode)
        {
            MeshListItems.Clear();
            BoneTreeItems.Clear();

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
            // Make sure GL objects are created on the UI thread.
            var wasRendering = Renderer.IsRendering;
            Renderer.PauseRendering();

            ResetAnimation();

            if (item is IRenderableNode node)
            {
                UpdateMeshesAndBones(node.GetRenderableNode());
                Renderer.ItemToRender = node.GetRenderableNode();
            }

            if (item is NuanimNode animation)
            {
                Renderer.RenderableAnimation = animation.GetRenderableAnimation();
                TotalFrames = Renderer.RenderableAnimation.GetFrameCount();
            }

            if (wasRendering)
                Renderer.RestartRendering();
        }

        public void RenderNodes()
        {
            Renderer.RenderNodes(null, CurrentFrame);
            if (IsPlayingAnimation)
            {
                CurrentFrame++;
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

            var rootLevelNodes = CreateBoneTreeGetRootLevel(skeleton.Bones);
            foreach (var item in rootLevelNodes)
                BoneTreeItems.Add(item);
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

        private static IEnumerable<BoneTreeItem> CreateBoneTreeGetRootLevel(IEnumerable<RBone> bones)
        {
            var boneItemById = bones.ToDictionary(b => b.Id, b => new BoneTreeItem { Name = b.Name });

            var rootLevel = new List<BoneTreeItem>();

            // Add each bone to its parent.
            // The root nodes will have a parent index of -1. 
            foreach (var bone in bones)
            {
                if (bone.ParentId != -1)
                    boneItemById[bone.ParentId].Children.Add(boneItemById[bone.Id]);
                else
                    rootLevel.Add(boneItemById[bone.Id]);
            }

            return rootLevel;
        }
    }
}
