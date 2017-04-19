using System;

namespace ACD
{
    public class MonitorEvent : Event
    {
        public static string TypeForKey(string key)
        {
            return "monitor:" + key;
        }

        public double Value { get; set; }

        public MonitorEvent()
        {
        }

        public MonitorEvent(string key, double value, DateTime time)
            : base(time, TypeForKey(key))
        {
            Value = value;
        }

        public MonitorEvent(string key, double value)
            : this(key, value, DateTime.Now)
        {
        }
    }
}
