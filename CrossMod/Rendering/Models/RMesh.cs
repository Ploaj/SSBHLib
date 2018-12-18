using OpenTK;
using SFGraphics.Cameras;
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

        public void Draw(Shader shader, Camera camera, RSkeleton skeleton)
        {
            if (!Visible)
                return;

            if (skeleton != null)
            {
                var matrix = Matrix4.Identity;
                if (SingleBindIndex >= 0)
                    matrix = skeleton.GetAnimationSingleBindsTransform(SingleBindIndex);
                shader.SetMatrix4x4("transform", ref matrix);
            }

            if (Material != null)
            {
                SetTextureUniforms(shader);
            }

            RenderMesh?.Draw(shader, camera);
        }

        private void SetTextureUniforms(Shader shader)
        {
            // TODO: Rework default texture creation.
            if (defaultTextures == null)
                defaultTextures = new Resources.DefaultTextures();

            var genericMaterial = Material.CreateGenericMaterial(Material);
            genericMaterial.SetShaderUniforms(shader);
        }
    }
}
