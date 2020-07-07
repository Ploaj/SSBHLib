using CrossMod.Rendering;
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

            foreach (var mat in rnumdl.MaterialByName.Values)
            {
                var material = new Material { Name = mat.Name };

                foreach (var param in mat.boolByParamId)
                {
                    material.BooleanParams.Add(new BooleanParam
                    {
                        Name = param.Key.ToString(),
                        Value = param.Value
                    });
                }

                foreach (var param in mat.floatByParamId)
                {
                    material.FloatParams.Add(new FloatParam
                    {
                        Name = param.Key.ToString(),
                        Value = param.Value
                    });
                }

                foreach (var param in mat.vec4ByParamId)
                {
                    material.Vec4Params.Add(new Vec4Param
                    {
                        Name = param.Key.ToString(),
                        Value1 = param.Value.X,
                        Value2 = param.Value.Y,
                        Value3 = param.Value.Z,
                        Value4 = param.Value.W,
                    });
                }
                // TODO: Texture params.
                // TODO: Possible texture names from rnumdl?
                // This may require reworking how textures are stored/loaded.

                Materials.Add(material);
            }
        }
    }
}
