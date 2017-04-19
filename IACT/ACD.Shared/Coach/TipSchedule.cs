using System;
using System.Collections.Generic;

namespace ACD
{
    public class TipSchedule
    {
        public SortedSet<TimeSpan> Times { get; set; }
        public Dictionary<TimeSpan, int> Notifications { get; set; }
        public int[] Tips { get; set; }
        public int CurrentTip { get; set; }
        public DateTime CheckFrom { get; set; }

        public TipSchedule()
        {
            Times = new SortedSet<TimeSpan>();
            Notifications = new Dictionary<TimeSpan, int>();
        }
    }
}

