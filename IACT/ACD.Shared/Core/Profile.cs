using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ACD.App;
using SQLite.Net;
using SQLite.Net.Attributes;
using Xamarin.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ACD
{
    /*
     * A user profile containing responses to surveys (signifing the user's state at a certain point in time) and
     * a number of events that occured while using the application that might have influenced the outcome of these surveys.
     */
    public class Profile
    {
        public int ID { get; private set; }

		[JsonIgnore]
		SQLiteConnection database;

        public Profile(int id)
        {
            ID = id;

            database = SQLite.GetConnection(id.ToString());
            database.CreateTable<EventBlob>();
            database.CreateTable<DiaryEntry>();
        }

		public class EventBlob
		{
			[PrimaryKey, AutoIncrement]
			public int ID { get; set; }
			public string Type { get; set; }
            public DateTime Time { get; set; }
            public byte[] Data { get; set; }

            public EventBlob() {}
            public EventBlob(Event ev)
            {
                Type = ev.Type ?? ev.GetType().Name;
                Time = ev.Time;
                Data = Serialization.SerializeInternal(ev);
            }

            public Event ToEvent() {
                var ev = Serialization.Deserialize<Event>(Data);
                ev.Time = Time;
                ev.Type = Type;
                return ev;
            }
		}

        public void Add(Event ev)
        {
            var blob = new EventBlob(ev);
            database.Insert(blob);
            if (NewEvent != null)
            {
                NewEvent(this, new NewEventEventArgs {
                    Event = ev,
                    Blob = blob
                });
            }
        }

        public class NewEventEventArgs : EventArgs
        {
            public EventBlob Blob { get; set; }
            public Event Event { get; set; }
        }

        public EventHandler<NewEventEventArgs> NewEvent;

        public TableQuery<EventBlob> QueryEvents()
        {
            return database.Table<EventBlob>();
        }

        public ReadOnlyObservableCollection<T> Where<T>(Func<EventBlob, bool> predicate) where T : Event
        {
            var collection = new ObservableCollection<T>(this.QueryEvents()
                .Where(predicate)
                .Select(eb => eb.ToEvent() as T)
            );
            NewEvent += (sender, e) => {
                if (predicate(e.Blob))
                    collection.Add(e.Event as T);
            };
            return new ReadOnlyObservableCollection<T>(collection);
        }

        public string Serialize(Coach coach)
        {
            var events = database.Table<EventBlob>().Select(eb => eb.ToEvent()).ToList();

            return Serialization.Serialize(new {
                ID = ID,
                Log = events,
                Tips = coach.Tips
            }, true);
        }

        public void Debug(object data)
        {
            Add(new LogEvent(data));
        }

        NotifySortedSet<DiaryEntry> diary = null;

        void UpdateEntry(object sender, PropertyChangedEventArgs e)
        {
            database.Update(sender as DiaryEntry, typeof(DiaryEntry));
        }

        public NotifySortedSet<DiaryEntry> Diary
        {
            get
            {
                if (diary == null)
                {
                    diary = new NotifySortedSet<DiaryEntry>(database.Table<DiaryEntry>());
                    foreach (var entry in diary)
                    {
                        entry.PropertyChanged += UpdateEntry;
                    }
                    diary.CollectionChanged += (sender, e) => {
                        switch (e.Action)
                        {
                            case NotifyCollectionChangedAction.Add:
                                database.InsertAll(e.NewItems, typeof(DiaryEntry));
                                foreach (var entry in e.NewItems.Cast<DiaryEntry>())
                                {
                                    entry.PropertyChanged += UpdateEntry;
                                }
                                break;
                            case NotifyCollectionChangedAction.Remove:
                                foreach (var entry in e.OldItems.Cast<DiaryEntry>())
                                {
                                    entry.PropertyChanged -= UpdateEntry;
                                    database.Delete(entry);
                                }   
                                break;
                        }
                    };
                }
                return diary;
            }
        }

        public static async Task<Profile> New()
        {
			var profile = new Profile(new Random().Next(99999));
			return profile;
        }

        public static async Task<Profile> Get(int id)
        {
            return new Profile(id);
        }
    }
}
