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

        public bool ShouldLoopAnimation { get; set; } = true;

        public float CurrentFrame
        {
            get => currentFrame;
            set 
            {
                if (value < 0)
                {
                    currentFrame = 0;
                }
                else if (value > TotalFrames)
                {
                    if (ShouldLoopAnimation)
                    {
                        currentFrame = 0;
                    }
                    else
                    {
                        currentFrame = TotalFrames;
                        IsPlayingAnimation = false;
                    }
                }
                else
                {
                    currentFrame = value; 
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

        private float currentFrame;

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
                Renderer.IsPlayingAnimation = value;
            }
        }
        private bool isPlayingAnimation;

        public string PlayAnimationText => IsPlayingAnimation ? "Pause" : "Play";

        // TODO: Where to store this value?
        public RNumdl RNumdl { get; set; }

        public MainWindowViewModel(ViewportRenderer renderer)
        {
            Renderer = renderer;
        }

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
                RNumdl = renderableModel as RNumdl;
                AddMeshesToGui(renderableModel.GetModel());
                AddSkeletonToGui(renderableModel.GetSkeleton());
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
            Renderer.RenderNodes(CurrentFrame);
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

            foreach (var mesh in model.SubMeshes)
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
