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

        public bool Visible = true;

        public void Draw(Shader shader, Camera camera, RSkeleton skeleton)
        {
            if (!Visible) return;
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

            var genericMaterial = CreateGenericMaterial(Material);

            genericMaterial.SetShaderUniforms(shader);
        }

        private SFGenericModel.Materials.GenericMaterial CreateGenericMaterial(Material material)
        {
            // Don't use the default texture unit.
            var genericMaterial = new SFGenericModel.Materials.GenericMaterial(1);

            if (material.col != null)
                genericMaterial.AddTexture("colMap", material.col);
            else
                genericMaterial.AddTexture("colMap", defaultTextures.defaultWhite);

            if (material.prm != null)
                genericMaterial.AddTexture("prmMap", material.prm);
            else
                genericMaterial.AddTexture("prmMap", defaultTextures.defaultWhite);

            if (material.nor != null)
                genericMaterial.AddTexture("norMap", material.nor);
            else
                genericMaterial.AddTexture("norMap", defaultTextures.defaultNormal);

            if (material.emi != null)
                genericMaterial.AddTexture("emiMap", material.emi);
            else
                genericMaterial.AddTexture("emiMap", defaultTextures.defaultBlack);

            genericMaterial.AddTexture("diffusePbrCube", defaultTextures.diffusePbr);
            genericMaterial.AddTexture("specularPbrCube", defaultTextures.specularPbr);
            genericMaterial.AddTexture("iblLut", defaultTextures.iblLut);

            // Set specific parameters and use a default value if not present.
            AddMtalVec4(genericMaterial, material, RenderSettings.paramId, new Vector4(0));

            return genericMaterial;
        }

        private static void AddMtalVec4(SFGenericModel.Materials.GenericMaterial genericMaterial, Material source, long paramId, Vector4 defaultValue)
        {
            if (source.vec4ByParamId.ContainsKey(paramId))
            {
                var value = source.vec4ByParamId[paramId];
                genericMaterial.AddVector4($"vec4Param", new Vector4(value.X, value.Y, value.Z, value.W));
            }
            else
            {
                genericMaterial.AddVector4($"vec4Param", defaultValue);
            }
        }

        public void Render(Camera Camera)
        {

        }
    }
}
