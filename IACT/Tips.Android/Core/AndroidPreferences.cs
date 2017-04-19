using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;

[assembly: Dependency(typeof(ACD.App.Droid.AndroidPreferences))]

namespace ACD.App.Droid
{
    public class AndroidPreferences : IPreferences
    {
        ISharedPreferences sp;

        public AndroidPreferences()
        {
            sp = Forms.Context.GetSharedPreferences("ACD", FileCreationMode.Private);
        }

        public void Set<T>(string key, T value)
        {
            sp.Edit().PutString(key, Serialization.Serialize(value)).Commit();
        }

        public T Get<T>(string key)
        {
            var str = sp.GetString(key, "");
            if (str == "")
                throw new KeyNotFoundException();
            return Serialization.Deserialize<T>(str);
        }
    }
}