using System;
using System.Windows.Forms;
using CrossMod.Rendering;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.GLObjectManagement;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Input;

namespace CrossMod.GUI
{
    public partial class ModelViewport : UserControl
    {
        // Render-ables
        public IRenderable RenderableNode;

        public Camera Camera;
        Vector2 MousePosition = new Vector2();
        float MouseScrollWheel = 0;

        public ModelViewport()
        {
            InitializeComponent();

            Camera = new Camera();
            Camera.Position = Camera.DefaultPosition;

            Application.Idle += AnimationRenderFrame;

            glViewport.OnRenderFrame += RenderNode;
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
            //Clearing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.25f, 0.25f, 0.25f, 1);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            UpdateCamera();

            if (RenderableNode != null)
                RenderableNode.Render(Camera);

            // Clean up any unused resources.
            GLObjectManager.DeleteUnusedGLObjects();
        }

        private void UpdateCamera()
        {
            Vector2 NewMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            float NewMouseScrollWheel = Mouse.GetState().Wheel;
            if (glViewport.Focused)
            {
                if (Mouse.GetState().IsButtonDown(MouseButton.Left))
                {
                    Camera.RotationXRadians += ((NewMousePosition.Y - MousePosition.Y) / 100f);
                    Camera.RotationYRadians += (NewMousePosition.X - MousePosition.X) / 100f;
                }
                if (Mouse.GetState().IsButtonDown(MouseButton.Right))
                {
                    Camera.Pan((NewMousePosition.X - MousePosition.X), (NewMousePosition.Y - MousePosition.Y));
                }
                if (Keyboard.GetState().IsKeyDown(Key.Up))
                    Camera.Zoom(0.5f);
                if (Keyboard.GetState().IsKeyDown(Key.Down))
                    Camera.Zoom(-0.5f);

                Camera.Zoom((NewMouseScrollWheel - MouseScrollWheel) * 0.1f);
            }
            MousePosition = NewMousePosition;
            MouseScrollWheel = NewMouseScrollWheel;
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
            Camera.RenderWidth = glViewport.Width;
            Camera.RenderHeight = glViewport.Height;

            glViewport.RenderFrame();
        }
    }
}
