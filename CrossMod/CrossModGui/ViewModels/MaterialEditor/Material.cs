using SSBHLib.Formats.Materials;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class Material : ViewModelBase
    {
        public string Name
        {
            get => entry.MaterialLabel;
            set
            {
                entry.MaterialLabel = value;
                OnPropertyChanged();
            }
        }

        public string ShaderLabel 
        { 
            get => entry.ShaderLabel; 
            set
            {
                entry.ShaderLabel = value;
                OnPropertyChanged();
            }
        }

        // TODO: Selected render pass should be based on shader label.
        public string SelectedRenderPass
        {
            get => entry.ShaderLabel.Substring(Math.Min(25, entry.ShaderLabel.Length));
            set
            {
                entry.ShaderLabel = value;
                OnPropertyChanged();
            }
        }

        public List<string> RenderPasses { get; set; } = new List<string>();

        public SolidColorBrush? MaterialIdColor { get; set; }

        public RasterizerStateParam? RasterizerState0 { get; set; }
        public BlendStateParam? BlendState0 { get; set; }

        public string ShaderAttributeNames { get; set; } = "";
        public string ShaderParameterNames { get; set; } = "";

        public ObservableCollection<BooleanParam> BooleanParams { get; } = new ObservableCollection<BooleanParam>();

        public ObservableCollection<FloatParam> FloatParams { get; } = new ObservableCollection<FloatParam>();

        public ObservableCollection<Vec4Param> Vec4Params { get; } = new ObservableCollection<Vec4Param>();

        public ObservableCollection<TextureSamplerParam> TextureParams { get; } = new ObservableCollection<TextureSamplerParam>();

        private readonly MatlEntry entry;

        public Material(MatlEntry entry)
        {
            this.entry = entry;
        }
    }
}
