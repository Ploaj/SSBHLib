using CrossMod.Rendering.GlTools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.Materials;
using SFGraphics.GLObjects.Shaders;
using System;

namespace CrossMod.Rendering.Models
{
    public class RMesh
    {
        public static Resources.DefaultTextures defaultTextures = null;

        public UltimateMesh RenderMesh { get; set; } = null;

        public string Name { get; set; }

        public Vector4 BoundingSphere { get; set; }

        public string SingleBindName { get; set; } = "";
        public int SingleBindIndex { get; set; } = -1;

        public Material Material { get; set; } = null;

        public bool Visible 
        { 
            get => visible; 
            set
            {
                if (value != visible)
                    VisibilityChanged?.Invoke(this, value);
                visible = value;
            }
        }
        private bool visible = true;

        public event EventHandler<bool> VisibilityChanged;

        public void Draw(Shader shader, RSkeleton skeleton)
        {
            if (!Visible)
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
