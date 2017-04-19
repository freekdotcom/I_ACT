using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using ACD.App;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using ACD.App.Droid;

//[assembly: ExportRenderer(typeof(Checkbox), typeof(CheckboxRenderer))]

namespace ACD.App.Droid
{
    public class CheckboxRenderer : ViewRenderer<Checkbox, Android.Widget.CheckBox>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Checkbox> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            var cb = new Android.Widget.CheckBox(Context) { Checked = Element.Checked };
            cb.CheckedChange += (sender, args) => Element.Checked = args.IsChecked;
            SetNativeControl(cb);

            Control.Focusable = false;
            
            Element.Opacity = 1;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == Checkbox.CheckedProperty.PropertyName)
            {
                Control.Checked = Element.Checked;
            }

            Element.Opacity = 1;
        }
    }
}