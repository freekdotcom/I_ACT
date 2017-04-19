using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xamarin.Forms;

namespace ACD.App
{
	public abstract class GraphPage : ContentPage
	{
		StackLayout layout;
		protected Coach Coach;

		public GraphPage(Coach coach)
		{
			Coach = coach;

			Content = layout = new StackLayout {
				//Padding = 50,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
		}

		protected abstract View CreateWeekGraph(DateTime thisWeek);

		protected abstract DateTime FirstDate();

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			await Coach.Authenticate(Navigation);

			var totalWeeks = 1 + (int)((DateTime.Now.StartOfWeek(DayOfWeek.Monday) - FirstDate().StartOfWeek(DayOfWeek.Monday)).TotalDays / 7);

			var currentWeek = 0;

			var leftButton = new Button {
				VerticalOptions = LayoutOptions.Center,
				Text = "<"
			};
			var rightButton = new Button {
				VerticalOptions = LayoutOptions.Center,
				Text = ">"
			};
			var numLabel = new Label {
				//HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.Center,
				Text = (totalWeeks + currentWeek).ToString()
			};

			if (currentWeek == -totalWeeks + 1)
				leftButton.Opacity = 0;
			if (currentWeek == 0)
				rightButton.Opacity = 0;

			leftButton.Clicked += (sender, e) => 
			{
				if (leftButton.Opacity != 0)
				{
					currentWeek--;
					layout.Children.RemoveAt(1);
					layout.Children.Add(CreateWeekGraph(DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(currentWeek * 7)));
					if (currentWeek == -totalWeeks + 1)
						leftButton.Opacity = 0;
					rightButton.Opacity = 1;
					numLabel.Text = (totalWeeks + currentWeek).ToString();
				}
			};

			rightButton.Clicked += (sender, e) => 
			{
				if (rightButton.Opacity != 0)
				{
					currentWeek++;
					layout.Children.RemoveAt(1);
					layout.Children.Add(CreateWeekGraph(DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(currentWeek * 7)));
					if (currentWeek == 0)
						rightButton.Opacity = 0;
					leftButton.Opacity = 1;
					numLabel.Text = (totalWeeks + currentWeek).ToString();
				}
			};

			layout.Children.Replace(
				new StackLayout {
					Orientation = StackOrientation.Horizontal,
					HorizontalOptions = LayoutOptions.Center,
					Children = {
						leftButton,
						/* new Picker {
							Items = { "Week", "Maand" },
							SelectedIndex = 0,
							//HorizontalOptions = LayoutOptions.CenterAndExpand
						}, */
						new Label {
							VerticalOptions = LayoutOptions.Center,
							Text = "Week"
						},
						numLabel,
						rightButton
					}
				},
				CreateWeekGraph(DateTime.Now.StartOfWeek(DayOfWeek.Monday))
			);
		}
	}
}
