using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Foundation;
using UIKit;
using ObjCRuntime;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Dependency(typeof(ACD.iOSNotificationScheduler))]

namespace ACD
{
    public class iOSNotificationScheduler : BaseNotificationScheduler
    {
        UILocalNotification BuildNotification(Notification n)
        {
            var notification = new UILocalNotification();
            notification.FireDate = (NSDate)n.Time;
            notification.RepeatInterval = NSCalendarUnit.Day;
            if (notification.RespondsToSelector(new Selector("setAlertTitle:")))
                notification.AlertTitle = n.Title;
            notification.AlertBody = n.Body;
            notification.AlertAction = n.Action;
            notification.ApplicationIconBadgeNumber = 1;
			var data = new NSMutableDictionary();
			data.Add(NSObject.FromObject("open"), NSObject.FromObject(n.Open ?? ""));
			data.Add(NSObject.FromObject("data"), NSObject.FromObject(n.Data ?? ""));
			data.Add(NSObject.FromObject("action"), NSObject.FromObject(n.Action ?? ""));
            data.Add(NSObject.FromObject("id"), NSObject.FromObject(n.ID.ToString()));
			notification.UserInfo = data;
            return notification;
        }

        public override void Schedule(Notification toSchedule)
        {
            base.Schedule(toSchedule);
            UIApplication.SharedApplication.ScheduleLocalNotification(BuildNotification(toSchedule));
        }

        public override void Cancel(int id)
        {
            var app = UIApplication.SharedApplication;
            foreach (var n in app.ScheduledLocalNotifications)
            {
                var nID = 0;
                if (int.TryParse((NSString)n.UserInfo["id"], out nID) && nID == id)
                {
                    app.CancelLocalNotification(n);
                    break;
                }
            }
            base.Cancel(id);
        }

        public static void Recieve(UILocalNotification n)
        {
            var nID = 0;
            if (int.TryParse((NSString)n.UserInfo["id"], out nID))
            {
                NotificationCenter.Recieve(nID);
            }
        }

        public static bool NeedPermission()
        {
            var app = UIApplication.SharedApplication;
            if (!app.RespondsToSelector(new Selector("registerUserNotificationSettings:")))
            {
                return false;
            }

            /*
            if ([[UIApplication sharedApplication] respondsToSelector:@selector(currentUserNotificationSettings)]){ // Check it's iOS 8 and above
                UIUserNotificationSettings *grantedSettings = [[UIApplication sharedApplication] currentUserNotificationSettings];

                if (grantedSettings.types == UIUserNotificationTypeNone) {
                    NSLog(@"No permiossion granted");
                }
                else if (grantedSettings.types & UIUserNotificationTypeSound & UIUserNotificationTypeAlert ){
                    NSLog(@"Sound and alert permissions ");
                }
                else if (grantedSettings.types  & UIUserNotificationTypeAlert){
                    NSLog(@"Alert Permission Granted");
                }
            }
            */

            else
            {
                var granted = app.CurrentUserNotificationSettings;
                return granted.Types == UIUserNotificationType.None;
            }
        }

        public static readonly string permissionKey = "ios-notify-permission";
        public static async Task AskPermission()
        {
            if (NeedPermission())
            {
                bool alreadyAsked = Preferences.GetOr(permissionKey, false);

                if (alreadyAsked == false)
                {
                    await Alert.Show(
                        "Notificaties toestaan",
                        "Ga je er mee akkoord dat deze app je af en toe een melding geeft, onder andere wanneer er een nieuwe tip beschikbaar is?\n\nWat voor meldingen je precies wil ontvangen kan je instellen in de Instellingen-app van je telefoon.",
                        null,
                        new AlertButton {
                            IsPreferred = true,
                            Text = "OK",
                            Action = () => {
                                if (!alreadyAsked)
                                {
                                    UIApplication.SharedApplication.RegisterUserNotificationSettings(
                                        UIUserNotificationSettings.GetSettingsForTypes(
                                            UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                                        )
                                    );
                                    Preferences.Set(permissionKey, (alreadyAsked = true));
                                    return true;
                                }
                                return false;
                            }
                        }
                    );
                }

                if (NeedPermission())
                {
                    //var error = new Label {
                    //    Text = "Volg de instructies hierboven om meldingen in te schakelen.",
                    //    HorizontalTextAlignment = TextAlignment.Center,
                    //    Style = StyleKit.AutoDarkLabelStyles.Caption,
                    //    FontSize = 12,
                    //    TextColor = Color.Red,
                    //    IsVisible = false
                    //};

                    await Alert.Show(
                        "Notificaties toestaan",
                        "Om de app u goed van dienst te laten zijn, moeten meldingen ingeschakeld zijn.\n\nGa hiervoor naar de Instellingen-app van je telefoon en zoek naar het logo van I-ACT. Daar kan je de meldingen inschakelen.",
                        null, //new StackLayout { Children = { error } },
                        new AlertButton {
                            IsPreferred = true,
                            Text = "Oké",
                            Action = () => {
                                //if (NeedPermission())
                                //{
                                //    error.IsVisible = true;
                                //    error.IsVisible = false;
                                //    error.IsVisible = true;
                                //    return true;
                                //}
                                return false;
                            }
                        }
                    );
                }
            }
        }
    }
}