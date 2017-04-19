using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace ACD
{
    public interface IPreferences
    {
        void Set<T>(string key, T value);
        T Get<T>(string key);
    }

    public static class Preferences
    {
        public static void Set<T>(string key, T value)
        {
            DependencyService.Get<IPreferences>().Set(key, value);
        }
        public static T Get<T>(string key)
        {
            return DependencyService.Get<IPreferences>().Get<T>(key);
        }
		public static T GetOr<T>(string key, T def)
		{
			try
			{
				return Get<T>(key);
			}
			catch
			{
				return def;
			}
		}
    }
}
