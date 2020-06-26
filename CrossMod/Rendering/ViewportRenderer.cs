using CrossMod.Nodes;
using CrossMod.Rendering.GlTools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SFGraphics.Cameras;
using SFGraphics.Controls;
using SFGraphics.GLObjects.Framebuffers;
using SFGraphics.GLObjects.GLObjectManagement;
using System;
using System.Linq;

namespace CrossMod.Rendering
{
    public class ViewportRenderer
    {
        // Previous mouse state.
        private Vector2 mousePosition;
        private float mouseScrollWheel;

        public IRenderable ItemToRender
        {
            get => itemToRender;
            set
            {
                itemToRender = value;
                FrameRenderableModels();
            }
        }
        private IRenderable itemToRender;
     
        public IRenderableAnimation RenderableAnimation { get; set; }

        public float CurrentFrame { get; set; }

        private readonly GLViewport glViewport;

        public Camera Camera { get; } = new Camera() { FarClipPlane = 500000 };

        public ViewportRenderer(GLViewport viewport)
        {
            glViewport = viewport;
        }

        public void SwapBuffers() => glViewport.SwapBuffers();

        public void PauseRendering() => glViewport.PauseRendering();

        public void RestartRendering() => glViewport.RestartRendering();

        public bool IsRendering => glViewport.IsRendering;

        public void ClearRenderableNodes()
        {
            SwitchContextToCurrentThreadAndPerformAction(() =>
            {
                ItemToRender = null;
                GC.WaitForPendingFinalizers();
                GLObjectManager.DeleteUnusedGLObjects();
            });
        }

        public void FrameRenderableModels()
        {
            if (itemToRender is IRenderableModel model && model.GetModel() != null)
                Camera.FrameBoundingSphere(model.GetModel().BoundingSphere, 0);
        }

        public void UpdateCameraFromMouse()
        {
            var mouseState = Mouse.GetState();

            Vector2 newMousePosition = new Vector2(mouseState.X, mouseState.Y);
            float newMouseScrollWheel = mouseState.Wheel;

            if (mouseState.IsButtonDown(MouseButton.Left))
            {
                Camera.RotationXRadians += (newMousePosition.Y - mousePosition.Y) / 100f;
                Camera.RotationYRadians += (newMousePosition.X - mousePosition.X) / 100f;
            }
            if (mouseState.IsButtonDown(MouseButton.Right))
            {
                Camera.Pan(newMousePosition.X - mousePosition.X, newMousePosition.Y - mousePosition.Y);
            }

            Camera.Zoom((newMouseScrollWheel - mouseScrollWheel) * 0.1f);

            mousePosition = newMousePosition;
            mouseScrollWheel = newMouseScrollWheel;
        }

        public void ReloadShaders()
        {
            SwitchContextToCurrentThreadAndPerformAction(() =>
            {
                ShaderContainer.ReloadShaders();
            });
        }

        public void RenderNodes(ScriptNode scriptNode)
        {
            // Ensure shaders are created before drawing anything.
            if (!ShaderContainer.HasSetUp)
                ShaderContainer.SetUpShaders();

            SetUpViewport();

            // Don't render anything else if there is a texture.

            if (itemToRender is IRenderableModel model)
            {
                RenderableAnimation?.SetFrameModel(model.GetModel(), CurrentFrame);
                RenderableAnimation?.SetFrameSkeleton(model.GetSkeleton(), CurrentFrame);
            }
            itemToRender?.Render(Camera);
                
            ParamNodeContainer.Render(Camera);
            scriptNode?.Render(Camera);
        }

        public System.Drawing.Bitmap GetScreenshot()
        {
            // Make sure the context is current on this thread.
            var wasRendering = glViewport.IsRendering;
            glViewport.PauseRendering();

            var bmp = Framebuffer.ReadDefaultFramebufferImagePixels(glViewport.Width, glViewport.Height, true);

            if (wasRendering)
                glViewport.RestartRendering();

            return bmp;
        }

        public void SwitchContextToCurrentThreadAndPerformAction(Action action)
        {
            // Make sure the context is current on this thread.
            var wasRendering = glViewport.IsRendering;
            glViewport.PauseRendering();

            action();

            if (wasRendering)
                glViewport.RestartRendering();
        }

        private static void SetUpViewport()
        {
            DrawBackgroundClearBuffers();
            SetRenderState();
        }

        private static void DrawBackgroundClearBuffers()
        {
            // TODO: Clearing can be skipped if there is a background to draw.
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
    }
}
