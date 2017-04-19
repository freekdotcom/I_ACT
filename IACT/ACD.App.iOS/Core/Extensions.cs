using System;
using CoreGraphics;
using System.Threading.Tasks;
using Foundation;
using ObjCRuntime;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using NPoint = NGraphics.Point;

namespace ACD
{
    public static partial class Extensions
    {
        /* public static UIView ToNative(this View view, CGRect size)
        {
            var renderer = RendererFactory.GetRenderer(view);

            var packager = new VisualElementPackager(renderer);
            packager.Load();

            renderer.NativeView.Frame = size;

            renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            renderer.Element.Layout(size.ToRectangle());

            var nativeView = renderer.NativeView;

            nativeView.SetNeedsLayout();

            return nativeView;
        }

        public static string RecursiveDescription(this UIView view)
        {
            return new NSString(Messaging.IntPtr_objc_msgSend(view.Handle, new Selector("recursiveDescription").Handle)).ToString();
        } */

        public static UIViewController TopMostViewController(this UIViewController self)
        {
            if (self == null)
                return null;

            if (self is UITabBarController)
                return (self as UITabBarController).SelectedViewController.TopMostViewController() ?? self;

            if (self is UINavigationController)
                return (self as UINavigationController).VisibleViewController.TopMostViewController() ?? self;

            // Handling Modal views
            var presentedViewController = self.PresentedViewController;
            if (presentedViewController != null)
            {
                return presentedViewController.TopMostViewController();
            }
            // Handling UIViewController's added as subviews to some other views.
            else 
            {
                foreach (var view in self.View.Subviews)
                {
                    // Key property which most of us are unaware of / rarely use.
                    var subViewController = view.NextResponder;
                    if (subViewController != null && subViewController is UIViewController)
                    {
                        var viewController = subViewController as UIViewController;
                        return viewController.TopMostViewController();
                    }
                }
                return self;
            }
        }

        public static async Task<NGraphics.IImage> ToIImage(this FileImageSource src)
        {
            var handler = new FileImageSourceHandler();
            var image = await handler.LoadImageAsync(src);
            return new NGraphics.CGImageImage(image.CGImage, 1);
        }
    }
}

