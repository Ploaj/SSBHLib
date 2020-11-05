using CrossMod.Tools;
using SSBHLib.Formats.Materials;
using System.Collections.Generic;

namespace CrossMod.Rendering.GlTools
{
    public static class MatlToMaterial
    {
        public static RMaterial CreateMaterial(MatlEntry currentEntry, int index, Dictionary<string, RTexture> textureByName)
        {
            RMaterial meshMaterial = new RMaterial()
            {
                MaterialLabel = currentEntry.MaterialLabel,
                ShaderLabel = currentEntry.ShaderLabel,
                Index = index,
                TextureByName = textureByName
            };

            foreach (var pair in currentEntry.GetTextures())
            {
                // Don't make texture names case sensitive.
                meshMaterial.UpdateTexture(pair.Key, pair.Value.ToLower());
            }

            foreach (var pair in currentEntry.GetFloats())
            {
                meshMaterial.UpdateFloat(pair.Key, pair.Value);
            }

            foreach (var pair in currentEntry.GetVectors())
            {
                meshMaterial.UpdateVec4(pair.Key, pair.Value.ToOpenTk());
            }

            foreach (var pair in currentEntry.GetBools())
            {
                meshMaterial.UpdateBoolean(pair.Key, pair.Value);
            }

            foreach (var pair in currentEntry.GetSamplers())
            {
                meshMaterial.UpdateSampler(pair.Key, pair.Value.ToSamplerData());
            }

            foreach (var pair in currentEntry.GetRasterizerStates())
            {
                SetRasterizerState(meshMaterial, pair.Value);
            }

            foreach (var pair in currentEntry.GetBlendStates())
            {
                SetBlendState(meshMaterial, pair.Value);
            }

            return meshMaterial;
        }

        private static void SetRasterizerState(RMaterial meshMaterial, MatlAttribute.MatlRasterizerState rasterizerState)
        {
            meshMaterial.DepthBias = rasterizerState.DepthBias;
            meshMaterial.CullMode = rasterizerState.CullMode.ToOpenTk();
            meshMaterial.EnableFaceCulling = rasterizerState.CullMode != MatlCullMode.None;
            meshMaterial.FillMode = rasterizerState.FillMode.ToOpenTk();
        }

        private static void SetBlendState(RMaterial meshMaterial, MatlAttribute.MatlBlendState blendState)
        {
            meshMaterial.SourceColor = blendState.SourceColor.ToOpenTk();
            meshMaterial.DestinationColor = blendState.DestinationColor.ToOpenTk();

            // TODO: Unk6?

            // TODO: Do both need to be set?
            meshMaterial.UseAlphaSampleCoverage = blendState.Unk7 == 1 || blendState.Unk8 == 1;
        }
    }
}
