using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class SelectTipsPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromHex("68d7c6");
        public static readonly Color BarTextColor = Color.White;

        private Coach coach;
        private ListView tipsList;

        public SelectTipsPage(Coach coach)
        {
            this.coach = coach;

            Title = "Tips selecteren";
            NavigationPage.SetBackButtonTitle(this, "Terug");

            tipsList = new SelectTipsList();
            Content = new StackLayout {
                Spacing = 0,
                Padding = 0,
                Children = {
                    new StackLayout {
                        Spacing = StyleKit.AutoSpacing.Small,
                        Padding = StyleKit.AutoPaddingLight,
                        Children = {
                            new Label {
                                Text = "Selecteer hier welke tips je wil ontvangen. Druk op de tekst van een tip om hem helemaal te lezen.",
                                HorizontalTextAlignment = TextAlignment.Center,
                                Style = StyleKit.AutoDarkLabelStyles.Caption
                            }
                        }
                    },
                    tipsList
                }
            };

            tipsList.ItemsSource = coach.Tips.GetAll();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await coach.PerformChecks(true);
        }
    }
}
