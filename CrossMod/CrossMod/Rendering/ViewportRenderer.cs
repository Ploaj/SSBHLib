using CrossMod.Nodes;
using CrossMod.Rendering.GlTools;
using CrossMod.Rendering.Models;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Framebuffers;
using SFGraphics.GLObjects.GLObjectManagement;
using System;
using System.Linq;

namespace CrossMod.Rendering
{
    public abstract class ViewportRenderer
    {
        // TODO: Handle input somewhere else.
        // Previous mouse state.
        private Vector2 mousePosition;
        private float mouseScrollWheel;

        // TODO: Find a cleaner way to override the main item?
        public IRenderable? ItemToRenderOverride { get; set;} = null;

        public IRenderable? ItemToRender
        {
            get => itemToRender;
            set
            {
                itemToRender = value;
                FrameRenderableModels();
            }
        }
        private IRenderable? itemToRender;

        public IRenderableAnimation? RenderableAnimation { get; set; }

        public ScriptNode? ScriptNode { get; set; }

        public Camera Camera { get; } = new Camera() { NearClipPlane = 2, FarClipPlane = 500000 };

        public void UpdateMouseScroll() => mouseScrollWheel = Mouse.GetState().WheelPrecise;

        public abstract int Width { get; }
        public abstract int Height { get; }

        public abstract void SwapBuffers();

        public abstract void PauseRendering();

        public abstract void RestartRendering();

        public bool IsRendering { get; protected set; }

        public void Clear()
        {
            SwitchContextToCurrentThreadAndPerformAction(() =>
            {
                ItemToRender = null;
                ItemToRenderOverride = null;
                RenderableAnimation = null;
                GC.WaitForPendingFinalizers();
                GLObjectManager.DeleteUnusedGLObjects();
            });
        }

        public void FrameRenderableModels()
        {
            if (itemToRender is IRenderableModel model && model.RenderModel != null)
                Camera.FrameBoundingSphere(model.RenderModel.BoundingSphere, 0);
        }

        public void UpdateCameraFromMouse()
        {
            var mouseState = Mouse.GetState();

            Vector2 newMousePosition = new Vector2(mouseState.X, mouseState.Y);
            float newMouseScrollWheel = mouseState.WheelPrecise;

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

        public void RenderNodes(float currentFrame = 0)
        {
            SetUpViewport();

            SetRenderState();
            DrawItemToRender(currentFrame);

            ParamNodeContainer.Render(Camera);
            ScriptNode?.Render(Camera);
        }

        private void DrawItemToRender(float currentFrame)
        {
            // Animate the override item instead of the regular renderable item.
            // This allows animating single models by clicking the model and then the animation.
            // TODO: How to support animating model collections?
            if (ItemToRenderOverride is IRenderableModel model)
            {
                RenderableAnimation?.SetFrameModel(model.RenderModel.SubMeshes, currentFrame);
                RenderableAnimation?.SetFrameSkeleton(model.Skeleton.Bones, currentFrame);
            }

            if (ItemToRenderOverride != null)
            {
                ItemToRenderOverride.Render(Camera);
                return;
            }

            if (ItemToRender is ModelCollection collection)
            {
                // TODO: There's probably a simpler/more efficient way to do this.
                var meshes = collection.Meshes
                    .Select(m => m.Item1);

                var bones = collection
                    .Meshes
                    .Select(m => m.Item2)
                    .Distinct()
                    .SelectMany(s => s.Bones);

                RenderableAnimation?.SetFrameModel(meshes, currentFrame);
                RenderableAnimation?.SetFrameSkeleton(bones, currentFrame);
            }

            ItemToRender?.Render(Camera);
        }

        public System.Drawing.Bitmap GetScreenshot()
        {
            // Make sure the context is current on this thread.
            var wasRendering = IsRendering;
            PauseRendering();

            var bmp = Framebuffer.ReadDefaultFramebufferImagePixels(Width, Height, true);

            if (wasRendering)
                RestartRendering();

            return bmp;
        }

        public void SwitchContextToCurrentThreadAndPerformAction(Action action)
        {
            // Make sure the context is current on this thread.
            var wasRendering = IsRendering;
            PauseRendering();

            action();

            if (wasRendering)
                RestartRendering();
        }

        private void SetUpViewport()
        {
            ClearBuffers();
        }

        private void ClearBuffers()
        {
            // TODO: Add background color to render settings.
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.ClearColor(System.Drawing.Color.FromArgb(255, 60, 60, 60));
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
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
