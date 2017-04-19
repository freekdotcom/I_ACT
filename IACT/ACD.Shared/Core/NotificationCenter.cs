using System;
using System.Collections.Generic;
using System.Text;

using SQLite.Net;
using SQLite.Net.Attributes;

using Xamarin.Forms;

namespace ACD
{
    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public string Open { get; set; }
        public string Data { get; set; }
		public string Action { get; set; }

        public DateTime Time { get; set; }
        public TimeSpan Repeat { get; set; }
    }

    public interface INotificationScheduler
    {
        void Schedule(Notification n);
        void Cancel(int id);
        Notification Get(int id);
    }

    public abstract class BaseNotificationScheduler : INotificationScheduler
    {
        protected SQLiteConnection Database { get; private set; }

        public BaseNotificationScheduler()
        {
            Database = SQLite.GetConnection("Notifications");
            Database.CreateTable<Notification>();
        }

        public virtual void Schedule(Notification n)
        {
            Database.Insert(n);
        }

        public virtual void Cancel(int id)
        {
            Database.Delete<Notification>(id);
        }

        public Notification Get(int id)
        {
            return Database.Get<Notification>(id);
        }
    }

    public static class NotificationCenter
    {
        public static void Schedule(Notification n)
        {
            DependencyService.Get<INotificationScheduler>().Schedule(n);
        }

        public static void Cancel(int id)
        {
            DependencyService.Get<INotificationScheduler>().Cancel(id);
        }

		public class NotificationEventArgs : EventArgs
		{
			public Notification Notification { get; set; }
		}

        public static void Recieve(int id)
        {
            if (NotificationRecieved != null)
            {
                NotificationRecieved(null, new NotificationEventArgs {
                    Notification = DependencyService.Get<INotificationScheduler>().Get(id)
                });
            }
        }

		public static EventHandler<NotificationEventArgs> NotificationRecieved;
    }
}
