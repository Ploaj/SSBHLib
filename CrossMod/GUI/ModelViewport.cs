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

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        // Render-ables
        public IRenderableNode RenderableNode
        {
            set
            {
                if (value == null)
                    return;
                
                if (value.GetRenderableNode() is IRenderableAnimation anim)
                {
                    animationBar.Animation = anim;
                    animationBar.FrameCount = anim.GetFrameCount();
                    ClearDisplay();
                    controlBox.Visible = true;
                }
                else
                {
                    renderableNode = value.GetRenderableNode();
                    ClearDisplay();
                }

                if (renderableNode is RSkeleton skeleton)
                {
                    DisplaySkeleton(skeleton);
                }
                else if (renderableNode is RModel model)
                {
                    DisplayMeshes(model);
                }
                else if(renderableNode is IRenderableModel renderableModel)
                {
                    DisplayMeshes(renderableModel.GetModel());
                    DisplaySkeleton(renderableModel.GetSkeleton());
                }
            }
        }
        private IRenderable renderableNode;

        public Camera camera;
        Vector2 mousePosition = new Vector2();
        float mouseScrollWheel = 0;

        private AnimationBar animationBar;

        public ModelViewport()
        {
            InitializeComponent();

            camera = new Camera();

            CreateRenderFrameEvents();

            AddAnimationBar();
        }

        public void ClearFiles()
        {
            animationBar.Model = null;
            animationBar.Skeleton = null;
            RenderableNode = null;

            GC.WaitForPendingFinalizers();
            GLObjectManager.DeleteUnusedGLObjects();
        }

        public System.Drawing.Bitmap GetScreenshot()
        {
            return Rendering.Utils.FramebufferUtils.ReadFrameBufferPixels(0, FramebufferTarget.Framebuffer, glViewport.Width, glViewport.Height);
        }

        private void AddAnimationBar()
        {
            animationBar = new AnimationBar
            {
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            controlBox.Controls.Add(animationBar);
        }

        private void CreateRenderFrameEvents()
        {
            Application.Idle += AnimationRenderFrame;
            glViewport.OnRenderFrame += RenderNode;
        }

        private void DisplayMeshes(RModel Model)
        {
            animationBar.Model = Model;
            if(Model != null)
            foreach(RMesh m in Model.subMeshes)
            {
                ListViewItem item = new ListViewItem
                {
                    Text = m.Name,
                    Tag = m,
                    Checked = true
                };

                meshList.Items.Add(item);
            }
        }

        private void DisplaySkeleton(RSkeleton skeleton)
        {
            animationBar.Skeleton = skeleton;
            Dictionary<int, TreeNode> boneById = new Dictionary<int, TreeNode>();

            foreach(RBone b in skeleton.Bones)
            {
                TreeNode node = new TreeNode
                {
                    Text = b.Name
                };

                boneById.Add(b.ID, node);
                if (b.ParentID == -1)
                    boneTree.Nodes.Add(node);
            }

            foreach (RBone b in skeleton.Bones)
            {
                if (b.ParentID != -1)
                    boneById[b.ParentID].Nodes.Add(boneById[b.ID]);
            }
        }

        private void ClearDisplay()
        {
            boneTree.Nodes.Clear();
            meshList.Items.Clear();
            controlBox.Visible = false;
        }

        private void AnimationRenderFrame(object sender, EventArgs e)
        {
            glViewport.RenderFrame();
        }

        public void RenderFrame()
        {
            glViewport.RenderFrame();
        }

        private void RenderNode(object sender, EventArgs e)
        {
            SetUpViewport();

            if (renderableNode != null)
            {
                renderableNode.Render(camera);
            }


            // Clean up any unused resources.
            GLObjectManager.DeleteUnusedGLObjects();
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

            GL.Enable(EnableCap.AlphaTest);
            GL.AlphaFunc(AlphaFunction.Gequal, 0.1f);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
        }

        private void UpdateCamera()
        {
            Vector2 newMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            float newMouseScrollWheel = Mouse.GetState().Wheel;

            if (glViewport.Focused)
            {
                if (Mouse.GetState().IsButtonDown(MouseButton.Left))
                {
                    camera.RotationXRadians += ((newMousePosition.Y - mousePosition.Y) / 100f);
                    camera.RotationYRadians += (newMousePosition.X - mousePosition.X) / 100f;
                }
                if (Mouse.GetState().IsButtonDown(MouseButton.Right))
                {
                    camera.Pan((newMousePosition.X - mousePosition.X), (newMousePosition.Y - mousePosition.Y));
                }
                if (Keyboard.GetState().IsKeyDown(Key.W))
                    camera.Zoom(0.5f);
                if (Keyboard.GetState().IsKeyDown(Key.S))
                    camera.Zoom(-0.5f);

                camera.Zoom((newMouseScrollWheel - mouseScrollWheel) * 0.1f);
            }

            mousePosition = newMousePosition;
            mouseScrollWheel = newMouseScrollWheel;
        }

        public void Clear()
        {
            RenderableNode = null;
        }

        private void glViewport_Load(object sender, EventArgs e)
        {
            glViewport.RenderFrame();
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
