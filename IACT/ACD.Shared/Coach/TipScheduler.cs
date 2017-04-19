using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;

using SQLite.Net;

using Xamarin.Forms;

namespace ACD
{
    public class TipScheduler : INotifyPropertyChanged
    {
        private static Random rng = new Random();

        private Profile profile;
        private TipStore tips;
        private TipSchedule schedule;
        private ObservableCollection<TimeSpan> times;

        public Tip CurrentTip
        {
            get { return tips.Get(CurrentTipID); }
            set { CurrentTipID = value.ID; }
        }

        public int CurrentTipID
        {
            private set
            {
                if (schedule.CurrentTip == value) return;

                schedule.CurrentTip = value;
                SaveSchedule();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentTipID"));
                    PropertyChanged(this, new PropertyChangedEventArgs("CurrentTip"));
                }
            }
            get
            {
                return schedule.CurrentTip;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public class NewTipEventArgs : EventArgs
        {
            public Tip Tip { get; set; }
        }

        public EventHandler<NewTipEventArgs> NewTip;

        public TipScheduler(Profile profile, TipStore tips, List<TimeSpan> initTimes = null)
        {
            this.profile = profile;
            this.tips = tips;
            this.schedule = Preferences.GetOr(profile.ID + "_schedule", new TipSchedule());
            this.times = new ObservableCollection<TimeSpan>();

            PropertyChanged += (sender, e) => {
                if (e.PropertyName == "CurrentTipID")
                {
                    var tde = new DisplayTipEvent(CurrentTipID);
                    profile.Add(tde);
                    schedule.CheckFrom = tde.Time;
                    SaveSchedule();
                    if (NewTip != null)
                        NewTip(this, new NewTipEventArgs { Tip = tips.Get(CurrentTipID) });
                }
            };

            schedule.Times.Add(initTimes);

            if (schedule.Tips == null || CurrentTipID == 0)
            {
                BuildSchedule();
                SetNextTip();
            }
            /* else
            {
                 lastTipTime = profile.QueryEvents().Where(e => e.Type == typeof(TipDisplayEvent).Name)
                    .OrderByDescending(e => e.Time).First().Time;
            } */

            tips.EnabledChange += (sender, e) =>
            {
                BuildSchedule();
            };

            this.times.Add(schedule.Times);
        }

        public void AddTime(TimeSpan time)
        {
            var added = schedule.Times.Add(time);
            var notification = BuildNotification(time);
            NotificationCenter.Schedule(notification);
            schedule.Notifications[time] = notification.ID;
            var timeToday = DateTime.Today + time;
            /* Fix to prevent extra tips when times between now and last tip are added */
            if (schedule.CheckFrom < timeToday && timeToday < DateTime.Now)
                schedule.CheckFrom = timeToday;
            SaveSchedule();
            if (added) times.Add(time);
        }

        public void RemoveTime(TimeSpan time)
        {
            bool removed = schedule.Times.Remove(time);
            NotificationCenter.Cancel(schedule.Notifications[time]);
            schedule.Notifications.Remove(time);
            if (removed)
            {
                SaveSchedule();
                times.Remove(time);
            }
        }

        public ReadOnlyObservableCollection<TimeSpan> GetTimes()
        {
            return new ReadOnlyObservableCollection<TimeSpan>(times);
        }

        private void SaveSchedule()
        {
            Preferences.Set(profile.ID + "_schedule", schedule);
            profile.Debug(new {
                Action = "Saved schedule",
                Schedule = Preferences.Get<TipSchedule>(profile.ID + "_schedule")
            });
        }

        private void BuildSchedule()
        {
            var tipIDs = tips.GetEnabledOrAll().Select(t => t.ID).ToArray();
            tipIDs.Shuffle(rng);
            if (CurrentTipID != 0 && tipIDs.Length > 1)
            {
                var split = Array.IndexOf(tipIDs, CurrentTipID);
                if (split == -1)
                {
                    schedule.Tips = tipIDs;
                }
                else
                {
                    var orderedTips = new int[tipIDs.Length - 1];
                    Array.Copy(tipIDs, split + 1, orderedTips, 0, tipIDs.Length - (split + 1));
                    Array.Copy(tipIDs, 0, orderedTips, tipIDs.Length - (split + 1), split);
                    schedule.Tips = orderedTips;
                }
            }
            else
            {
                schedule.Tips = tipIDs;
            }
            profile.Debug(new {
                Action = "Rebuilt schedule",
                Tips = schedule.Tips
            });
            SaveSchedule();
        }

        private bool SetNextTip()
        {
            if (schedule.Tips.Length == 0)
            {
                BuildSchedule();
                if (schedule.Tips.Length == 0) return false;
            }
            profile.Debug(new {
                Action = "Switching tips!"
            });
            var lastTip = CurrentTipID;
            CurrentTipID = schedule.Tips[0];
            var leftovers = new int[schedule.Tips.Length - 1];
            if (leftovers.Length == 0)
            {
                BuildSchedule();
            }
            else
            {
                Array.Copy(schedule.Tips, 1, leftovers, 0, leftovers.Length);
                schedule.Tips = leftovers;
                profile.Debug(new {
                    Action = "Shifted tips in schedule",
                    Tips = schedule.Tips
                });
                SaveSchedule();
            }
            return CurrentTipID != lastTip;
        }

        public void ForceNextTip()
        {
            SetNextTip();
        }

        private bool ShouldSwitchTips()
        {
            var now = DateTime.Now;
            foreach (var time in schedule.Times)
            {
                var fullTime = DateTime.Today + time;
                if (time > now.TimeOfDay)
                    fullTime = fullTime.AddDays(-1);
                if (schedule.CheckFrom < fullTime)
                    return true;
            }
            return false;
        }

        public bool CheckSchedule()
        {
            var next = ShouldSwitchTips();
            if (next)
            {
                next = SetNextTip();
            }
            return next;
        }

        Notification BuildNotification(TimeSpan t)
        {
            var time = DateTime.Today + t;
            if (time <= DateTime.Now)
                time = time.AddDays(1);

            return new Notification {
                Title = "Nieuwe tip",
                Body = "Er staat een nieuwe tip voor je klaar!",
                Open = "0",
				Action = "bekijken",
                Time = time,
                Repeat = TimeSpan.FromDays(1)
            };
        }
    }
}
