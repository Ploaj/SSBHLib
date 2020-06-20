using CrossMod.Rendering.GlTools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGenericModel.Materials;
using SFGraphics.GLObjects.Shaders;

namespace CrossMod.Rendering.Models
{
    public class RMesh
    {
        public static Resources.DefaultTextures defaultTextures = null;

        public RenderMesh RenderMesh { get; set; } = null;

        public string Name { get; set; }

        public Vector4 BoundingSphere { get; set; }

        public string SingleBindName { get; set; } = "";
        public int SingleBindIndex { get; set; } = -1;

        public Material Material { get; set; } = null;

        public bool Visible { get; set; } = true;

        public GenericMaterial genericMaterial = null;
        private UniformBlock uniformBlock = null;

        public void Draw(Shader shader, RSkeleton skeleton)
        {
            if (!Visible)
                return;

            if (skeleton != null)
            {
                shader.SetMatrix4x4("transform", skeleton.GetAnimationSingleBindsTransform(SingleBindIndex));
            }

            // TODO: ???
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(0xFFFFFFFF);

            RenderMesh?.Draw(shader);
        }
        public void SetMaterialUniforms(Shader shader, GenericMaterial previousMaterial)
        {
            // TODO: Rework default texture creation.
            if (defaultTextures == null)
                defaultTextures = new Resources.DefaultTextures();

            if (genericMaterial == null)
                genericMaterial = Material.CreateGenericMaterial();
            genericMaterial.SetShaderUniforms(shader, previousMaterial);

            if (uniformBlock == null)
            {
                uniformBlock = new UniformBlock(shader, "MaterialParams") { BlockBinding = 1 };
                Material.AddMaterialParams(uniformBlock);
            }
            // This needs to be updated more than once.
            Material.AddDebugParams(uniformBlock);

            uniformBlock.BindBlock(shader);
        }
    }
}
