using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using Xamarin.Forms;

namespace ACD.App
{
    public class StatsPage : TabbedPage
    {
		Coach coach;

        public StatsPage(Coach coach)
        {
            Title = "Voortgang";

			this.coach = coach;

            Children.Add(new MoodGraphPage(coach));

			Children.Add(new SurveyListPage(coach, (s, n) => n.PushAsync(new SurveyGraphPage(coach, s))) {
                Title = "Vragenlijsten"
            });
        }

		protected override async void OnAppearing()
		{
			base.OnAppearing();

            await coach.Authenticate(Navigation);
		}
    }
}
