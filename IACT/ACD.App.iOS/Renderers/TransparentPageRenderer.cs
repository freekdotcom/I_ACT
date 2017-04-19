using System;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using ACD.App.iOS;

[assembly: ExportRendererAttribute(typeof(TransparentPage), typeof(TransparentPageRenderer))]

namespace ACD.App.iOS
{
    public class TransparentPage : ContentPage
    {
        public TransparentPage()
        {
            BackgroundColor = Color.Transparent;
        }
    }

    public class TransparentPageRenderer : PageRenderer
    {
        /* public override void WillMoveToParentViewController(UIViewController parent)
        {
            base.WillMoveToParentViewController(parent);

            if (parent != null)
            {
                parent.ModalPresentationStyle = ModalPresentationStyle;
            }
        } */

        public override void DidMoveToParentViewController(UIViewController parent)
        {
            base.DidMoveToParentViewController(parent);

            if (ParentViewController != null)
            {
                //ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
                //ParentViewController.ModalPresentationStyle = UIModalPresentationStyle.OverCurrentContext;
                //ParentViewController.PresentingViewController.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            View.Superview.BackgroundColor = UIColor.Clear;

            var pvc = ParentViewController?.PresentingViewController;
            if (pvc != null)
            {
                //pvc.DefinesPresentationContext = true;
                //pvc.ModalPresentationStyle = UIModalPresentationStyle.CurrentContext;
            }
        }
    }
}

