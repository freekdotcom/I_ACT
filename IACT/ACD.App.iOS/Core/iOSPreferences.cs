using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms;

using SQLite.Net;
using SQLite.Net.Attributes;

[assembly: Dependency(typeof(ACD.App.iOS.iOSPreferences))]

namespace ACD.App.iOS
{
    public class iOSPreferences : IPreferences
    {
        public void Set<T>(string key, T value)
        {
            var ud = NSUserDefaults.StandardUserDefaults;
            ud.SetString(Serialization.Serialize(value), key);
			ud.Synchronize();
		}

        public T Get<T>(string key)
        {
            var ud = NSUserDefaults.StandardUserDefaults;
            var str = ud.StringForKey(key);
            if (str == null)
                throw new KeyNotFoundException();
            return Serialization.Deserialize<T>(str);
        }
    }
}