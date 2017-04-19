using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using SQLite.Net.Attributes;

namespace ACD
{
    public class DiaryEntry : INotifyPropertyChanged, IComparable<DiaryEntry>
    {
        private DateTime _Day;
        [PrimaryKey]
        public DateTime Day
        {
            protected set { SetField(ref _Day, value.Date); }
            get { return _Day; }
        }

        private string _Text;
        [DefaultValue("")]
        public string Text
        {
            set { SetField(ref _Text, value?.Trim() ?? ""); }
            get { return _Text; }
        }

        public DiaryEntry()
        {
        }

        public DiaryEntry(DateTime day)
        {
            Day = day;
        }

        public int CompareTo(DiaryEntry de)
        {
            return -Day.CompareTo(de.Day);
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
}

