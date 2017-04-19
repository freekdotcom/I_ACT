using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using ACD.App.Droid;

[assembly: ExportRenderer(typeof(Button), typeof(ListButtonRenderer))]

namespace ACD.App.Droid
{
    public class ListButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            Control.Focusable = false;
        }
    }
}