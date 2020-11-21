using CrossMod.Rendering.GlTools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;
using System;

namespace CrossMod.Rendering.Models
{
    public class RMesh
    {
        public UltimateMesh RenderMesh { get; }

        public string Name { get; }

        public long SubIndex { get; }

        public Vector4 BoundingSphere { get; }

        public string SingleBindName { get; }
        public int SingleBindIndex { get; }

        public RMaterial? Material { get; set; } = null;

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (value != isVisible)
                    VisibilityChanged?.Invoke(this, value);
                isVisible = value;
            }
        }
        private bool isVisible = true;

        public event EventHandler<bool>? VisibilityChanged;

        public RMesh(string name, long subIndex, string singleBindName, int singleBindIndex, Vector4 boundingSphere, UltimateMesh renderMesh)
        {
            Name = name;
            SubIndex = subIndex;
            SingleBindName = singleBindName;
            SingleBindIndex = singleBindIndex;
            BoundingSphere = boundingSphere;
            RenderMesh = renderMesh;
        }

        public void Draw(Shader shader, RSkeleton skeleton)
        {
            if (!IsVisible)
                return;

            if (skeleton != null)
            {
                shader.SetMatrix4x4("transform", skeleton.GetAnimationSingleBindsTransform(SingleBindIndex));
            }

            // TODO: When is primitive restart used?
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(0xFFFFFFFF);

            RenderMesh?.Draw(shader);
        }
    }
}
