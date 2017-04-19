using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ACD
{
    /*
     * Event for when a tip is displayed.
     */
    public class DisplayTipEvent : Event
    {
        public static readonly string TypeIdentifier = "tip:display";

        public int Tip { get; set; }

        public DisplayTipEvent()
        {
        }

        public DisplayTipEvent(int tipID, DateTime time)
            : base(time, DisplayTipEvent.TypeIdentifier)
        {
            Tip = tipID;
        }

        public DisplayTipEvent(int tipID)
            : this(tipID, DateTime.Now)
        {
        }

        public DisplayTipEvent(Tip tip, DateTime time)
            : this(tip.ID, time)
        {
        }

        public DisplayTipEvent(Tip tip)
            : this(tip, DateTime.Now)
        {
        }
    }
}
