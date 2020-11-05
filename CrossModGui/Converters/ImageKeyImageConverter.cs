using System;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrossModGui.Converters
{
    [ValueConversion(typeof(string), typeof(ImageSource))]

    public class ImageKeyImageConverter : IValueConverter
    {
        private static readonly BitmapImage animationIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_animation.png", UriKind.Relative));
        private static readonly BitmapImage folderIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_folder.png", UriKind.Relative));
        private static readonly BitmapImage materialIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_material.png", UriKind.Relative));
        private static readonly BitmapImage meshIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_mesh.png", UriKind.Relative));
        private static readonly BitmapImage modelIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_model.png", UriKind.Relative));
        private static readonly BitmapImage skeletonIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_skeleton.png", UriKind.Relative));
        private static readonly BitmapImage textureIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_texture.png", UriKind.Relative));
        private static readonly BitmapImage unknownIco = new BitmapImage(new Uri(@"/CrossModGui;component/Resources/ico_unknown.png", UriKind.Relative));

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return unknownIco;

            return (value.ToString()) switch
            {
                "animation" => animationIco,
                "folder" => folderIco,
                "material" => materialIco,
                "mesh" => meshIco,
                "model" => modelIco,
                "skeleton" => skeletonIco,
                "texture" => textureIco,
                _ => unknownIco,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // This conversion isn't needed.
            return null;
        }
    }
}
