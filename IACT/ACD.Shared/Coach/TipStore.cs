using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite.Net;

namespace ACD
{
    public class TipStore : ObservableCollection<Tip>
    {
        readonly SQLiteConnection database;

        public TipStore(SQLiteConnection database)
        {
            this.database = database;
            database.CreateTable<Tip>();

            this.Add(database.Table<Tip>().ToList());
            foreach(Tip item in this)
            {
                item.PropertyChanged += TipPropertyChanged;
            }

            CollectionChanged += HandleChange;
        }

        private void HandleChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                database.RunInTransaction(() =>
                {
                    foreach (Tip t in e.OldItems)
                        database.Delete(t);
                    database.InsertAll(e.NewItems);
                });
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                database.InsertAll(e.NewItems);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                database.RunInTransaction(() =>
                {
                    foreach (Tip t in e.OldItems)
                        database.Delete(t);
                });
            }

            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach(Tip item in e.OldItems)
                {
                    item.PropertyChanged -= TipPropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(Tip item in e.NewItems)
                {
                    item.PropertyChanged += TipPropertyChanged;
                }     
            }  
        }

        public class EnabledChangeEventArgs : EventArgs
        {
            public Tip Tip { get; set; }
        }

        public EventHandler<EnabledChangeEventArgs> EnabledChange;

        public void TipPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled" && EnabledChange != null)
            {
                var tip = (Tip)sender;
                EnabledChange(this, new EnabledChangeEventArgs { Tip = tip });
            }
            database.Update(sender);
        }

        public Tip Get(int id)
        {
            return this.Where(tip => tip.ID == id).First();
        }

        public IReadOnlyList<Tip> GetAll()
        {
            return this.ToList();
        }

        public IReadOnlyList<Tip> GetEnabledOrAll()
        {
            var enabled = this.Where(tip => tip.Enabled).ToList();
            return enabled.Count > 0 ? enabled : GetAll();
        }

        /* public void Add(Tip tip)
        {
            _database.Insert(tip);
        }

        public void Add(IEnumerable<Tip> tips)
        {
            _database.InsertAll(tips);
        }

        public void Remove(Tip tip)
        {
            _database.Delete<Tip>(tip);
        } */

        public void Update(Tip tip)
        {
            database.Update(tip);
        }

        /* public IEnumerator<Tip> GetEnumerator()
        {
            //return new TipEnumerator(_database.Table<Tip>().GetEnumerator(), this);
            return _database.Table<Tip>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        } */

        /* private void TipUpdated(object sender, PropertyChangedEventArgs args)
        {
            Update((Tip)sender);
        }

        private class TipEnumerator : IEnumerator<Tip>
        {
            private IEnumerator<Tip> source;
            private TipStore store;

            public TipEnumerator(IEnumerator<Tip> source, TipStore store)
            {
                this.source = source;
                this.store = store;
            }

            public Tip Current
            {
                get
                {
                    var tip = source.Current;
                    tip.PropertyChanged += store.TipUpdated;
                    return tip;
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                return source.MoveNext();
            }

            public void Reset()
            {
                source.Reset();
            }

            public void Dispose()
            {
                source.Dispose();
            }
        } */
    }
}
