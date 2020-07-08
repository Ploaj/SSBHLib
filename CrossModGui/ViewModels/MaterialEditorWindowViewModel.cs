using CrossMod.Rendering;
using SSBHLib.Formats.Materials;
using System.Collections.ObjectModel;

namespace CrossModGui.ViewModels
{
    public class MaterialEditorWindowViewModel : ViewModelBase
    {
        public class BooleanParam : ViewModelBase
        {
            public string Name { get; set; }
            public bool Value { get; set; }
        }

        public class FloatParam : ViewModelBase
        {
            public string Name { get; set; }
            public float Value { get; set; }
        }

        public class Vec4Param : ViewModelBase
        {
            public string Name { get; set; }
            public float Value1 { get; set; }
            public float Value2 { get; set; }
            public float Value3 { get; set; }
            public float Value4 { get; set; }
        }

        public class TextureParam : ViewModelBase
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class Material : ViewModelBase
        {
            public string Name { get; set; }

            public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

            public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

            public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

            public ObservableCollection<TextureParam> TextureParams { get; } = new ObservableCollection<TextureParam>();
        }

        public ObservableCollection<Material> Materials { get; } = new ObservableCollection<Material>();

        public Material CurrentMaterial { get; set; }

        public ObservableCollection<string> PossibleTextureNames { get; } = new ObservableCollection<string>();

        public MaterialEditorWindowViewModel()
        {

        }

        public MaterialEditorWindowViewModel(RNumdl rnumdl)
        {
            if (rnumdl == null)
                return;

            foreach (var name in rnumdl.TextureByName.Keys)
                PossibleTextureNames.Add(name);

            foreach (var glMaterial in rnumdl.MaterialByName.Values)
            {
                var material = new Material { Name = glMaterial.Name };

                AddBooleanParams(glMaterial, material);
                AddFloatParams(glMaterial, material);
                AddVec4Params(glMaterial, material);
                AddTextureParams(glMaterial, rnumdl, material);

                Materials.Add(material);
            }
        }

        private static void AddTextureParams(CrossMod.Rendering.GlTools.Material mat, RNumdl rnumdl, Material material)
        {
            // TODO: Get param ID and texture name from material.
            // TODO: Rework how textures are stored in the material.
            // TODO: Also add dummy texture as an option (#replace_cubemap, etc).
        }

        private static void AddVec4Params(CrossMod.Rendering.GlTools.Material mat, Material material)
        {
            foreach (var param in mat.vec4ByParamId)
            {
                var vec4Param = new Vec4Param
                {
                    Name = param.Key.ToString(),
                    Value1 = param.Value.X,
                    Value2 = param.Value.Y,
                    Value3 = param.Value.Z,
                    Value4 = param.Value.W,
                };

                // Update the material for rendering.
                vec4Param.PropertyChanged += (s, e) => 
                {
                    var sender = (s as Vec4Param);
                    mat.UpdateVec4(param.Key, new OpenTK.Vector4(sender.Value1, sender.Value2, sender.Value3, sender.Value4));
                };

                material.Vec4Params.Add(vec4Param);
            }
        }

        private static void AddFloatParams(CrossMod.Rendering.GlTools.Material mat, Material material)
        {
            foreach (var param in mat.floatByParamId)
            {
                var floatParam = new FloatParam
                {
                    Name = param.Key.ToString(),
                    Value = param.Value
                };

                // Update the material for rendering.
                floatParam.PropertyChanged += (s, e) => mat.UpdateFloat(param.Key, (s as FloatParam).Value);

                material.FloatParams.Add(floatParam);
            }
        }

        private static void AddBooleanParams(CrossMod.Rendering.GlTools.Material mat, Material material)
        {
            foreach (var param in mat.boolByParamId)
            {
                var boolParam = new BooleanParam
                {
                    Name = param.Key.ToString(),
                    Value = param.Value
                };

                // Update the material for rendering.
                boolParam.PropertyChanged += (s, e) => mat.UpdateBoolean(param.Key, (s as BooleanParam).Value);

                material.BooleanParams.Add(boolParam);
            }
        }
    }
}
