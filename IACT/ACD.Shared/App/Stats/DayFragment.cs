using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ACD
{
    public class DayFragment : INotifyPropertyChanged
    {
        private DateTime _Day;
        public DateTime Day
        {
            private set { SetField(ref _Day, value); }
            get { return _Day; }
        }

        private NotifyCollection<MonitorEvent> _Events;
        public IReadOnlyNotifyCollection<MonitorEvent> Events
        {
            private set { SetField(ref _Events, value as NotifyCollection<MonitorEvent>); }
            get { return _Events; }
        }

        private DayFragmentCollection _Overview;
        public DayFragmentCollection Overview
        {
            private set { SetField(ref _Overview, value); }
            get { return _Overview; }
        }

        public DayFragment(DateTime day, IEnumerable<MonitorEvent> events, DayFragmentCollection overview)
        {
            Day = day.Date;
            Events = new NotifyCollection<MonitorEvent>(events);
            Overview = overview;
        }

        public DayFragment(DateTime day, DayFragmentCollection overview, params MonitorEvent[] events)
            : this(day, events, overview)
        {
        }

        public void AddEvent(MonitorEvent ev)
        {
            if (Day <= ev.Time && ev.Time < Day.AddDays(1))
            {
                _Events.AddSorted(ev);
                OnPropertyChanged("Events");
            }
        }

        public DayFragment Yesterday
        {
            get
            {
                DayFragment fragment = null;
                Overview.TryGetValue(Day.AddDays(-1), out fragment);
                return fragment;
            }
        }

        public DayFragment Tomorrow
        {
            get
            {
                DayFragment fragment = null;
                Overview.TryGetValue(Day.AddDays(1), out fragment);
                return fragment;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion
    }

    class NewestFirstComparer : IComparer<DateTime>, IComparer<DayFragment>
    {
        public int Compare(DateTime dt1, DateTime dt2)
        {
            return -dt1.CompareTo(dt2);
        }

        public int Compare(DayFragment df1, DayFragment df2)
        {
            if (df1 == df2)
                return 0;
            if (df1 == null)
                return 1;
            if (df2 == null)
                return -1;
            return Compare(df1.Day, df2.Day);
        }
    }

    public class DayFragmentCollection : NotifyCollection<DayFragment>
    {
        private Dictionary<DateTime, DayFragment> dict = new Dictionary<DateTime, DayFragment>();

        public DayFragmentCollection(IList<MonitorEvent> events)
        {
            var firstDay = DateTime.Today.AddDays(-6);
            var lastDay = DateTime.Today;
            if (events.Any())
            {
                firstDay = Extensions.Min(events[0].Time.Date, firstDay);
                lastDay = Extensions.Max(events[events.Count - 1].Time.Date, lastDay);
            }
            var eventsByDay = events.ToLookup(ev => ev.Time.Date);
            for (var day = lastDay; day >= firstDay; day -= TimeSpan.FromDays(1))
            {
                var fragment = new DayFragment(day, eventsByDay[day], this);
                this.Add(fragment);
                dict.Add(day, fragment);
            }

            var observableEvents = events as INotifyCollectionChanged;
            if (observableEvents != null)
            {
                observableEvents.CollectionChanged += (sender, e) => {
                    if (e.Action != NotifyCollectionChangedAction.Add)
                        throw new NotImplementedException();

                    foreach (var obj in e.NewItems)
                    {
                        var ev = obj as MonitorEvent;
                        DayFragment fragment;
                        if (dict.TryGetValue(ev.Time.Date, out fragment))
                        {
                            fragment.AddEvent(ev);
                        }
                        else
                        {
                            var newFragment = new DayFragment(day: ev.Time.Date, events: ev, overview: this);
                            dict.Add(newFragment.Day, newFragment);
                            this.AddSorted(fragment, new NewestFirstComparer());
                        }
                    }
                };
            }
        }

        #region IDictionary implementation
        public bool ContainsKey(DateTime key)
        {
            return dict.ContainsKey(key);
        }
        public bool TryGetValue(DateTime key, out DayFragment value)
        {
            return dict.TryGetValue(key, out value);
        }
        DayFragment this[DateTime index]
        {
            get
            {
                return dict[index];
            }
        }
        public ICollection<DateTime> Keys
        {
            get
            {
                return dict.Keys;
            }
        }
        #endregion
    }
}

