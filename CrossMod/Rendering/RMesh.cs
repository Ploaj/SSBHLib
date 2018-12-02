using OpenTK.Graphics.OpenGL;
using SFGraphics.Cameras;
using SFGraphics.GLObjects.Shaders;
using System.Collections.Generic;

namespace CrossMod.Rendering
{
    public class RMesh : IRenderable
    {
        public string Name;

        public List<CustomVertexAttribute> VertexAttributes = new List<CustomVertexAttribute>();

        public int IndexOffset;
        public int IndexCount;

        public string SingleBindName = "";
        public int SingleBindIndex = -1;

        public Material Material;

        public void Draw(Shader shader, Camera camera)
        {
            shader.SetInt("SingleBindIndex", SingleBindIndex);
            if (Material != null)
            {
                SetTextureUniforms(shader);
            }
            foreach (CustomVertexAttribute a in VertexAttributes)
            {
                a.Bind(shader);
            }

            GL.DrawElements(PrimitiveType.Triangles, IndexCount, DrawElementsType.UnsignedShort, IndexOffset);
        }

        private void SetTextureUniforms(Shader shader)
        {
            // Don't use the default texture unit.
            var genericMaterial = new SFGenericModel.Materials.GenericMaterial(1);
            if (Material.col != null)
                genericMaterial.AddTexture("colMap", Material.col);

            if (Material.prm != null)
                genericMaterial.AddTexture("prmMap", Material.col);

            if (Material.nor != null)
                genericMaterial.AddTexture("norMap", Material.col);

            genericMaterial.SetShaderUniforms(shader);
        }

        public void Render(Camera Camera)
        {

        }
    }
}
