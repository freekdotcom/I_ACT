using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace ACD.App.Droid
{
    [Activity(Label = "ACD", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        ScreenOrientation = ScreenOrientation.Portrait, Theme = "@style/AppTheme", Icon = "@drawable/icon")]
    public class MainActivity : FormsApplicationActivity
    {
        public const string StartTip = "START_TIP";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

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

