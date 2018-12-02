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

            glViewport.OnRenderFrame += RenderFrame;
        }

        private void AnimationRenderFrame(object sender, EventArgs e)
        {
            glViewport.RenderFrame();
        }

        private void RenderFrame(object sender, EventArgs e)
        {
            //Clearing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1);

            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Lequal);

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
                    Camera.Position += new Vector3(-(NewMousePosition.X - MousePosition.X) / 10f,
                        -(NewMousePosition.Y - MousePosition.Y) / -10f, 0);
                }
                if (Keyboard.GetState().IsKeyDown(Key.Up))
                    Camera.Zoom(0.5f);
                if (Keyboard.GetState().IsKeyDown(Key.Down))
                    Camera.Zoom(-0.5f);
                Camera.Position += new Vector3(0,
                    0,
                    (NewMouseScrollWheel - MouseScrollWheel) / 10);
            }
            MousePosition = NewMousePosition;
            MouseScrollWheel = NewMouseScrollWheel;

            if (RenderableNode != null)
                RenderableNode.Render(Camera);

            glViewport.SwapBuffers();
            
            // Clean up any unused resources.
            GLObjectManager.DeleteUnusedGLObjects();
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
            glViewport.RenderFrame();
        }
    }
}
