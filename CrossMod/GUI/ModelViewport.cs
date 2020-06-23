using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Rendering.Models;
using SFGraphics.GLObjects.Framebuffers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        public ViewportRenderer Renderer { get; }

        private AnimationBar animationBar;

        public bool HasAnimation => animationBar.Animation != null;
        public IRenderableAnimation RenderableAnimation
        {
            set
            {
                animationBar.Animation = value;
                controlBox.Visible = true;
            }
        }

        public ScriptNode ScriptNode
        {
            get => animationBar.ScriptNode;
            set => animationBar.ScriptNode = value;
        }

        public ModelViewport()
        {
            InitializeComponent();
            Renderer = new ViewportRenderer(glViewport);
            AddAnimationBar();
            CreateRenderFrameEvents();

            glViewport.MouseMove += GlViewport_MouseInteract;
            glViewport.MouseWheel += GlViewport_MouseInteract;
        }

        private void GlViewport_MouseInteract(object sender, MouseEventArgs e)
        {
            Renderer.UpdateCameraFromMouse();
        }

        public void ClearFiles()
        {
            animationBar.Clear();
            meshList.Clear();
            boneTree.Nodes.Clear();
            ParamNodeContainer.HitData = new Collision[0];

            Renderer.ClearRenderableNodes();
        }

        public void SaveScreenshot(string filePath)
        {
            using (var bmp = Renderer.GetScreenshot())
                bmp.Save(filePath);
        }

        public void ResetAnimation()
        {
            animationBar.Frame = 0f;
        }

        public CameraControl GetCameraControl()
        {
            return new CameraControl(Renderer.Camera);
        }

        public void Close()
        {
            glViewport.Dispose();
        }

        public void HideExpressionMeshes()
        {
            string[] expressionPatterns = { "Blink", "Attack", "Ouch", "Talk",
                "Capture", "Ottotto", "Escape", "Half",
                "Pattern", "Result", "Harf", "Hot", "Heavy",
                "Voice", "Fura", "Catch", "Cliff", "FLIP",
                "Bound", "Down", "Final", "Result", "StepPose",
                "Sorori", "Fall", "Appeal", "Damage", "CameraHit", "laugh", 
                "breath", "swell", "_low", "_bink", "inkMesh" };

            // TODO: This is probably not a very efficient way of doing this.
            foreach (ListViewItem item in meshList.Items)
            {
                var itemName = item.Name.ToLower();
                foreach (var pattern in expressionPatterns)
                {
                    if (itemName.Contains("openblink") || itemName.Contains("belly_low") || itemName.Contains("facen"))
                        continue;

                    if (itemName.Contains(pattern.ToLower()))
                    {
                        item.Checked = false;
                        ((RMesh)item.Tag).Visible = false;
                    }
                }
            }
        }

        public void UpdateGui(IRenderable newNode)
        {
            ClearBonesAndMeshList();

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

        public void RenderAnimationToFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
                return;

            glViewport.PauseRendering();
            animationBar.Stop();

            try
            {
                int repeat = (int)(1 / RenderSettings.Instance.RenderSpeed);
                for (int i = 0; i <= animationBar.FrameCount; ++i)
                {
                    animationBar.Frame = i;
                    glViewport.RenderFrame();
                    for (int j = 1; j <= repeat; ++j)
                    {
                        Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, false)
                                   .Save($"{folderPath}//Frame {i}.png");
                    }
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error while exporting frames");
                Console.Error.WriteLine(e.ToString());
            }

            glViewport.RestartRendering();
        }

        public async System.Threading.Tasks.Task RenderAnimationToGifAsync(string outputPath, IProgress<int> progress)
        {
            if (string.IsNullOrEmpty(outputPath))
                return;

            // Disable automatic updates so frames can be rendered manually.
            glViewport.PauseRendering();
            animationBar.Stop();

            var frames = new List<System.Drawing.Bitmap>(animationBar.FrameCount);

            // Rendering can't happen on a separate thread
            try
            {
                int repeat = (int)(1 / RenderSettings.Instance.RenderSpeed);
                for (int i = 0; i <= animationBar.FrameCount; ++i)
                {
                    animationBar.Frame = i;
                    glViewport.RenderFrame();
                    for (int j = 1; j <= repeat; ++j)
                    {
                        frames.Add(Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, false));
                    }

                    var ratio = (double) i / animationBar.FrameCount;
                    progress.Report((int)(ratio * 100));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error while making GIF");
                Console.Error.WriteLine(e.ToString());
            }

            glViewport.RestartRendering();

            // Continue on separate thread to maintain responsiveness.
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (var gif = new AnimatedGif.AnimatedGifCreator(outputPath, 20, 0))
                {
                    for (int i = 0; i < frames.Count; i++)
                        gif.AddFrame(frames[i], -1, AnimatedGif.GifQuality.Bit8);
                }
            });

            foreach (var frame in frames)
            {
                frame.Dispose();
            }
        }

        private void AddAnimationBar()
        {
            animationBar = new AnimationBar
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
            };
            controlBox.Controls.Add(animationBar);
        }

        private void CreateRenderFrameEvents()
        {
            glViewport.RenderFrameInterval = 15;
            glViewport.VSync = false;
            glViewport.OnRenderFrame += RenderNodes;
            glViewport.RestartRendering();
        }

        private void RenderNodes(object sender, EventArgs e)
        {
            Renderer.RenderNodes(ScriptNode);
        }

        private void AddMeshesToGui(RModel model)
        {
            animationBar.Model = model;

            if (model != null)
            {
                meshList.Items.Clear();

                foreach (var mesh in model.subMeshes)
                {
                    ListViewItem item = new ListViewItem
                    {
                        Name = mesh.Name,
                        Text = mesh.Name,
                        Tag = mesh,
                        Checked = mesh.Visible
                    };
                    meshList.Items.Add(item);
                }
            }
        }

        /// <summary>
        /// Populates the bones tab, and binds the given skeleton to subcomponents such as the animation bar
        /// </summary>
        /// <param name="skeleton"></param>
        private void AddSkeletonToGui(RSkeleton skeleton)
        {
            if (skeleton == null)
                return;

            animationBar.Skeleton = skeleton;
            Dictionary<int, TreeNode> boneById = new Dictionary<int, TreeNode>();

            foreach(RBone b in skeleton.Bones)
            {
                TreeNode node = new TreeNode
                {
                    Text = b.Name
                };
                boneById.Add(b.Id, node);

                // The root bone has no parent.
                if (b.ParentId == -1)
                    boneTree.Nodes.Add(node);
            }
            
            // Add each bone to its parent.
            foreach (RBone b in skeleton.Bones)
            {
                if (b.ParentId != -1)
                    boneById[b.ParentId].Nodes.Add(boneById[b.Id]);
            }
        }

        private void ClearBonesAndMeshList()
        {
            boneTree.Nodes.Clear();
            meshList.Items.Clear();
            controlBox.Visible = false;
        }

        public void SwapBuffers()
        {
            glViewport.SwapBuffers();
        }

        public void RenderFrame()
        {
            if (!glViewport.IsDisposed)
                glViewport.RenderFrame();
        }

        public void BeginBatchRenderMode()
        {
            glViewport.PauseRendering();
            boneTree.Visible = false;
            meshList.Visible = false;
        }

        public void EndBatchRenderMode()
        {
            glViewport.RestartRendering();
            boneTree.Visible = true;
            meshList.Visible = true;
        }

        private void glViewport_Resize(object sender, EventArgs e)
        {
            // Adjust for changing render dimensions.
            Renderer.Camera.RenderWidth = glViewport.Width;
            Renderer.Camera.RenderHeight = glViewport.Height;
            glViewport.RenderFrame();
        }

        private void meshList_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (e.Item == null || !(e.Item.Tag is RMesh))
                return;

            ((RMesh)e.Item.Tag).Visible = e.Item.Checked;
        }
    }
}
