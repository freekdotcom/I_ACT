using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public partial class CoachPage : TabbedPage
    {
        public const string NavigateMessage = "Navigate",
                            CoachMessage = "Coach";

        public enum Subpages
        {
            StatsPage = 0,
            TipPage = 1,
            SettingsPage = 2
        }

        Page[] pages;
        int firstPage;

        public CoachPage(int start)
        {
            firstPage = start;

            NavigationPage.SetBackButtonTitle(this, "Terug");

            MainApp.Navigation.BarBackgroundColor = Color.FromHex("68d7c6");
            MainApp.Navigation.BarTextColor = Color.White;

            this.SetBinding(TabbedPage.TitleProperty, "CurrentPage.NavTitle");
            this.BindingContext = this;

            CurrentPageChanged += (sender, e) => {
                MainApp.Navigation?.CurrentPageChanged();
            };
                   
            try
            {
                int userID = Preferences.Get<int>("user");
                var profile = Profile.Get(userID).Result;
                Setup(new Coach(profile, MainApp.LoadTipStore(profile)));
            }
            catch (KeyNotFoundException)
            {
                Navigation.PushModalAsync(new SetupPage());
                MessagingCenter.Subscribe<SetupPage, Coach>(this, CoachMessage,
                    (sender, coach) => Setup(coach));
            }
        }

        void Setup(Coach coach)
        {
            pages = new Page[]
            {
                new StatsPage(coach),
                new TipPage(coach),
                new SettingsPage(coach)
            };

            Console.WriteLine("Adding stats page, icon: " + pages[0].Icon.File);
            Children.Add(pages[0]);
            Console.WriteLine("Adding tip page, icon: " + pages[1].Icon.File);
            Children.Add(pages[1]);
            Console.WriteLine("Adding settings page, icon: " + pages[2].Icon.File);
            Children.Add(pages[2]);
            Console.WriteLine("Done!");

            Navigate(firstPage);

            MessagingCenter.Subscribe<object, Subpages>(this, NavigateMessage,
                async (sender, index) => await Navigate(index));

            PlatformSetup(coach);
        }

        async Task Navigate(int index)
        {
            SelectedItem = pages[index];
            if (Navigation.NavigationStack.Count > 0)
                await Navigation.PopToRootAsync();
        }

        async Task Navigate(Subpages page)
        {
            await Navigate((int)page);
        }
    }
}

