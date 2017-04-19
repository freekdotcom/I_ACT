using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace ACD.App
{
    public class SurveyListPage : ContentPage
    {
        Coach coach;
        ListView surveyList;

		public SurveyListPage(Coach coach, Action<Survey, INavigation> onSelect)
        {
            this.coach = coach;

            Title = "Feedback";

            surveyList = new ListView();
            surveyList.ItemTapped += (sender, args) =>
            {
                var survey = ((Survey)args.Item);
				onSelect(survey, Navigation);
            };

            Content = surveyList;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await coach.Authenticate(Navigation);

            surveyList.ItemsSource = coach.Surveys;
        }
    }
}
