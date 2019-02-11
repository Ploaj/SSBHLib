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

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        public IRenderableNode RenderableNode
        {
            set
            {
                ClearDisplay();

                if (value == null)
                {
                    renderableNode = null;
                    animationBar.Animation = null;
                    return;
                }
                
                if (value.GetRenderableNode() is IRenderableAnimation anim)
                {
                    animationBar.Animation = anim;
                    controlBox.Visible = true;
                }
                else
                {
                    animationBar.Animation = null;
                    renderableNode = value.GetRenderableNode();
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

                if (value is NUMDL_Node node)
                {
                    var rnumdl = (RNUMDL)node.GetRenderableNode();
                    FrameSelection(rnumdl.Model);
                }
            }
        }
        public ScriptNode ScriptNode
        {
            get { return animationBar.scriptNode; }
            set { animationBar.scriptNode = value; }
        }
        private IRenderable renderableNode;

        public Camera camera = new Camera() { FarClipPlane = 500000 };
        Vector2 mousePosition = new Vector2();
        float mouseScrollWheel = 0;

        private AnimationBar animationBar;

        public ModelViewport()
        {
            InitializeComponent();

            CreateRenderFrameEvents();

            AddAnimationBar();
        }

        public void FrameSelection(RModel model)
        {
            if (model == null)
                return;

            // Bounding spheres will help account for the vastly different model sizes.
            var sphere = model.BoundingSphere;
            camera.FrameBoundingSphere(sphere.Xyz, sphere.W, 5);
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
            return SFGraphics.GLObjects.Framebuffers.Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height);
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
            // HACK: Add proper frame timing.
            Application.Idle += AnimationRenderFrame;
            glViewport.OnRenderFrame += RenderNode;
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
                //remove any meshes that aren't on the model
                foreach (ListViewItem item in meshList.Items)
                {
                    RMesh mesh = (RMesh)item.Tag;
                    if (!model.subMeshes.Contains(mesh))
                        meshList.Items.Remove(item);
                }
                //only add meshes that aren't in the mesh list already
                foreach (var mesh in model.subMeshes)
                {
                    if (!meshList.Items.ContainsKey(mesh.Name))
                    {
                        ListViewItem item = new ListViewItem
                        {
                            Name = mesh.Name,
                            Text = mesh.Name,
                            Tag = mesh,
                            Checked = false
                        };

                        meshList.Items.Add(item);
                    }
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
            //meshList.Items.Clear();
            controlBox.Visible = false;
        }

        private void AnimationRenderFrame(object sender, EventArgs e)
        {
            glViewport.RenderFrame();
        }

        public void RenderFrame()
        {
            if (!glViewport.IsDisposed)
                glViewport.RenderFrame();
        }

        private void RenderNode(object sender, EventArgs e)
        {
            SetUpViewport();

            if (renderableNode != null)
            {
                renderableNode.Render(camera);
            }
            ParamNodeContainer.Render(camera);
            if (ScriptNode != null)
            {
                ScriptNode.Render(camera);
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
                    camera.RotationXRadians += (newMousePosition.Y - mousePosition.Y) / 100f;
                    camera.RotationYRadians += (newMousePosition.X - mousePosition.X) / 100f;
                }
                if (Mouse.GetState().IsButtonDown(MouseButton.Right))
                {
                    camera.Pan(newMousePosition.X - mousePosition.X, newMousePosition.Y - mousePosition.Y);
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
