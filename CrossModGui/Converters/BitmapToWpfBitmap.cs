using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CrossModGui.Converters
{
    class BitmapToWpfBitmap
    {
        public static WriteableBitmap CreateBitmapImage(int width, int height, byte[] imageDataBgra)
        {
            using (var memory = new MemoryStream(imageDataBgra))
            {
                var bitmapimage = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
                var rect = new Int32Rect(0, 0, width, height);
                var rowStrideInBytes = width * (PixelFormats.Bgr32.BitsPerPixel / 8);
                bitmapimage.WritePixels(rect, imageDataBgra, rowStrideInBytes, 0);

                return bitmapimage;
            }
        }
    }
}
