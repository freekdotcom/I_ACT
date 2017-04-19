using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using UIKit;
using ACD;
using ACD.App.iOS;

[assembly: ExportRendererAttribute(typeof(BorderView), typeof(BorderViewRenderer))]

namespace ACD.App.iOS
{
    public class BorderViewRenderer : BoxRenderer
    {
        public override void Draw(CGRect rect)
        {
            BorderView bv = (BorderView)this.Element;
            /* This is really just an elaborate cast to float. */
            var thickness = new CGRect(
                (float)bv.Thickness.Left,
                (float)bv.Thickness.Top,
                (float)(bv.Thickness.Right - bv.Thickness.Left),
                (float)(bv.Thickness.Bottom - bv.Thickness.Top)
            );
            using (var context = UIGraphics.GetCurrentContext())
            {
                context.SetFillColor(bv.BorderColor.ToCGColor());
                context.AddRects(new CGRect[] {
                    new CGRect(0, 0, thickness.Left, Bounds.Height),
                    new CGRect(Bounds.Width - thickness.Right, 0, thickness.Right, Bounds.Height),
                    new CGRect(0, 0, Bounds.Width, thickness.Top),
                    new CGRect(0, Bounds.Height - thickness.Bottom, Bounds.Width, thickness.Bottom)
                });
                context.DrawPath(CGPathDrawingMode.Fill);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == BorderView.BorderColorProperty.PropertyName || e.PropertyName == BorderView.ThicknessProperty.PropertyName)
                this.SetNeedsDisplay();
        }
    }
}
