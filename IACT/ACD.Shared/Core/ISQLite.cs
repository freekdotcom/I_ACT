using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

using SQLite.Net;

namespace ACD
{
	internal static class SQLite
	{
        static SQLiteConnection conn = null;

		public static SQLiteConnection GetConnection(string db)
		{
            conn = conn ?? DependencyService.Get<ISQLite>().GetConnection(db);
            if (conn.StoreDateTimeAsTicks)
                throw new Exception("Tijden zullen niet goed opgeslagen worden! (StoreDateTimeAsTicks houdt geen rekening met tijdzones)");
            return conn;
		}
	}

    public interface ISQLite
    {
        SQLiteConnection GetConnection(string db);
    }
}
