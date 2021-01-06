using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using OpenTK;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace CrossModGui.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public bool ShouldLoopAnimation { get; set; } = true;

        public float CurrentFrame
        {
            get => currentFrame;
            set
            {
                UpdateCurrentFrame(ref currentFrame, value);
                // Make sure the value is updated when starting playback.
                renderFrame = currentFrame;
            }
        }
        private float currentFrame;

        // The dispatching needed to update the viewmodel from the render thread messes with the framerate. 
        // Make another field and just updated the viewmodel when starting/stopping playback.
        private float renderFrame = 0;

        public float TotalFrames { get; set; }

        public ViewportRenderer Renderer { get; }

        public ObservableCollection<FileNode> FileTreeItems { get; } = new ObservableCollection<FileNode>();

        public ObservableCollection<BoneTreeItem> BoneTreeItems { get; } = new ObservableCollection<BoneTreeItem>();

        public ObservableCollection<MeshListItem> MeshListItems { get; } = new ObservableCollection<MeshListItem>();

        public ObservableCollection<Tuple<string, RNumdl>> Rnumdls { get; } = new ObservableCollection<Tuple<string, RNumdl>>();

        public bool IsPlayingAnimation
        {
            get => isPlayingAnimation;
            set
            {
                isPlayingAnimation = value;

                if (isPlayingAnimation)
                {
                    Renderer.RestartRendering();
                }
                else
                {
                    Renderer.PauseRendering();
                    // Update the values after stopping animation.
                    CurrentFrame = renderFrame;
                }
            }
        }
        private bool isPlayingAnimation;

        public MainWindowViewModel(ViewportRenderer renderer)
        {
            Renderer = renderer;
        }

        public void Clear()
        {
            // Pause the rendering thread before modifying any renderables.
            IsPlayingAnimation = false;

            FileTreeItems.Clear();
            BoneTreeItems.Clear();
            MeshListItems.Clear();
            Rnumdls.Clear();
        }

        public void PopulateFileTree(string folderPath, bool isRecursive)
        {
            var rootNode = new DirectoryNode(folderPath) { IsExpanded = true };
            FileTreeItems.Add(rootNode);

            // Use the existing collection when possible.
            var collection = (Renderer.ItemToRender as ModelCollection) ?? new ModelCollection();

            foreach (var child in rootNode.Nodes)
            {
                AddModelsToCollection(child, collection, isRecursive);
            }

            Renderer.ItemToRender = collection;
        }

        private void AddModelsToCollection(FileNode node, ModelCollection collection, bool isRecursive)
        {
            // TODO: There's probably a better way to avoid adding a numdlb twice.
            if (node is NumdlbNode numdlb && !numdlb.HasBeenAddedToCollection)
            {
                var rnumdl = numdlb.GetRenderableNode();

                if (rnumdl.RenderModel != null)
                {
                    collection.Meshes.AddRange(rnumdl.RenderModel.SubMeshes.Select(m => new Tuple<RMesh, RSkeleton?>(m, rnumdl.Skeleton)));
                    collection.AddBoundingSphere(rnumdl.RenderModel.BoundingSphere);
                    Renderer.Camera.FrameBoundingSphere(collection.BoundingSphere);
                }

                // The parent will be a folder and should have a more descriptive name.
                // Use model.numdlb as a fallback if there is no parent.
                var parentText = numdlb.Parent?.Text ?? numdlb.Text;
                AddMeshesToGui(parentText, rnumdl.RenderModel);
                AddSkeletonToGui(rnumdl.Skeleton);

                Rnumdls.Add(new Tuple<string, RNumdl>(parentText, rnumdl));

                numdlb.HasBeenAddedToCollection = true;
            }
            else if (isRecursive && node is DirectoryNode directory)
            {
                foreach (var child in directory.Nodes)
                {
                    AddModelsToCollection(child, collection, isRecursive);
                }
            }
        }

        public void UpdateBones(IRenderable newNode)
        {
            BoneTreeItems.Clear();

            if (newNode == null)
                return;

            // Duplicate nodes should still update the mesh list.
            if (newNode is RSkeleton skeleton)
            {
                AddSkeletonToGui(skeleton);
            }
            else if (newNode is RNumdl rnumdl)
            {
                AddSkeletonToGui(rnumdl.Skeleton);
            }
        }

        public void UpdateCurrentRenderableNode(FileNode item)
        {
            // Make sure GL objects are created on the UI thread.
            var wasRendering = Renderer.IsRendering;
            Renderer.PauseRendering();

            // Pause animation playback until the new animation is loaded.
            var wasPlaying = IsPlayingAnimation;
            IsPlayingAnimation = false;

            if (item is NumdlbNode numdlb && Renderer.ItemToRender is ModelCollection collection)
                AddModelsToCollection(numdlb, collection, false);

            UpdateRendererItems(item);

            if (wasRendering)
                Renderer.RestartRendering();

            if (wasPlaying)
                IsPlayingAnimation = true;
        }

        private void UpdateRendererItems(FileNode item)
        {
            ResetAnimation();

            // Preserve the existing model collection when drawing individual items.
            // Textures and bones will override the model collection.
            // Updating the animation shouldn't clear the current renderable.
            if (item is IRenderableNode renderableNode && !(item is NumdlbNode))
            {
                Renderer.ItemToRenderOverride = renderableNode.Renderable.Value;
            }
            else if (item is NuanmbNode animation)
            {
                Renderer.RenderableAnimation = animation.GetRenderableAnimation();
                TotalFrames = Renderer.RenderableAnimation.GetFrameCount();
                if (Renderer.ScriptNode != null)
                    Renderer.ScriptNode.CurrentAnimationName = animation.Text;
            }
            else
            {
                Renderer.ItemToRenderOverride = null;
            }

            // TODO: ScriptNode.MotionRate?

            // TODO: The script node should probably be stored with the model somehow.
            // Having to find the node each time seems redundant.
            var scriptNode = FindSiblingOfType<ScriptNode>(item);
            if (scriptNode != null)
            {
                // Load hitboxes.
                var skelNode = FindSiblingOfType<NusktbNode>(item);
                if (skelNode != null)
                {
                    scriptNode.SkelNode = skelNode;
                    Renderer.ScriptNode = scriptNode;
                    ParamNodeContainer.SkelNode = skelNode;
                }
            }
        }

        private static T? FindSiblingOfType<T>(FileNode item) where T : FileNode
        {
            return item?
                .Parent?
                .Nodes
                .OfType<T>()
                .FirstOrDefault();
        }

        public void RenderNodes()
        {
            Renderer.RenderNodes(renderFrame);
            if (IsPlayingAnimation)
            {
                UpdateCurrentFrame(ref renderFrame, renderFrame + 1);
            }
        }

        private void ResetAnimation()
        {
            Renderer.RenderableAnimation = null;
            CurrentFrame = 0;
            TotalFrames = 0;
        }

        private void AddSkeletonToGui(RSkeleton? skeleton)
        {
            if (skeleton == null)
                return;

            var rootLevelNodes = CreateBoneTreeGetRootLevel(skeleton.Bones);
            foreach (var item in rootLevelNodes)
                BoneTreeItems.Add(item);
        }

        private void AddMeshesToGui(string rootName, RModel? model)
        {
            if (model == null)
                return;

            // Make fighters less painful to look at.
            // No one wants to see all those faces at once.
            // TODO: This should work differently for stages.
            model.HideExpressionMeshes();

            var parent = new MeshListItem
            {
                Name = rootName,
                IsChecked = true,
                IsExpanded = true
            };

            foreach (var mesh in model.SubMeshes)
            {
                var newItem = new MeshListItem
                {
                    Name = mesh.Name,
                    IsChecked = mesh.IsVisible
                };

                // Sync in both directions to support expression hiding.
                newItem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MeshListItem.IsChecked))
                    {
                        mesh.IsVisible = newItem.IsChecked;
                    }
                };
                mesh.VisibilityChanged += (s, e) => newItem.IsChecked = e;

                parent.Children.Add(newItem);
            }

            MeshListItems.Add(parent);
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

        // TODO: Move frame logic to another class.
        private void UpdateCurrentFrame(ref float currentValue, float newValue)
        {
            if (newValue < 0)
            {
                currentValue = 0;
            }
            else if (newValue > TotalFrames)
            {
                if (ShouldLoopAnimation)
                {
                    currentValue = 0;
                }
                else
                {
                    currentValue = TotalFrames;
                    IsPlayingAnimation = false;
                }
            }
            else
            {
                currentValue = newValue;
            }

            if (Renderer.ScriptNode != null)
            {
                // Workaround for not being able to play scripts backwards.
                // TODO: The redundant string parsing in the script node makes this slow.
                // Most of the parsing could be cached.
                Renderer.ScriptNode.Start();
                for (int i = 0; i < CurrentFrame; i++)
                {
                    Renderer.ScriptNode.Update(i);
                }
            }
        }
    }
}
