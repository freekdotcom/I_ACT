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

[assembly: Dependency(typeof(ACD.App.Droid.AndroidTipSchedulerFactory))]

namespace ACD.App.Droid
{
    /* [BroadcastReceiver]
    public class TipBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.PutExtra(MainActivity.StartTip, true);

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
            stackBuilder.AddNextIntent(resultIntent);

            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            var nb = new NotificationCompat.Builder(context)
                .SetContentTitle("Tips tegen Dips")
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetContentText("Er staat een nieuwe tip voor je klaar!")
                .SetContentIntent(resultPendingIntent)
                .SetAutoCancel(true);
            var nm = (NotificationManager)context.GetSystemService(Context.NotificationService);
            nm.Notify(14, nb.Build());
        }
    }

    public class AndroidTipScheduler : TipScheduler
    {
        public AndroidTipScheduler(SQLiteConnection db)
            : base(db)
        {
        }

        Intent CreateIntent(ScheduledTip st)
        {
            var intent = new Intent(Forms.Context, typeof(TipBroadcastReceiver));
            intent.SetData(Android.Net.Uri.Parse(string.Format("content://{3}/{0}{1}.{2}",
                st.Hour.ToString("00"), st.Minute.ToString("00"), (int)st.RepeatDays, st.ID)));
            return intent;
        }

        public override void Schedule(ScheduledTip toSchedule)
        {
            base.Schedule(toSchedule);

            try
            {
                var alarm = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
                var intent = PendingIntent.GetBroadcast(Forms.Context, 0, CreateIntent(toSchedule), 0);
                var time = DateTime.Parse(toSchedule.Hour + ":" + toSchedule.Minute);
                alarm.Set(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(time - DateTime.Now).TotalMilliseconds, intent);

                foreach (var day in toSchedule.RepeatDays.GetEnumFlags())
                {
                    var repeatTime = time;
                    while (repeatTime.DayOfWeek.ToString() != day.ToString())
                        repeatTime = repeatTime.AddDays(1);
                    alarm.SetRepeating(AlarmType.RtcWakeup, Java.Lang.JavaSystem.CurrentTimeMillis() + (long)(repeatTime - DateTime.Now).TotalMilliseconds, (long)TimeSpan.FromDays(7).TotalMilliseconds, intent);
                }
            }
            catch (Exception e)
            {
                base.Cancel(toSchedule);
                throw e;
            }
        }

        public override void Cancel(ScheduledTip scheduled)
        {
            var alarm = (AlarmManager)Forms.Context.GetSystemService(Context.AlarmService);
            var intent = PendingIntent.GetBroadcast(Forms.Context, 0, CreateIntent(scheduled), 0);
            alarm.Cancel(intent);

            base.Cancel(scheduled);
        }
    } */

    public class AndroidTipScheduler : TipScheduler
    {
        public AndroidTipScheduler(SQLiteConnection db)
            : base(db)
        {
        }

        Notification BuildNotification(ScheduledTip st)
        {
            var time = DateTime.Parse(st.Hour + ":" + st.Minute);
            if (time <= DateTime.Now)
                time = time.AddDays(1);

            return new Notification {
                Title = "Tips tegen Dips",
                Body = "Er staat een nieuwe tip voor je klaar!",
                Open = "1",
                Data = TipPage.NewTip,
                Time = time,
                Repeat = TimeSpan.FromDays(1)
            };
        }

        public override void Schedule(ScheduledTip toSchedule)
        {
            var notification = BuildNotification(toSchedule);
            NotificationCenter.Schedule(notification);
            toSchedule.NotificationID = notification.ID;
            base.Schedule(toSchedule);
        }

        public override void Cancel(ScheduledTip scheduled)
        {
            NotificationCenter.Cancel(scheduled.NotificationID);
            base.Cancel(scheduled);
        }
    }

    public class AndroidTipSchedulerFactory : TipSchedulerFactory
    {
        public TipScheduler Create(SQLiteConnection db)
        {
            return new AndroidTipScheduler(db);
        }
    }
}