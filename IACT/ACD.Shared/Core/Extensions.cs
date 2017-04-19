using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Forms;

namespace ACD
{
    public static partial class Extensions
    {
        public static P GetStatic<P>(this Type t, string name)
            where P : class
        {
            return t?.GetField(name)?.GetValue(null) as P;
        }

        public static void Shuffle<T>(this IList<T> list, Random rnd)
        {
            for (var i = 0; i < list.Count; i++)
                list.Swap(i, rnd.Next(i, list.Count));
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /* From http://praeclarum.org/post/45231096776/await-in-the-land-of-ios-drag-n-drop */
        public static Task<T> GetEventAsync<T>(this object eventSource, string eventName)
            where T : EventArgs
        {
            var tcs = new TaskCompletionSource<T>();

            var type = eventSource.GetType();
            var ev = type.GetRuntimeEvent(eventName);

            EventHandler handler = null;

            handler = delegate(object sender, EventArgs e) {
                ev.RemoveEventHandler(eventSource, handler);
                tcs.SetResult((T)e);
            };

            ev.AddEventHandler(eventSource, handler);
            return tcs.Task;
        }

        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }

            return dt.AddDays(-1 * diff).Date;
        }

        public static IEnumerable<T> GetEnumFlags<T>(this T e)
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Where(t => (Convert.ToUInt64(e) & Convert.ToUInt64(t)) == Convert.ToUInt64(t));
        }

        public static void Replace<T>(this IList<T> l, T elem)
        {
            l.Clear();
            l.Add(elem);
        }

        public static void Replace<T>(this IList<T> l, params T[] elems)
        {
            l.Replace(elems);
        }

        public static void Replace<T>(this IList<T> l, IEnumerable<T> elems)
        {
            l.Clear();
            l.Add(elems);
        }

        public static void Add<T>(this ICollection<T> l, params T[] elems)
        {
            if (elems != null)
            {
                foreach (T elem in elems)
                    l.Add(elem);
            }
        }

        public static void Add<T>(this ICollection<T> l, IEnumerable<T> elems)
        {
            if (elems != null)
            {
                foreach (T elem in elems)
                    l.Add(elem);
            }
        }

        public static INotifyCollectionChanged AsObservable<T>(this ReadOnlyObservableCollection<T> collection)
        {
            return (INotifyCollectionChanged)collection;
        }

        public static TimeSpan MinutesOnly(this TimeSpan time)
        {
            return TimeSpan.FromMinutes(Math.Floor(time.TotalMinutes));
        }

        public static TimeSpan Divide(this TimeSpan time, double factor)
        {
            return TimeSpan.FromTicks((long)Math.Round(time.Ticks / factor));
        }

        public static Thickness With(this Thickness t, double? left = null, double? right = null,
            double? top = null, double? bottom = null) {
            return new Thickness(
                left ?? t.Left,
                top ?? t.Top,
                right ?? t.Right,
                bottom ?? t.Bottom
            );
        }

        public static Thickness Negative(this Thickness t) {
            return new Thickness(
                -t.Left,
                -t.Top,
                -t.Right,
                -t.Bottom
            );
        }

        public static void SetBinding<TModel, TProp, TDest>(this BindableObject self, BindableProperty targetProperty, Expression<Func<TModel, object>> sourceProperty, Func<TProp, TDest> converter)
        {
            self.SetBinding<TModel>(targetProperty, sourceProperty, BindingMode.OneWay, new FuncValueConverter<TProp, TDest>(converter));
        }

        public static List<T> AsList<T>(this IList<T> @this)
        {
            return @this as List<T> ?? @this.ToList();
        }

        public static int FindSortedIndex<T>(this IList<T> @this, T item) where T: IComparable<T>
        {
            if (@this.Count == 0)
            {
                return 0;
            }
            if (@this[@this.Count - 1].CompareTo(item) <= 0)
            {
                return @this.Count;
            }
            if (@this[0].CompareTo(item) >= 0)
            {
                return 0;
            }

            int index = @this.AsList().BinarySearch(item);
            if (index < 0) 
                index = ~index;
            return index;
        }

        public static int AddSorted<T>(this IList<T> @this, T item) where T: IComparable<T>
        {
            int index = @this.FindSortedIndex(item);
            @this.Insert(index, item);
            return index;
        }

        public static int AddSorted<T>(this IList<T> @this, T item, IComparer<T> comparer)
        {
            if (@this.Count == 0)
            {
                @this.Add(item);
                return 0;
            }
            if (comparer.Compare(@this[@this.Count - 1], item) <= 0)
            {
                @this.Add(item);
                return @this.Count - 1;
            }
            if (comparer.Compare(@this[0], item) >= 0)
            {
                @this.Insert(0, item);
                return 0;
            }

            int index = @this.AsList().BinarySearch(item, comparer);
            if (index < 0) 
                index = ~index;
            @this.Insert(index, item);
            return index;
        }

        public static DateTime Min(DateTime dt1, DateTime dt2)
        {
            return dt1 < dt2 ? dt1 : dt2;
        }

        public static DateTime Max(DateTime dt1, DateTime dt2)
        {
            return dt1 > dt2 ? dt1 : dt2;
        }
    }

    class FuncValueConverter<TSource, TDest> : IValueConverter
    {
        Func<TSource, TDest> func;

        public FuncValueConverter(Func<TSource, TDest> func)
        {
            this.func = func;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(TDest))
                throw new Exception("Unsuitable target type.");
            if (value is TSource)
                return func((TSource)value);
            if (value == null && typeof(TSource).IsClass)
                return func(default(TSource));
            throw new Exception("Unsuitable source value type.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public interface INotifyCollection<T> 
        : ICollection<T>, 
          INotifyCollectionChanged
    {}

    public interface IReadOnlyNotifyCollection<out T> 
        : IReadOnlyCollection<T>,
          IReadOnlyList<T>,
          INotifyCollectionChanged
    {}

    public class NotifyCollection<T> 
        : ObservableCollection<T>, 
          INotifyCollection<T>, 
          IReadOnlyNotifyCollection<T>
    {
        public NotifyCollection()
            : base()
        {
        }

        public NotifyCollection(IEnumerable<T> items)
        {
            this.Add(items);
        }
    }

    public class NotifySortedCollection<T>
        : NotifyCollection<T> where T : IComparable<T>
    {
        public NotifySortedCollection()
        {
        }

        public NotifySortedCollection(IEnumerable<T> items)
        {
            this.Add(items);
        }

        protected override void InsertItem(int index, T item)
        {
            index = Items.FindSortedIndex(item);
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            return;
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            return;
        }
    }

    public class NotifySet<T>
        : NotifyCollection<T>
    {
        public NotifySet()
        {
        }

        public NotifySet(IEnumerable<T> items)
        {
            this.Add(items);
        }

        protected override void InsertItem(int index, T item)
        {
            if (!this.Contains(item))
                base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            if (!this.Contains(item))
                base.SetItem(index, item);
        }
    }

    public class NotifySortedSet<T>
        : NotifyCollection<T> where T : IComparable<T>
    {
        public NotifySortedSet()
        {
        }

        public NotifySortedSet(IEnumerable<T> items)
        {
            this.Add(items);
        }

        protected override void InsertItem(int index, T item)
        {
            if (!this.Contains(item))
            {
                index = Items.FindSortedIndex(item);
                base.InsertItem(index, item);
            }
        }

        protected override void SetItem(int index, T item)
        {
            return;
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            return;
        }
    }
}

