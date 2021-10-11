using CrossMod.MaterialValidation;
using CrossMod.Rendering.GlTools;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using SFGraphics.GLObjects.Shaders;
using System;
using System.Collections.Generic;

namespace CrossMod.Rendering.Models
{
    public class RMesh
    {
        public UltimateMesh RenderMesh { get; }

        public string Name { get; }

        public int SortBias { get; }

        public long SubIndex { get; }

        public Vector4 BoundingSphere { get; }

        public string SingleBindName { get; }
        public int SingleBindIndex { get; }
        public bool EnableDepthWrites { get; }
        public bool EnableDepthTest { get; }


        public RMaterial? Material {
            get => material;
            set
            {
                material = value;
                if (value != null)
                {
                    // Cache this database lookup to avoid doing it every frame.
                    // TODO: The actual in game check is more complicated and involves checking names, subindex, and usage.
                    HasRequiredAttributes = ShaderValidation.IsValidAttributeList(value.ShaderLabel, AttributeNames);
                }
                else
                {
                    // Avoid having both an invalid shader label and missing attribute error if no material.
                    HasRequiredAttributes = true;
                }
            }
        }
        private RMaterial? material = null;

        public bool HasRequiredAttributes { get; private set; }

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

        public List<string> AttributeNames { get; } = new List<string>();

        public RMesh(string name, long subIndex, string singleBindName, int singleBindIndex, 
            Vector4 boundingSphere, UltimateMesh renderMesh, List<string> attributeNames, int sortBias,
            bool enableDepthWrites, bool enableDepthTest)
        {
            Name = name;
            SubIndex = subIndex;
            SingleBindName = singleBindName;
            SingleBindIndex = singleBindIndex;
            BoundingSphere = boundingSphere;
            RenderMesh = renderMesh;
            AttributeNames = attributeNames;
            SortBias = sortBias;
            EnableDepthWrites = enableDepthWrites;
            EnableDepthTest = enableDepthTest;
        }

        public void Draw(Shader shader, RSkeleton? skeleton)
        {
            if (!IsVisible)
                return;

            // Meshes without a skeleton should still render, so default to an identity matrix.
            shader.SetMatrix4x4("transform", skeleton?.GetAnimationSingleBindsTransform(SingleBindIndex) ?? Matrix4.Identity);

            RenderMesh?.Draw(shader);
        }
    }
}
