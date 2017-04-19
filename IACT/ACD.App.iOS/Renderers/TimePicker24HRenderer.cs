using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ACD.App.iOS;

[assembly: ExportRenderer(typeof(TimePicker), typeof(TimePicker24HRenderer))]

namespace ACD.App.iOS
{
    public class TimePicker24HRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            base.OnElementChanged(e);
            var timePicker = (UIDatePicker)Control.InputView;
            timePicker.Locale = new NSLocale("nl_nl");
        }
    }
}

