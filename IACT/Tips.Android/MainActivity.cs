using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace ACD.App.Droid
{
    [Activity(Label = "I-ACT", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const string START_TIP = "START_TIP";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Forms.Init(this, savedInstanceState);

            var start = int.Parse(Intent.GetStringExtra("open") ?? "0");
            MainApp.LaunchData = Intent.GetStringExtra("data");

            var app = new MainApp(start);
            LoadApplication(app);
        }

        public override void OnBackPressed()
        {
            if (MainApp.BackEnabled)
                base.OnBackPressed();
        }
    }
}
