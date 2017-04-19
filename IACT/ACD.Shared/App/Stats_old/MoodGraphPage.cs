using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xamarin.Forms;

namespace ACD.App
{
    public class MoodGraphPage : GraphPage
    {
		public MoodGraphPage(Coach coach) : base(coach)
        {
            Title = "Stemming";
        }

		protected override View CreateWeekGraph(DateTime thisWeek)
		{
			var moodStatsWeek = Coach.MoodStats;//.Where(t => t.Item1 >= thisWeek && t.Item1 <= thisWeek.AddDays(7));
			return new LabeledGraph(
				xAxis: 7f,
				yAxis: 1,
				x: moodStatsWeek.Select(t => (float)(t.Item1 - thisWeek).TotalDays).ToArray(),
				y: moodStatsWeek.Select(t => (float)t.Item2).ToArray(),
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
					{ 0, new Image { Source = ImageSource.FromResource(App.GetResourceName("face-sad.png")), Scale = 0.7 } },
					{ 1, new Image { Source = ImageSource.FromResource(App.GetResourceName("face-smile.png")), Scale = 0.7 } }
				}
			) {
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.Center
			};
		}

		protected override DateTime FirstDate()
		{
			return (Coach.MoodStats.OrderBy(t => t.Item1).FirstOrDefault() ?? Tuple.Create(DateTime.Now, 0.0)).Item1;
		}
    }
}
