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

        // This isn't a dictionary to preserve render order.
        private readonly HashSet<string> renderableNodeNames = new HashSet<string>();
        private readonly List<IRenderable> renderableNodes = new List<IRenderable>();
        private IRenderable renderTexture = null;

        private readonly Camera camera = new Camera() { FarClipPlane = 500000 };
        private Vector2 mousePosition;
        private float mouseScrollWheel;
        private static string tb1 = "64";
        private static string tb2 = "64";
        private static string tb3 = "64";
        private static bool renderAlpha = false;

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
            return Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, renderAlpha);
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
                frames.Add(Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, renderAlpha));

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
            GL.ClearColor(float.Parse(tb1) / 255.0f, float.Parse(tb2) / 255.0f, float.Parse(tb3) / 255.0f, 0);
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

        private void GlViewport_Load(object sender, EventArgs e)
        {

        }

        private void BgColorSetter_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9]"))
            {
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
            if (textBox1.Text == "")
            {
                textBox1.Text = "0";
            }
            if (Int32.Parse(textBox1.Text) >= 256)
            {
                textBox1.Text = "255";
            }
            tb1 = textBox1.Text;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(float.Parse(tb1)/255.0f, float.Parse(tb2) / 255.0f, float.Parse(tb3) / 255.0f, 0);
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, "[^0-9]"))
            {
                textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
            }
            if (textBox2.Text == "")
            {
                textBox2.Text = "0";
            }
            if (Int32.Parse(textBox2.Text) >= 256)
            {
                textBox2.Text = "255";
            }
            tb2 = textBox2.Text;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(float.Parse(tb1) / 255.0f, float.Parse(tb2) / 255.0f, float.Parse(tb3) / 255.0f, 0);
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox3.Text, "[^0-9]"))
            {
                textBox3.Text = textBox3.Text.Remove(textBox3.Text.Length - 1);
            }
            if (textBox3.Text == "")
            {
                textBox3.Text = "0";
            }
            if (Int32.Parse(textBox3.Text) >= 256)
            {
                textBox3.Text = "255";
            }
            tb3 = textBox3.Text;
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(float.Parse(tb1) / 255.0f, float.Parse(tb2) / 255.0f, float.Parse(tb3) / 255.0f, 0);
        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            renderAlpha = checkBox1.Checked;
        }
    }
}
