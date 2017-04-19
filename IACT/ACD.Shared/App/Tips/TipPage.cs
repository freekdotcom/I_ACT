using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class TipPage : ContentPage
    {
        public const string NewTip = "NewTip";

        public static readonly Color BarBackgroundColor = Color.FromHex("68d7c6");
        public static readonly Color BarTextColor = Color.White;

        public string NavTitle { get; private set; }

		Coach coach;
		StackLayout layout;

        ManageTipsPage manageTipsPage;

        public TipPage(Coach coach, string title = "Tips")
        {
			this.coach = coach;

            Title = title;
            //Icon = "tips3.png";
            Icon = "icon";
            NavigationPage.SetBackButtonTitle(this, "Terug");

            var tipView = new TipView {
                BindingContext = coach.Scheduler
            };

            tipView.SetBinding(TipView.TipProperty, "CurrentTip");

			layout = new StackLayout {
                Spacing = StyleKit.AutoSpacing.Small,
                Padding = StyleKit.AutoPaddingLight,
                Children = {
                    new Label {
                        Text = "We hebben een tip voor je:",
                        HorizontalTextAlignment = TextAlignment.Center,
                        Style = StyleKit.AutoDarkLabelStyles.Caption
                    },
					tipView
				}
			};

            Content = new ScrollView {
                Content = layout
            };

            manageTipsPage = new ManageTipsPage(coach);

            var manageItem = new ToolbarItem {
                Text = "Tips instellen"
            };
            manageItem.Command = new Command(async () => {
                await Navigation.PushAsync(manageTipsPage);
            });
            ToolbarItems.Add(manageItem);
        }

		protected override async void OnAppearing()
		{
            await coach.PerformChecks(false);

            base.OnAppearing();
		}
    }
}
