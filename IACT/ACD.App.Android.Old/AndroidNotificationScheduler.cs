using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Support.V4.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite.Net;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(ACD.App.Droid.AndroidNotificationScheduler))]

namespace ACD.App.Droid
{
    [BroadcastReceiver]
    public class NotificationBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var n = Serialization.Deserialize<Notification>(intent.GetStringExtra("object"));

            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.PutExtra("open", n.Open);
            resultIntent.PutExtra("data", n.Data);

			if (!string.IsNullOrWhiteSpace(n.Data))
			{
				var sp = context.GetSharedPreferences("ACD", FileCreationMode.Private);
				sp.Edit().PutString("launchData", Serialization.Serialize(n.Data)).Commit();
			}

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            var nb = new NotificationCompat.Builder(context)
                .SetContentTitle(n.Title)
                //.SetSmallIcon(Resource.Drawable.Icon)
                .SetContentText(n.Body)
                .SetContentIntent(resultPendingIntent)
                .SetAutoCancel(true);
            var nm = (NotificationManager)context.GetSystemService(Context.NotificationService);
            nm.Notify(n.ID, nb.Build());
        }
    }

    public class AndroidNotificationScheduler : BaseNotificationScheduler, INotificationScheduler
    {
        public AndroidNotificationScheduler()
        {
            foreach (Notification n in Database.Table<Notification>())
            {
                CancelAlarm(n);
                SetAlarm(n);
            }
        }

        Intent CreateIntent(Notification n)
        {
            var intent = new Intent(Forms.Context, typeof(NotificationBroadcastReceiver));
            intent.SetData(Android.Net.Uri.Parse(string.Format("content://{0}", n.ID)));
            intent.PutExtra("object", Serialization.Serialize(n));
            return intent;
        }

        void SetAlarm(Notification n)
        {
            var alarm = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
            var intent = PendingIntent.GetBroadcast(Forms.Context, 0, CreateIntent(n), 0);
            var time = n.Time;
            if (time < DateTime.Now)
                time = DateTime.Now.Date.AddDays(1).Add(time.TimeOfDay);
            if (n.Repeat > TimeSpan.Zero)
            {
                alarm.SetRepeating(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(n.Time - DateTime.Now).TotalMilliseconds, (long)n.Repeat.TotalMilliseconds, intent);
            }
            else
            {
                alarm.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(n.Time - DateTime.Now).TotalMilliseconds, intent);
            }
        }

        void CancelAlarm(Notification n)
        {
            var alarm = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
            var intent = PendingIntent.GetBroadcast(Forms.Context, 0, CreateIntent(n), 0);
            alarm.Cancel(intent);
        }

        public override void Schedule(Notification n)
        {
            base.Schedule(n);

            try
            {
                SetAlarm(n);
            }
            catch (Exception e)
            {
                base.Cancel(n.ID);
                throw e;
            }
        }

        public override void Cancel(int id)
        {
            var n = Database.Table<Notification>().Where(nt => nt.ID == id).FirstOrDefault();

            if (n == null)
                return;

            CancelAlarm(n);

            base.Cancel(id);
        }
    }
}