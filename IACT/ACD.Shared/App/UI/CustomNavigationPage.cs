using System;
using System.Linq;

using Xamarin.Forms;

#if __ANDROID__
using Android;
using Activity = Android.App.Activity;
using Android.Views;
using Android.Content;

using Xamarin.Forms.Platform.Android;
#endif

namespace ACD.App
{
    public class CustomNavigationPage : NavigationPage
    {
        public CustomNavigationPage(Application app)
        {
            NavigationPage.SetBackButtonTitle(this, "Terug");

            PropertyChanged += (sender, e) => {
                if (e.PropertyName == CurrentPageProperty.PropertyName)
                    CurrentPageChanged();
                if (e.PropertyName == BarBackgroundColorProperty.PropertyName)
                    BarBackgroundColorChanged();
            };

            app.ModalPushed += (sender, e) => CurrentPageChanged();
            app.ModalPopped += (sender, e) => CurrentPageChanged();
        }

        public void CurrentPageChanged()
        {
            UpdateBarColor(Navigation.ModalStack.LastOrDefault() ?? CurrentPage);
        }

        void UpdateBarColor(Page page)
        {
            while (page is MultiPage<Page>)
                page = (page as MultiPage<Page>).CurrentPage;
            var type = page?.GetType();
            BarBackgroundColor = (Color)(type?.GetStatic<object>(BarBackgroundColorProperty.PropertyName) ?? BarBackgroundColor);
            BarTextColor = (Color)(type?.GetStatic<object>(BarTextColorProperty.PropertyName) ?? BarTextColor);
        }

        void BarBackgroundColorChanged()
        {
#if __ANDROID__
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
            {
                var activity = ((Activity)Forms.Context);
                var window = activity.Window;

                // clear FLAG_TRANSLUCENT_STATUS flag:
                window.ClearFlags(WindowManagerFlags.TranslucentStatus);

                // add FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS flag to the window
                window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);

                // finally change the color
                window.SetStatusBarColor(BarBackgroundColor.ToAndroid());
            }
#endif
        }
    }
}

