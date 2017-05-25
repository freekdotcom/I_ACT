using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using NPoint = NGraphics.Point;

namespace ACD
{
    public partial class Extensions
    {

        public static async Task<NGraphics.IImage> ToIImage(this FileImageSource src)
        {
            var handler = new FileImageSourceHandler();
            var bitmap = await handler.LoadImageAsync(src, Forms.Context);
            return new NGraphics.BitmapImage(bitmap);
        }
    }
}
