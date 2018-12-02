using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;
using OpenTK;

namespace CrossMod.Rendering
{
    public class RMesh : IRenderable
    {
        private static DefaultTextures defaultTextures = null;

        public string Name;

        public DrawElementsType DrawElementType = DrawElementsType.UnsignedShort;

        public List<CustomVertexAttribute> VertexAttributes = new List<CustomVertexAttribute>();

        public int IndexOffset;
        public int IndexCount;

        public string SingleBindName = "";
        public int SingleBindIndex = -1;

        public Material Material;

        public void Draw(Shader shader, Camera camera, RSkeleton skeleton)
        {
            if (skeleton != null)
            {
                var transforms = skeleton.GetWorldTransforms();
                var matrix = Matrix4.Identity;
                if (SingleBindIndex >= 0 && SingleBindIndex < transforms.Length)
                    matrix = transforms[SingleBindIndex];
                shader.SetMatrix4x4("transform", ref matrix);
            }

            if (Material != null)
            {
                SetTextureUniforms(shader);
            }
            foreach (CustomVertexAttribute a in VertexAttributes)
            {
                a.Bind(shader);
            }

            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementType, IndexOffset);
        }

        private void SetTextureUniforms(Shader shader)
        {
            if (defaultTextures == null)
                defaultTextures = new DefaultTextures();

            // Don't use the default texture unit.
            var genericMaterial = new SFGenericModel.Materials.GenericMaterial(1);
            if (Material.col != null)
                genericMaterial.AddTexture("colMap", Material.col);
            else
                genericMaterial.AddTexture("colMap", defaultTextures.defaultWhite);

            if (Material.prm != null)
                genericMaterial.AddTexture("prmMap", Material.prm);
            else
                genericMaterial.AddTexture("prmMap", defaultTextures.defaultWhite);

            if (Material.nor != null)
                genericMaterial.AddTexture("norMap", Material.nor);
            else
                genericMaterial.AddTexture("norMap", defaultTextures.defaultNormal);

            genericMaterial.SetShaderUniforms(shader);
        }

        public void Render(Camera Camera)
        {

        }
    }
}
