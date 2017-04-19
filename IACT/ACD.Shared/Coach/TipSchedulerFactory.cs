using System;
using System.Collections.Generic;
using System.Text;

using SQLite.Net;

namespace ACD.App
{
    public interface TipSchedulerFactory
    {
        TipScheduler Create(SQLiteConnection db);
    }
}
