using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using CoreGraphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace ACD.App.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Forms.Init();
            NControl.iOS.NControlViewRenderer.Init();

            MessagingCenter.Subscribe<View, string>(this, "Share", (sender, args) => {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var filename = Path.Combine(documents, "profile.json");
                File.WriteAllText(filename, args);
                var activityItems = new NSObject[] { new NSUrl(filename, false) }; 
                var activityController = new UIActivityViewController(activityItems, null);

                var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;

                /* if(null != activityController.PopoverPresentationController)
                {
                    var view = Platform.GetRenderer(sender).NativeView;
                    var bounds = view.ConvertRectToView(view.Bounds, null);
                    bounds.Inflate(10, 10);
                    activityController.PopoverPresentationController.SourceView = topController.View;
                    activityController.PopoverPresentationController.SourceRect = bounds;
                } */

                topController.PresentViewController (activityController, true, () => {});
            });


            if (Preferences.GetOr("firstLaunch", true))
            {
                UIApplication.SharedApplication.CancelAllLocalNotifications();
                Preferences.Set("firstLaunch", false);
            }

            var application = new MainApp();

            LoadApplication(application);

            if (options != null && options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey)) {
                var notification = options[UIApplication.LaunchOptionsLocalNotificationKey] as UILocalNotification;
                if (notification != null) {
                    Device.StartTimer(TimeSpan.FromMilliseconds (10), () => {
                        iOSNotificationScheduler.Recieve(notification);
                        return false;
                    });
                }
            }

            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;

            return base.FinishedLaunching(app, options);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            iOSNotificationScheduler.Recieve(notification);
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }  

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
            LogUnhandledException(newExc);
        }  

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                const string errorFileName = "Fatal.log";
                var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
                var errorFilePath = Path.Combine(libraryPath, errorFileName);  
                var errorMessage = String.Format("Time: {0}\r\n{1}",
                    DateTime.Now, exception.ToString());
                File.WriteAllText(errorFilePath, errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }



		/* void OpenNotification(UILocalNotification notification)
		{
			try
			{
				int open = int.Parse((NSString)notification.UserInfo["open"]);
				MessagingCenter.Send<object, int>(this, CoachPage.NavigateMessage, open);
			}
			catch {}
		}

		public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
		{
			var data = (NSString)notification.UserInfo["data"];
			if (!string.IsNullOrWhiteSpace(data))
				App.LaunchData = data;

			if (application.ApplicationState == UIApplicationState.Active)
			{
				// show an alert
				var alert = new UIAlertView(notification.AlertAction, notification.AlertBody, null, "Later", (NSString)notification.UserInfo["action"]);
				alert.Clicked += (sender, e) =>
				{
					if (e.ButtonIndex != alert.CancelButtonIndex)
						OpenNotification(notification);
				};
				alert.Show();
			}
			else
			{
				OpenNotification(notification);
			}

			// reset our badge
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
		}

		public override void WillTerminate(UIApplication application)
		{
			Preferences.Set("closeDate", DateTime.UtcNow);
		} */
    }
}
