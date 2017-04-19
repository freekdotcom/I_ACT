using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Forms;

using SQL = SQLite.Net;

[assembly: Dependency(typeof(ACD.App.Droid.SQLite_Android))]

namespace ACD.App.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQL.SQLiteConnection GetConnection(string db)
        {
            var sqliteFilename = "ACD_" + db + ".db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            var path = Path.Combine(documentsPath, sqliteFilename);
            // Create the connection
            var plat = new SQL.Platform.XamarinAndroid.SQLitePlatformAndroid();
            var conn = new SQL.SQLiteConnection(plat, path, false);
            // Return the database connection 
            //conn.StoreDateTimeAsTicks = false;
            return conn;
        }
    }
}
