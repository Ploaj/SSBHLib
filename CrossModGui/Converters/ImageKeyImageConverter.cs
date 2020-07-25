using System;
using System.Drawing;
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
			switch (value.ToString())
			{
				case "animation":
					return animationIco;
				case "folder":
					return folderIco;
				case "material":
					return materialIco;
				case "mesh":
					return meshIco;
				case "model":
					return modelIco;
				case "skeleton":
					return skeletonIco;
				case "texture":
					return textureIco;
			}
			return unknownIco;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			// This conversion isn't needed.
			return null;
		}
	}
}
