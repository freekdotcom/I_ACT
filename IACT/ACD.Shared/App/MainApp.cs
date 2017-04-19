using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Forms;

using System.Diagnostics;

namespace ACD.App
{
    public partial class MainApp : Application
	{
        [DefaultValue(true)]
        public static bool BackEnabled { get; set; }

        public static string LaunchData
		{
			get { try { return Preferences.Get<string>("launchData"); } catch { return null; } }
			set { Preferences.Set("launchData", value ?? ""); }
		}

        public static CustomNavigationPage Navigation { get; private set; }

        public MainApp(int start = (int)CoachPage.Subpages.TipPage)
        {
            Navigation = new CustomNavigationPage(this);
            Navigation.PushAsync(new CoachPage(start));
            MainPage = Navigation;
        }

        //protected override async void OnStart()
        //{
        //    await CheckCrashReport();
        //}

        public static string FindResourceName(string name)
        {
            var assembly = typeof(MainApp).GetTypeInfo().Assembly;
            var names = assembly.GetManifestResourceNames();
            foreach (var res in names)
            {
                if (res.Contains(name))
                {
                    return res;
                }
            }
            throw new Exception("Resource wih name " + name + " not found.");
        }

        public static string ReadTextResource(string name)
        {
            var assembly = typeof(MainApp).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream(FindResourceName(name));
            string text = "";
            using (var reader = new StreamReader(stream))
            {
                text = reader.ReadToEnd();
            }
            return text;
        }

        public static TipStore LoadTipStore(Profile userProfile)
        {
            var db = SQLite.GetConnection(userProfile.ID.ToString());

            var tipStore = new TipStore(db);
            if (!tipStore.Any())
            {
                var tips = Serialization.Deserialize<List<Tip>>(ReadTextResource("tips.json"));
                tips.Shuffle(random);
                tipStore.Add(tips);
            }

            return tipStore;
        }

        static Random random = new Random();
        public static Color RandomColor()
        {
            return Color.FromRgb(random.Next(255), random.Next(255), random.Next(255));
        }
	}
}
