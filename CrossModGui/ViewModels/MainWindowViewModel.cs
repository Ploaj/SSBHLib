using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

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

        // TODO: Where to store this value?
        public RNumdl RNumdl { get; set; }

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
                RNumdl = renderableModel as RNumdl;
                AddMeshesToGui(renderableModel.RenderModel);
                AddSkeletonToGui(renderableModel.Skeleton);
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

            ResetAnimation();

            if (item is IRenderableNode renderableNode)
            {
                UpdateMeshesAndBones(renderableNode.GetRenderableNode());
                Renderer.ItemToRender = renderableNode.GetRenderableNode();
            }
            else if (item is NuanimNode animation)
            {
                Renderer.RenderableAnimation = animation.GetRenderableAnimation();
                TotalFrames = Renderer.RenderableAnimation.GetFrameCount();
                if (Renderer.ScriptNode != null)
                    Renderer.ScriptNode.CurrentAnimationName = animation.Text;
            }

            // TODO: ScriptNode.MotionRate?

             // TODO: The script node should probably be stored with the model somehow.
             // Having to find the node each time seems redundant.
            var scriptNode = FindSiblingOfType<ScriptNode>(item);
            if (scriptNode != null)
            {
                // Load hitboxes.
                var skelNode = FindSiblingOfType<SkelNode>(item);
                if (skelNode != null)
                {
                    scriptNode.SkelNode = skelNode;
                    Renderer.ScriptNode = scriptNode;
                    ParamNodeContainer.SkelNode = skelNode;
                }
            }

            if (wasRendering)
                Renderer.RestartRendering();

            if (wasPlaying)
                IsPlayingAnimation = true;
        }

        private static T FindSiblingOfType<T>(FileNode item) where T : FileNode
        {
            if (item == null || item.Parent == null)
                return null;

            foreach (var node in item.Parent.Nodes)
            {
                if (node is T tNode)
                {
                    return tNode;
                }
            }

            return null;
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

            // Make fighters less painful to look at.
            // No one wants to see all those faces at once.
            model.HideExpressionMeshes();

            foreach (var mesh in model.SubMeshes)
            {
                var newItem = new MeshListItem
                {
                    Name = mesh.Name,
                    IsChecked = mesh.Visible
                };

                // Sync in both directions to support expression hiding.
                newItem.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(MeshListItem.IsChecked))
                        mesh.Visible = newItem.IsChecked;
                    else if (e.PropertyName == nameof(MeshListItem.Name))
                        mesh.Name = newItem.Name;
                };
                mesh.VisibilityChanged += (s, e) => newItem.IsChecked = e;

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
