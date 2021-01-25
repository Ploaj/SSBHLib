using SSBHLib.Formats.Materials;

namespace CrossModGui.ViewModels.MaterialEditor
{
    public class RasterizerStateParam : ViewModelBase
    {
        public MatlCullMode CullMode {
            get => rasterizerState.CullMode;
            set => rasterizerState.CullMode = value;
        }

        public MatlFillMode FillMode {
            get => rasterizerState.FillMode;
            set => rasterizerState.FillMode = value;
        }

        private readonly MatlAttribute.MatlRasterizerState rasterizerState;

        public RasterizerStateParam(MatlAttribute.MatlRasterizerState rasterizerState)
        {
            this.rasterizerState = rasterizerState;
        }
    }
}
