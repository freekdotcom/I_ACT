using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SQLite.Net.Attributes;
using Polenter.Serialization;

namespace ACD
{
    /*
     * Base class representing an event occuring that might influence the user's state (i.e. responses to surveys).
     */
    public class Event : IComparable, IComparable<Event>
    {
        [ExcludeFromSerialization]
        public DateTime Time { get; set; }
        [ExcludeFromSerialization]
        public string Type { get; set; }

        public Event()
        {
        }

        public Event(DateTime time, string type)
        {
            Time = time;
            Type = type;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            var otherEvent = obj as Event;
            if (otherEvent != null)
                return CompareTo(otherEvent);
            else
                throw new ArgumentException("Object to compare to is not an Event.");
        }

        public int CompareTo(Event ev)
        {
            return ev == null ? 1 : Time.CompareTo(ev.Time);
        }
    }
}
