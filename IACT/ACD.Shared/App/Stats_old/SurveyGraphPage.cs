using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xamarin.Forms;

namespace ACD.App
{
	public class SurveyGraphPage : GraphPage
	{
		public Survey Survey { get; set; }

		public SurveyGraphPage(Coach coach, Survey survey) : base(coach)
		{
			Survey = survey;
			Title = survey.Title;
		}

		protected override View CreateWeekGraph(DateTime thisWeek)
		{
			var responses = Coach.UserProfile.GetResponsesForSurvey(Survey);
				//.Where(r => r.Time >= thisWeek && r.Time <= thisWeek.AddDays(7));
			return new LabeledGraph(
				xAxis: 7f,
				yAxis: Survey.MaxScore,
				x: responses.Select(r => (float)(r.Time - thisWeek).TotalDays).ToArray(),
				y: responses.Select(r => (float)r.Score).ToArray(),
				xLabels: new Dictionary<float, View> {
					{ 0*7/6f, new Label { Text = "MA", XAlign = TextAlignment.Center } },
					{ 1*7/6f, new Label { Text = "DI", XAlign = TextAlignment.Center } },
					{ 2*7/6f, new Label { Text = "WO", XAlign = TextAlignment.Center } },
					{ 3*7/6f, new Label { Text = "DO", XAlign = TextAlignment.Center } },
					{ 4*7/6f, new Label { Text = "VR", XAlign = TextAlignment.Center } },
					{ 5*7/6f, new Label { Text = "ZA", XAlign = TextAlignment.Center } },
					{ 6*7/6f, new Label { Text = "ZO", XAlign = TextAlignment.Center } },
				},
				yLabels: new Dictionary<float, View> {
					{ 0, new Label { Text = "0", XAlign = TextAlignment.Center } },
					{ Survey.MaxScore, new Label { Text = Survey.MaxScore.ToString(), XAlign = TextAlignment.Center } }
				}
			) {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
		}

		protected override DateTime FirstDate()
		{
			var responses = Coach.UserProfile.GetResponsesForSurvey(Survey);
			if (!responses.Any())
				return DateTime.Now;
			return responses.OrderBy(r => r.Time).First().Time;
		}
	}
}
