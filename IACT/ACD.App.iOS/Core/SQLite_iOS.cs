using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;

using SQL = SQLite.Net;
using SQLite.Net.Interop;

[assembly: Dependency(typeof(ACD.App.iOS.SQLite_iOS))]

namespace ACD.App.iOS
{
    public class SQLite_iOS : ISQLite
    {
        public SQL.SQLiteConnection GetConnection(string db)
        {
            var sqliteFilename = "ACD_" + db + ".db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "..", "Library"); // Library folder
            var path = Path.Combine(libraryPath, sqliteFilename);
            // Create the connection
            var plat = new SQL.Platform.XamarinIOS.SQLitePlatformIOS();
            var conn = new SQL.SQLiteConnection(plat, path, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, false);
            // Return the database connection
            //conn.StoreDateTimeAsTicks = false;
            return conn;
        }
    }
}