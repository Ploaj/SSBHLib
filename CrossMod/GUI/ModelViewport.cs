using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CrossMod.Rendering;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.GLObjectManagement;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Input;
using CrossMod.Nodes;
using CrossMod.Rendering.Models;
using SFGraphics.GLObjects.Framebuffers;
using SFGraphics.GLObjects.Shaders;

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        private AnimationBar animationBar;

        private readonly HashSet<string> renderableNodeNames = new HashSet<string>();

        // This isn't a dictionary so that render order is preserved.
        private readonly List<IRenderable> renderableNodes = new List<IRenderable>();

        private IRenderable renderTexture = null;

        private readonly Camera camera = new Camera() { FarClipPlane = 500000 };
        private Vector2 mousePosition;
        private float mouseScrollWheel;

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
            CreateRenderFrameEvents();
            AddAnimationBar();
        }

        public void UpdateTexture(NutexNode texture)
        {
            var node = texture?.GetRenderableNode();
            renderTexture = node;
        }

        public void AddRenderableNode(string name, IRenderableNode value)
        {
            ClearBonesAndMeshList();

            if (value == null)
                return;

            var newNode = value.GetRenderableNode();

            // Prevent duplicates. Paths should be unique.
            if (!renderableNodeNames.Contains(name))
            {
                renderableNodes.Add(newNode);
                renderableNodeNames.Add(name);
            }

            // Duplicate nodes should still update the mesh list.
            if (newNode is RSkeleton skeleton)
            {
                DisplaySkeleton(skeleton);
            }
            else if (newNode is IRenderableModel renderableModel)
            {
                DisplayMeshes(renderableModel.GetModel());
                DisplaySkeleton(renderableModel.GetSkeleton());
            }

            if (value is NumdlNode)
            {
                FrameSelection();
            }
        }

        public void FrameSelection()
        {
            // Bounding spheres will help account for the vastly different model sizes.
            var spheres = new List<Vector4>();
            foreach (var node in renderableNodes)
            {
                if (node is Rnumdl rnumdl && rnumdl.Model != null)
                {
                    spheres.Add(rnumdl.Model.BoundingSphere);
                }
            }

            var allModelBoundingSphere = SFGraphics.Utils.BoundingSphereGenerator.GenerateBoundingSphere(spheres);
            camera.FrameBoundingSphere(allModelBoundingSphere, 0);
        }

        public void ClearFiles()
        {
            animationBar.Clear();

            renderableNodes.Clear();
            renderableNodeNames.Clear();

            meshList.Clear();
            boneTree.Nodes.Clear();

            GLObjectManager.DeleteUnusedGLObjects();
        }

        public System.Drawing.Bitmap GetScreenshot()
        {
            return Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, true);
        }

        public CameraControl GetCameraControl()
        {
            return new CameraControl(camera);
        }

        public void Close()
        {
            glViewport.Dispose();
        }

        public async System.Threading.Tasks.Task RenderAnimationToGifAsync(string outputPath, IProgress<int> progress)
        {
            // Disable automatic updates so frames can be rendered manually.
            glViewport.PauseRendering();
            animationBar.Stop();

            var frames = new List<System.Drawing.Bitmap>(animationBar.FrameCount);

            // Rendering can't happen on a separate thread.
            for (int i = 0; i <= animationBar.FrameCount; i++)
            {
                animationBar.Frame = i;
                glViewport.RenderFrame();
                frames.Add(Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, false));

                var ratio = (double) i / animationBar.FrameCount;
                progress.Report((int)(ratio * 100));
            }

            // Continue on separate thread to maintain responsiveness.
            glViewport.ResumeRendering();

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
            glViewport.OnRenderFrame += RenderNodes;
            glViewport.ResumeRendering();
        }

        /// <summary>
        /// Populates the meshes tab, and binds the given model to subcomponents such as the animation bar.
        /// </summary>
        /// <param name="model"></param>
        private void DisplayMeshes(RModel model)
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
        private void DisplaySkeleton(RSkeleton skeleton)
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
                if (b.ParentId == -1)
                    boneTree.Nodes.Add(node);
            }

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
            glViewport.ResumeRendering();
            boneTree.Visible = true;
            meshList.Visible = true;
        }

        private void RenderNodes(object sender, EventArgs e)
        {
            // Ensure shaders are created before drawing anything.
            if (!ShaderContainer.HasSetUp)
                ShaderContainer.SetUpShaders();

            SetUpViewport();

            if (renderTexture != null)
            {
                renderTexture.Render(camera);
            }            
            else
            {
                foreach (var node in renderableNodes)
                    node.Render(camera);

                ParamNodeContainer.Render(camera);
                ScriptNode?.Render(camera);
            }
        }

        private void SetUpViewport()
        {
            ClearViewportBuffers();
            SetRenderState();
            UpdateCamera();
        }

        private static void ClearViewportBuffers()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);
        }

        private static void SetRenderState()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        private void UpdateCamera()
        {
            var mouseState = Mouse.GetState();
            var keyboardState = Keyboard.GetState();

            Vector2 newMousePosition = new Vector2(mouseState.X, mouseState.Y);
            float newMouseScrollWheel = mouseState.Wheel;

            // Reduce the chance of rotating the viewport while the mouse is on other controls.
            if (glViewport.Focused && glViewport.ClientRectangle.Contains(PointToClient(MousePosition)))
            {
                if (mouseState.IsButtonDown(MouseButton.Left))
                {
                    camera.RotationXRadians += (newMousePosition.Y - mousePosition.Y) / 100f;
                    camera.RotationYRadians += (newMousePosition.X - mousePosition.X) / 100f;
                }
                if (mouseState.IsButtonDown(MouseButton.Right))
                {
                    camera.Pan(newMousePosition.X - mousePosition.X, newMousePosition.Y - mousePosition.Y);
                }
                if (keyboardState.IsKeyDown(Key.W))
                    camera.Zoom(0.5f);
                if (keyboardState.IsKeyDown(Key.S))
                    camera.Zoom(-0.5f);

                camera.Zoom((newMouseScrollWheel - mouseScrollWheel) * 0.1f);
            }

            mousePosition = newMousePosition;
            mouseScrollWheel = newMouseScrollWheel;
        }

        private void glViewport_Resize(object sender, EventArgs e)
        {
            // Adjust for changing render dimensions.
            camera.RenderWidth = glViewport.Width;
            camera.RenderHeight = glViewport.Height;

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
