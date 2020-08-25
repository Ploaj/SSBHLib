using CrossMod.Rendering;
using CrossModGui.Tools;
using SSBHLib;
using SSBHLib.Formats.Materials;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrossModGui.ViewModels
{
    public class MaterialEditorWindowViewModel : ViewModelBase
    {
        public class BooleanParam : ViewModelBase
        {
            public string ParamId { get; set; }
            public bool Value { get; set; }
        }

        public class FloatParam : ViewModelBase
        {
            public string ParamId { get; set; }
            public float Value { get; set; }
            public float Min { get; set; } = 0.0f;
            public float Max { get; set; } = 1.0f;
        }

        public class Vec4Param : ViewModelBase
        {

            public string ParamId { get; set; }
            public Brush ColorBrush { get; set; }

            public string Label1 { get; set; } = "X";
            public float Min1 { get; set; } = 0.0f;
            public float Max1 { get; set; } = 1.0f;
            public float Value1
            {
                get => value1;
                set
                {
                    value1 = value;
                    UpdateColor();
                }
            }
            private float value1;

            public string Label2 { get; set; } = "Y";
            public float Min2 { get; set; } = 0.0f;
            public float Max2 { get; set; } = 1.0f;
            public float Value2
            {
                get => value2;
                set
                {
                    value2 = value;
                    UpdateColor();
                }
            }
            private float value2;

            public string Label3 { get; set; } = "Z";
            public float Min3 { get; set; } = 0.0f;
            public float Max3 { get; set; } = 1.0f;
            public float Value3
            {
                get => value3;
                set
                {
                    value3 = value;
                    UpdateColor();
                }
            }
            private float value3;

            public string Label4 { get; set; } = "W";
            public float Min4 { get; set; } = 0.0f;
            public float Max4 { get; set; } = 1.0f;
            public float Value4
            {
                get => value4; 
                set 
                { 
                    value4 = value; 
                    UpdateColor(); 
                } 
            }
            private float value4;

            private void UpdateColor()
            {
                // TODO: The linear -> srgb conversion should be handled by a library.
                var gamma = 0.4545;
                var red = (float)System.Math.Pow(Value1, gamma);
                var green = (float)System.Math.Pow(Value2, gamma);
                var blue = (float)System.Math.Pow(Value3, gamma);
                var Color = SFGraphics.Utils.ColorUtils.GetColor(red, green, blue);
                ColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, Color.R, Color.G, Color.B));
            }
        }

        public class TextureParam : ViewModelBase
        {
            public string ParamId { get; set; }
            public string Value { get; set; }
            public ImageSource Image { get; set; }
        }

        public class Material : ViewModelBase
        {
            public string Name { get; set; }

            public string ShaderLabel { get; set; }

            public SolidColorBrush MaterialIdColor { get; set; }

            public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

            public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

            public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

            public ObservableCollection<TextureParam> TextureParams { get; } = new ObservableCollection<TextureParam>();
        }

        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();

        public Material CurrentMaterial { get; set; }

        public ObservableCollection<string> PossibleTextureNames { get; } = new ObservableCollection<string>();

        // TODO: Does this reference need to be in the view model?
        private readonly RNumdl rnumdl;

        public MaterialEditorWindowViewModel(RNumdl rnumdl)
        {
            if (rnumdl == null)
                return;

            this.rnumdl = rnumdl;

            foreach (var name in rnumdl.TextureByName.Keys)
                PossibleTextureNames.Add(name);
            // TODO: Restrict the textures used for cube maps.
            foreach (var name in CrossMod.Rendering.GlTools.RMaterial.DefaultTexturesByName.Keys)
                PossibleTextureNames.Add(name);

            // TODO: Allow for editing all matl entries and not just materials assigned to meshes.
            foreach (var glMaterial in rnumdl.MaterialByName.Values)
            {
                var material = new Material { 
                    Name = glMaterial.MaterialLabel, 
                    ShaderLabel = glMaterial.ShaderLabel,
                    MaterialIdColor = new SolidColorBrush(Color.FromArgb(255, 
                        (byte)glMaterial.MaterialIdColorRgb255.X, 
                        (byte)glMaterial.MaterialIdColorRgb255.Y, 
                        (byte)glMaterial.MaterialIdColorRgb255.Z))
                };

                AddBooleanParams(glMaterial, material);
                AddFloatParams(glMaterial, material);
                AddVec4Params(glMaterial, material);
                AddTextureParams(glMaterial, material);

                Materials.Add(material);
            }
        }

        public void SaveMatl(string outputPath)
        {
            // TODO: This probably doesn't need to be part of the viewmodel.
            if (rnumdl == null || rnumdl.Material == null)
                return;

            foreach (var entry in rnumdl.Material.Entries)
            {
                // TODO: This only checks materials that are already assigned to a mesh.
                if (!rnumdl.MaterialByName.ContainsKey(entry.MaterialLabel))
                    continue;

                var material = rnumdl.MaterialByName[entry.MaterialLabel];
                foreach (var attribute in entry.Attributes)
                {
                    // The data type isn't known, so check each type.
                    if (material.floatByParamId.ContainsKey(attribute.ParamId))
                    {
                        attribute.DataObject = material.floatByParamId[attribute.ParamId];
                    }
                    else if (material.boolByParamId.ContainsKey(attribute.ParamId))
                    {
                        attribute.DataObject = material.boolByParamId[attribute.ParamId];
                    }
                    else if (material.vec4ByParamId.ContainsKey(attribute.ParamId))
                    {
                        var value = material.vec4ByParamId[attribute.ParamId];
                        attribute.DataObject = new MatlAttribute.MatlVector4 { X = value.X, Y = value.Y, Z = value.Z, W = value.W };
                    }
                    else if (material.textureNameByParamId.ContainsKey(attribute.ParamId))
                    {
                        attribute.DataObject = new MatlAttribute.MatlString { Text = material.textureNameByParamId[attribute.ParamId] };
                    }
                }
            }

            Ssbh.TrySaveSsbhFile(outputPath, rnumdl.Material);
        }

        private static void AddTextureParams(CrossMod.Rendering.GlTools.RMaterial mat, Material material)
        {
            foreach (var param in mat.textureNameByParamId)
            {
                // TODO: Don't create a new bitmap every time.
                // Create a thumbnail icon.
                //var image = GetPreviewImage(mat, param.Key);

                var textureParam = new TextureParam { ParamId = param.Key.ToString(), Value = param.Value };

                // Update the material for rendering.
                textureParam.PropertyChanged += (s, e) => mat.UpdateTexture(param.Key, (s as TextureParam).Value);

                material.TextureParams.Add(textureParam);
            }
        }

        // TODO: Nutex previews.
        private static WriteableBitmap GetPreviewImage(CrossMod.Rendering.GlTools.RMaterial mat, MatlEnums.ParamId paramId)
        {
            // null values will be replaced with a default image in the view.
            if (!mat.textureNameByParamId.ContainsKey(paramId))
                return null;
            if (!mat.TextureByName.ContainsKey(mat.textureNameByParamId[paramId]))
                return null;

            var rTexture = mat.TextureByName[mat.textureNameByParamId[paramId]];
            var image = Converters.BitmapToWpfBitmap.CreateBitmapImage(rTexture.SfTexture.Width, rTexture.SfTexture.Height, rTexture.BitmapImageData);
            return image;
        }

        private static void AddVec4Params(CrossMod.Rendering.GlTools.RMaterial mat, Material material)
        {
            foreach (var param in mat.vec4ByParamId)
            {
                var vec4Param = new Vec4Param
                {
                    ParamId = param.Key.ToString(),
                    Value1 = param.Value.X,
                    Value2 = param.Value.Y,
                    Value3 = param.Value.Z,
                    Value4 = param.Value.W,
                };

                TryAssignValuesFromDescription(vec4Param);

                // Update the material for rendering.
                vec4Param.PropertyChanged += (s, e) =>
                {
                    switch (e.PropertyName)
                    {
                        case nameof(Vec4Param.Value1):
                        case nameof(Vec4Param.Value2):
                        case nameof(Vec4Param.Value3):
                        case nameof(Vec4Param.Value4):
                            var sender = s as Vec4Param;
                            mat.UpdateVec4(param.Key, new OpenTK.Vector4(sender.Value1, sender.Value2, sender.Value3, sender.Value4));
                            break;
                        default:
                            break;
                    }
                };

                material.Vec4Params.Add(vec4Param);
            }
        }

        private static void TryAssignValuesFromDescription(Vec4Param vec4Param)
        {
            if (MaterialParamDescriptions.Instance.ParamDescriptionsByName.TryGetValue(vec4Param.ParamId,
                out MaterialParamDescriptions.ParamDescription description))
            {
                vec4Param.Label1 = description.Label1 ?? "Unused";
                vec4Param.Min1 = description.Min1.GetValueOrDefault(0);
                vec4Param.Max1 = description.Max1.GetValueOrDefault(1);

                vec4Param.Label2 = description.Label2 ?? "Unused";
                vec4Param.Min2 = description.Min2.GetValueOrDefault(0);
                vec4Param.Max2 = description.Max2.GetValueOrDefault(1);

                vec4Param.Label3 = description.Label3 ?? "Unused";
                vec4Param.Min3 = description.Min3.GetValueOrDefault(0);
                vec4Param.Max3 = description.Max3.GetValueOrDefault(1);

                vec4Param.Label4 = description.Label4 ?? "Unused";
                vec4Param.Min4 = description.Min4.GetValueOrDefault(0);
                vec4Param.Max4 = description.Max4.GetValueOrDefault(1);
            }
        }

        private static void AddFloatParams(CrossMod.Rendering.GlTools.RMaterial mat, Material material)
        {
            foreach (var param in mat.floatByParamId)
            {
                var floatParam = new FloatParam
                {
                    ParamId = param.Key.ToString(),
                    Value = param.Value
                };

                // Update the material for rendering.
                floatParam.PropertyChanged += (s, e) => mat.UpdateFloat(param.Key, (s as FloatParam).Value);

                material.FloatParams.Add(floatParam);
            }
        }

        private static void AddBooleanParams(CrossMod.Rendering.GlTools.RMaterial mat, Material material)
        {
            foreach (var param in mat.boolByParamId)
            {
                var boolParam = new BooleanParam
                {
                    ParamId = param.Key.ToString(),
                    Value = param.Value
                };

                // Update the material for rendering.
                boolParam.PropertyChanged += (s, e) => mat.UpdateBoolean(param.Key, (s as BooleanParam).Value);

                material.BooleanParams.Add(boolParam);
            }
        }
    }
}
