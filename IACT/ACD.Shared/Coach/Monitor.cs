using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ACD
{
    public abstract class Monitor
    {
        protected readonly Profile Profile;

        private string key;
        protected SortedSet<TimeSpan> Times { get; private set; }
        private Dictionary<TimeSpan, int> Notifications { get; set; }
        private DateTime checkFrom;

        private string PreferencesKey(string property)
        {
            return Profile.ID + "_monitor_" + key + "_" + property;
        }

        public Monitor(Profile profile, string key)
        {
            Profile = profile;

            this.key = key;
            Times = Preferences.GetOr(PreferencesKey("times"), new SortedSet<TimeSpan>());
            Notifications = Preferences.GetOr(PreferencesKey("notifications"), new Dictionary<TimeSpan, int>());
            checkFrom = Preferences.GetOr(PreferencesKey("checkFrom"), DateTime.MinValue);
        }

        protected void Save()
        {
            // Cancel unused notifications
            foreach (var key in Notifications.Keys.ToList())
            {
                if (!Times.Contains(key))
                {
                    int nID = Notifications[key];
                    NotificationCenter.Cancel(nID);
                    Notifications.Remove(key);
                }
            }
            // Create new notifications
            foreach (var time in Times)
            {
                if (!Notifications.ContainsKey(time))
                {
                    var nt = MakeNotification(time);
                    NotificationCenter.Schedule(nt);
                    Notifications[time] = nt.ID;
                }
            }

            Preferences.Set(PreferencesKey("times"), Times);
            Preferences.Set(PreferencesKey("checkFrom"), checkFrom);
            Preferences.Set(PreferencesKey("notifications"), Notifications);

            Profile.Debug(new {
                Action = "Saved monitor",
                Key = this.key,
                Times = Times,
                checkFrom = checkFrom,
                Notifications = Notifications
            });
        }

        protected abstract Task<double> GetValueFromUser();
        protected abstract Notification MakeNotification(TimeSpan time);

        DateTime lastCheck = DateTime.MinValue;
        public bool Check()
        {
            var perform = false;

            var now = DateTime.Now;
            for (int i = 0; i < Times.Count; i++)
            {
                var time = Times.ElementAt(i);
                var lastOccurence = DateTime.Today.AddDays(time > now.TimeOfDay ? -1 : 0) + time;

                var timeAfter = Times.ElementAt((i + 1) % Times.Count);
                var firstOccurenceAfter = lastOccurence.Date.AddDays(time >= timeAfter ? 1 : 0) + timeAfter;

                var halfwayTime = lastOccurence + (firstOccurenceAfter - lastOccurence).Divide(2);

                /* If the time to monitor has passed, trigger the event - unless
                 * we are already halfway to the next scheduled monitoring time. */
                if (checkFrom < lastOccurence && now < halfwayTime)
                {
                    perform = true;
                    break;
                }
            }

            if (DateTime.Now - lastCheck >= TimeSpan.FromMinutes(10))
            {
                Profile.Debug(new {
                    Action = "Checking monitor",
                    Key = this.key,
                    Times = Times,
                    checkFrom = checkFrom,
                    now = now,
                    Result = perform
                });

                lastCheck = DateTime.Now;
            }

            return perform;
        }

        public async Task Perform()
        {
            Profile.Debug(new {
                Action = "Performing monitor",
                Key = this.key
            });
            Profile.Add(new MonitorEvent(key, await GetValueFromUser()));
            checkFrom = DateTime.Now;
            Save();
        }

        public ReadOnlyObservableCollection<MonitorEvent> GetEvents()
        {
            var type = TypeIdentifier();
            return Profile.Where<MonitorEvent>(e => e.Type == type);
        }

        public string TypeIdentifier()
        {
            return MonitorEvent.TypeForKey(key);
        }
    }
}

