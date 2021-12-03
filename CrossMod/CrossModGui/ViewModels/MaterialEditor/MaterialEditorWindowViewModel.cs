using CrossMod.MaterialValidation;
using CrossMod.Nodes;
using CrossMod.Rendering;
using CrossMod.Rendering.GlTools;
using CrossMod.Tools;
using CrossModGui.Tools;
using SsbhLib.MatlXml;
using SsbhLib.Tools;
using SSBHLib.Formats;
using SSBHLib.Formats.Materials;
using SSBHLib.Formats.Meshes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class MaterialEditorWindowViewModel : ViewModelBase
    {
        public ObservableCollection<MaterialCollection> MaterialCollections { get; } = new ObservableCollection<MaterialCollection>();

        public MaterialCollection? CurrentMaterialCollection
        {
            get => currentMaterialCollection;
            set 
            { 
                currentMaterialCollection = value;
                // Reset the selected material since the containing collection is no longer selected.
                CurrentMaterial = currentMaterialCollection?.Materials.FirstOrDefault();
            }
        }
        private MaterialCollection? currentMaterialCollection;

        public Material? CurrentMaterial { get; set; }

        public event EventHandler? RenderFrameNeeded;

        public class MaterialSaveEventArgs : EventArgs
        {
            public MaterialCollection MaterialCollection { get; }
            public string FilePath { get; }

            public MaterialSaveEventArgs(MaterialCollection materialCollection, string filePath)
            {
                MaterialCollection = materialCollection;
                FilePath = filePath;
            }
        }

        // TODO: This could contain additional metadata like IsSrgb or IsCubeMap.
        // This would allow providing more detailed feedback about potential model issues.
        public ObservableCollection<string> PossibleTextureNames { get; } = new ObservableCollection<string>();

        public Dictionary<MatlCullMode, string> DescriptionByCullMode { get; } = new Dictionary<MatlCullMode, string>
        {
            { MatlCullMode.Back, "Back" },
            { MatlCullMode.Front, "Front" },
            { MatlCullMode.None, "None" },
        };

        public Dictionary<MatlBlendFactor, string> DescriptionByBlendFactor { get; } = new Dictionary<MatlBlendFactor, string>
        {
            { MatlBlendFactor.Zero, "Zero" },
            { MatlBlendFactor.One, "One" },
            { MatlBlendFactor.SourceAlpha, "Source Alpha" },
            { MatlBlendFactor.DestinationAlpha, "Destination Alpha" },
            { MatlBlendFactor.SourceColor, "Source Color" },
            { MatlBlendFactor.DestinationColor, "Destination Color" },
            { MatlBlendFactor.OneMinusSourceAlpha, "One Minus Source Alpha" },
            { MatlBlendFactor.OneMinusDestinationAlpha, "One Minus Destination Alpha" },
            { MatlBlendFactor.OneMinusSourceColor, "One Minus Source Color" },
            { MatlBlendFactor.OneMinusDestinationColor, "One Minus Destination Color" },
            { MatlBlendFactor.SourceAlphaSaturate, "Source Alpha Saturate" },
        };

        public Dictionary<int, string> DescriptionByAnisotropy { get; } = new Dictionary<int, string>
        {
            { 0, "1x" },
            { 2, "2x" },
            { 4, "4x" },
            { 8, "16x" },
            { 16, "128x" },
        };

        public Dictionary<MatlFillMode, string> DescriptionByFillMode { get; } = new Dictionary<MatlFillMode, string>
        {
            { MatlFillMode.Solid, "Solid" },
            { MatlFillMode.Line, "Line" },
        };

        public Dictionary<MatlMagFilter, string> DescriptionByMagFilter { get; } = new Dictionary<MatlMagFilter, string>
        {
            { MatlMagFilter.Nearest, "Nearest" },
            { MatlMagFilter.Linear, "Linear" },
            { MatlMagFilter.Linear2, "Linear + ???" },
        };

        public Dictionary<MatlMinFilter, string> DescriptionByMinFilter { get; } = new Dictionary<MatlMinFilter, string>
        {
            { MatlMinFilter.Nearest, "Nearest" },
            { MatlMinFilter.LinearMipmapLinear, "Linear Mipmap Linear" },
            { MatlMinFilter.LinearMipmapLinear2, "Linear Mipmap Linear2" },
        };

        public Dictionary<FilteringType, string> DescriptionByFilteringType { get; } = new Dictionary<FilteringType, string>
        {
            { FilteringType.Default, "Default" },
            { FilteringType.Default2, "Default2" },
            { FilteringType.AnisotropicFiltering, "Anisotropic Filtering" },
        };

        public Dictionary<MatlWrapMode, string> DescriptionByWrapMode { get; } = new Dictionary<MatlWrapMode, string>
        {
            { MatlWrapMode.Repeat, "Repeat" },
            { MatlWrapMode.ClampToEdge, "Clamp to Edge" },
            { MatlWrapMode.MirroredRepeat, "Mirrored Repeat" },
            { MatlWrapMode.ClampToBorder, "Clamp to Border" },
        };

        public MaterialEditorWindowViewModel(IEnumerable<FileNode> nodes)
        {

            // TODO: Restrict the textures used for cube maps.
            foreach (var name in TextureAssignment.defaultTexturesByName.Keys)
                PossibleTextureNames.Add(name);

            // TODO: Store the models for later?
            // Group materials by matl.
            var models = FindModels(nodes);
            foreach (var model in models)
            {
                // TODO: Add texture names?
                var collection = CreateMaterialCollection(model.Item1, new Dictionary<string, RMaterial>(), model.Item2);
                MaterialCollections.Add(collection);
            }
        }

        private static List<(string, Matl, Mesh?, Modl?)> FindModels(IEnumerable<FileNode> nodes)
        {
            var models = new List<(string, Matl, Mesh?, Modl?)>();

            // TODO: The code to find nodes by types is duplicated somewhere?
            foreach (var node in nodes)
            {
                if (node is DirectoryNode directory)
                {
                    FindModels(models, directory);
                }
            }

            return models;
        }

        private static void FindModels(List<(string, Matl, Mesh?, Modl?)> models, DirectoryNode directory)
        {
            Matl? matl = null;
            Modl? modl = null;
            Mesh? mesh = null;

            // Use the folder name for the associated files.
            string name = directory.Text;

            // Find material related files in this directory.
            foreach (var child in directory.Nodes)
            {
                if (child is NumatbNode numatb)
                {
                    matl = numatb.Material;
                }
                else if (child is NumshbNode numshb)
                {
                    mesh = numshb.mesh;
                }
                else if (child is NumdlbNode numdlb)
                {
                    modl = numdlb.modl;
                }
            }

            // Skip folders that don't contain a numatb.
            if (matl != null)
            {
                models.Add((name, matl, mesh, modl));
            }

            // Recurse on any subfolders.
            foreach (var child in directory.Nodes)
            {
                if (child is DirectoryNode childDirectory)
                    FindModels(models, childDirectory);
            }
        }

        public void ApplyPreset(MaterialPreset? selectedPreset)
        {
            if (selectedPreset == null)
                return;

            // Find the entry for the selected preset.
            if (MaterialPresets.MaterialPresets.Presets.Value == null)
                return;

            var presetMaterial = MaterialPresets.MaterialPresets.Presets.Value.Entries
                .Where(e => e.MaterialLabel == selectedPreset.Name)
                .FirstOrDefault();

            if (presetMaterial == null)
                return;

            if (CurrentMaterial == null || CurrentMaterialCollection == null)
                return;

            Matl? currentMatl = null;
            if (currentMatl == null)
                return;

            var currentEntryIndex = Array.FindIndex(currentMatl.Entries, 0, (e) => e.MaterialLabel == CurrentMaterial.Name);
            if (currentEntryIndex == -1)
                return;

            // Apply the preset and update the matl entry.
            var newEntry = MatlTools.FromShaderAndAttributes(currentMatl.Entries[currentEntryIndex], presetMaterial, true);
            currentMatl.Entries[currentEntryIndex] = newEntry;

            // store the result of applying the preset's entry to the current material's entry
            var currentMaterialIndex = CurrentMaterialCollection.Materials.IndexOf(CurrentMaterial);
            if (currentMaterialIndex == -1)
                return;

            // TODO: How to sync the viewmodels with the viewport?

            // Recreate and reassign materials to refresh rendering.
            // Do this first to ensure the RMaterials are recreated.
            //NumdlbNode.InitializeAndAssignMaterials(currentRnumdl.RenderModel, currentMatl, currentRnumdl.TextureByName, currentRnumdl.Modl);

            // Update the view model.
            //currentRnumdl.MaterialByName.TryGetValue(newEntry.MaterialLabel, out RMaterial? rMaterial);

            // Create the new viewmodel material and sync it with the newly created render material.
            //var newMaterial = CreateMaterial(newEntry, currentEntryIndex, rMaterial);
            //CurrentMaterialCollection.Materials[currentMaterialIndex] = newMaterial;
            //CurrentMaterial = newMaterial;

            OnRenderFrameNeeded();
        }

        private MaterialCollection CreateMaterialCollection(string name, Dictionary<string, RMaterial> materialByName, Matl matl)
        {
            var collection = new MaterialCollection(name);
            for (int i = 0; i < matl.Entries.Length; i++)
            {
                // Pass a reference to the render material to enable real time updates.
                materialByName.TryGetValue(matl.Entries[i].MaterialLabel, out RMaterial? rMaterial);

                var material = CreateMaterial(matl.Entries[i], i, rMaterial);
                collection.Materials.Add(material);
            }

            return collection;
        }

        private Material CreateMaterial(MatlEntry entry, int index, RMaterial? rMaterial)
        {
            var material = CreateMaterial(entry, index);

            // Enable real time viewport updates.
            if (rMaterial != null)
            {
                SyncViewModelToRMaterial(rMaterial, material);
            }

            return material;
        }

        private static Material CreateMaterial(MatlEntry entry, int index, Modl? modl = null, Mesh? mesh = null)
        {
            var idColor = UniqueColors.IndexToColor(index);

            var material = new Material(entry)
            {
                MaterialIdColor = new SolidColorBrush(Color.FromArgb(255,
                    (byte)idColor.X,
                    (byte)idColor.Y,
                    (byte)idColor.Z)),
                ShaderAttributeNames = string.Join(", ", ShaderValidation.GetAttributes(entry.ShaderLabel)),
                ShaderParameterNames = string.Join(", ", ShaderValidation.GetParameters(entry.ShaderLabel).Select(p => p.ToString()).ToList()),
                RenderPasses = ShaderValidation.GetRenderPasses(entry.ShaderLabel),
                AttributeErrors = GetAttributeErrors(entry.ShaderLabel, entry.MaterialLabel, modl, mesh),
                IsNotValidShaderLabel = !ShaderValidation.IsValidShaderLabel(entry.ShaderLabel)
            };
            AddAttributesToMaterial(entry, material);
            return material;
        }

        private static List<AttributeError> GetAttributeErrors(string shaderLabel, string materialLabel, Modl? modl, Mesh? mesh)
        {
            // Create a list of missing required attributes for each mesh object.
            if (mesh == null || modl == null)
                return new List<AttributeError>();

            var required = ShaderValidation.GetAttributes(shaderLabel);
            var errors = new List<AttributeError>();
            foreach (var meshObject in mesh.Objects)
            {
                // Only include errors for meshes with this material assigned.
                var modlEntry = modl.ModelEntries.FirstOrDefault(e => e.MeshName == meshObject.Name && e.SubIndex == meshObject.SubIndex);
                if (modlEntry == null)
                    continue;

                if (modlEntry.MaterialLabel != materialLabel)
                    continue;

                var current = meshObject.Attributes.Select(a => a.AttributeStrings[0].Text);
                var missingAttributes = string.Join(", ", required.Except(current).ToList());
                if (string.IsNullOrEmpty(missingAttributes))
                    continue;

                errors.Add(new AttributeError { MeshName = meshObject.Name, MissingAttributes = missingAttributes });
            }
            return errors;
        }

        public void OnRenderFrameNeeded()
        {
            RenderFrameNeeded?.Invoke(this, EventArgs.Empty);
        }

        // TODO: Move this logic to the respective ViewModel classes?
        private void SyncViewModelToRMaterial(RMaterial rMaterial, Material material)
        {
            material.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Material.ShaderLabel):
                        rMaterial.ShaderLabel = material.ShaderLabel;
                        OnRenderFrameNeeded();
                        break;
                    default:
                        break;
                }
            };
            SyncBooleans(rMaterial, material);
            SyncFloats(rMaterial, material);
            SyncTexturesSamplers(rMaterial, material);
            SyncVectors(rMaterial, material);
            SyncBlendState(rMaterial, material);
            SyncRasterizerState(rMaterial, material);
        }

        private void SyncRasterizerState(RMaterial rMaterial, Material material)
        {
            if (material.RasterizerState0 != null)
            {
                material.RasterizerState0.PropertyChanged += (s, e) =>
                {
                    rMaterial.CullMode = material.RasterizerState0.CullMode.ToOpenTk();
                    rMaterial.EnableFaceCulling = material.RasterizerState0.CullMode != MatlCullMode.None;
                    rMaterial.FillMode = material.RasterizerState0.FillMode.ToOpenTk();
                    OnRenderFrameNeeded();
                };
            }
        }

        private void SyncBlendState(RMaterial rMaterial, Material material)
        {
            if (material.BlendState0 != null)
            {
                material.BlendState0.PropertyChanged += (s, e) =>
                {
                    rMaterial.SourceColor = material.BlendState0.SourceColor.ToOpenTk();
                    rMaterial.DestinationColor = material.BlendState0.DestinationColor.ToOpenTk();
                    rMaterial.EnableAlphaSampleCoverage = material.BlendState0.EnableAlphaSampleToCoverage;
                    OnRenderFrameNeeded();
                };
            }
        }

        private void SyncBooleans(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.BooleanParams)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateBoolean(paramId, param.Value);
                    OnRenderFrameNeeded();
                };
            }
        }

        private void SyncFloats(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.FloatParams)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateFloat(paramId, param.Value);
                    OnRenderFrameNeeded();
                };
            }
        }

        private void SyncTexturesSamplers(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.TextureParams)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                if (!Enum.TryParse(param.SamplerParamId, out MatlEnums.ParamId samplerParamId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateTexture(paramId, param.Value);
                    rMaterial.UpdateSampler(samplerParamId, GetSamplerData(param));
                    OnRenderFrameNeeded();
                };
            }
        }

        private void SyncVectors(RMaterial rMaterial, Material material)
        {
            foreach (var param in material.Vec4Params)
            {
                if (!Enum.TryParse(param.ParamId, out MatlEnums.ParamId paramId))
                    continue;

                param.PropertyChanged += (s, e) =>
                {
                    rMaterial.UpdateVec4(paramId, new OpenTK.Vector4(param.Value1, param.Value2, param.Value3, param.Value4));
                    OnRenderFrameNeeded();
                };
            }
        }

        private static SamplerData GetSamplerData(TextureSamplerParam param)
        {
            return new SamplerData
            {
                MinFilter = param.MinFilter.ToOpenTk(),
                MagFilter = param.MagFilter.ToOpenTk(),
                WrapS = param.WrapS.ToOpenTk(),
                WrapT = param.WrapT.ToOpenTk(),
                WrapR = param.WrapR.ToOpenTk(),
                LodBias = param.LodBias,
                // Disable anisotropic filtering if it's disabled in the material.
                MaxAnisotropy = param.TextureFilteringType == FilteringType.AnisotropicFiltering ? param.MaxAnisotropy : 1,
            };
        }

        private static void AddAttributesToMaterial(MatlEntry source, Material destination)
        {
            // There should only be a single rasterizer state in each material.
            if (source.GetRasterizerStates().TryGetValue(MatlEnums.ParamId.RasterizerState0, out MatlAttribute.MatlRasterizerState? rasterizerState))
            {
                destination.RasterizerState0 = new RasterizerStateParam(rasterizerState);
            }

            // There should only be a single blend state in each material.
            if (source.GetBlendStates().TryGetValue(MatlEnums.ParamId.BlendState0, out MatlAttribute.MatlBlendState? blendState))
            {
                destination.BlendState0 = new BlendStateParam(blendState);
            }

            destination.FloatParams.AddRange(source.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Float)
                .Select(a => new FloatParam(a)));

            destination.BooleanParams.AddRange(source.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Boolean)
                .Select(a => new BooleanParam(a)));

            destination.Vec4Params.AddRange(source.Attributes
                .Where(a => a.DataType == MatlEnums.ParamDataType.Vector4)
                .Select(a => new Vec4Param(a)));

            AddTextures(source, destination);
        }

        private static void AddTextures(MatlEntry source, Material destination)
        {
            // TODO: Are texture names case sensitive?
            foreach (var texture in source.Attributes.Where(a => a.DataType == MatlEnums.ParamDataType.String))
            {
                // TODO: Handle the case where samplers are missing?
                var sampler = source.Attributes.SingleOrDefault(a => a.ParamId == ParamIdExtensions.GetSampler(texture.ParamId));
                if (sampler == null)
                    continue;

                destination.TextureParams.Add(new TextureSamplerParam(texture, sampler));
            }
        }

        public void SaveMatl(string outputPath)
        {
            if (CurrentMaterialCollection != null)
            {
                //var matl = rnumdls.SingleOrDefault(r => r.Item1 == CurrentMaterialCollection.Name).Item2.Matl;
                //if (matl != null)
                //{
                //    SSBHLib.Ssbh.TrySaveSsbhFile(outputPath, matl);
                //}
            }
        }

        internal void ExportMatlToXml(string outputPath)
        {
            if (CurrentMaterialCollection != null)
            {
                //var matl = rnumdls.SingleOrDefault(r => r.Item1 == CurrentMaterialCollection.Name).Item2.Matl;
                //if (matl != null)
                //{
                //    using var writer = new StringWriter();
                //    MatlSerialization.SerializeMatl(writer, matl);
                //    File.WriteAllText(outputPath, writer.ToString());
                //}
            }
        }
    }
}
