using SSBHLib.Formats.Materials;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class BlendStateParam : ViewModelBase
    {
        public MatlBlendFactor SourceColor
        {
            get => blendState.SourceColor;
            set => blendState.SourceColor = value;
        }

        public MatlBlendFactor DestinationColor
        {
            get => blendState.DestinationColor;
            set => blendState.DestinationColor = value;
        }

        public bool EnableAlphaSampleToCoverage
        {
            get => blendState.EnableAlphaSampleToCoverage != 0;
            set => blendState.EnableAlphaSampleToCoverage = value ? 1 : 0;
        }

        private readonly MatlAttribute.MatlBlendState blendState;

        public BlendStateParam(MatlAttribute.MatlBlendState blendState)
        {
            this.blendState = blendState;
        }
    }
}
