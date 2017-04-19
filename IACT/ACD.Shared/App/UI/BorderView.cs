using System;
using Xamarin.Forms;

namespace ACD
{
    public class BorderView : BoxView
    {
        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create<BorderView, Color>(p => p.BorderColor, Color.Transparent);

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }

        public static readonly BindableProperty ThicknessProperty =
            BindableProperty.Create<BorderView, Thickness>(p => p.Thickness, new Thickness());

        public Thickness Thickness
        {
            get { return (Thickness)GetValue(ThicknessProperty); }
            set { SetValue(ThicknessProperty, value); }
        }
    }
}

