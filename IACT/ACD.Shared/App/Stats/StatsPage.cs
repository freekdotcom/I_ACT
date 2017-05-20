using System;
using System.Linq;


using Xamarin.Forms;

namespace ACD.App
{
    public class StatsPage : ContentPage
    {
        public static readonly string Visit = "StatsPage.Visit"; 

        public enum Subpages
        {
            MoodGraph,
            ActivityGraph,
            DiaryPage
        }

        public static readonly Color BarBackgroundColor = Color.FromRgb(242, 183, 57);
        public static readonly Color BarTextColor = Color.White;
        String first = "This word is ";


        public string NavTitle { get; private set; }

        Coach coach;

        public StatsPage(Coach coach)
        {
            this.coach = coach;

            Title = NavTitle = "Voortgang";
            //Icon = "stats.png";
            Icon = "icon";

            var moodGraph = new GraphPage(
                coach,
                coach.Monitors.First(mn => mn is MoodMonitor),
                "Stemmingsverloop",
                "Hierboven zie je het verloop van je stemming over de afgelopen tijd. Sleep de grafiek om verder terug in de tijd te kijken."
            );

            var activityGraph = new GraphPage(
                coach,
                coach.Monitors.First(mn => mn is ActivityMonitor),
                "Activiteit",
                "Hierboven zie je hoe tevreden je was over je activiteit de afgelopen tijd. Sleep de grafiek om verder terug in de tijd te kijken. Raak een dag aan om te bekijken wat je die dag in je dagboekje geschreven hebt.",
                df => {
                    var entry = coach.UserProfile.Diary.FirstOrDefault(de => de.Day == df.Day);
                    if (entry != null)
                    {
                        Navigation.PushAsync(new DiaryEntryPage(entry));
                    }
                }
            );

            var diaryPage = new DiaryPage(coach);

            Content = new TableView {
                Root = new TableRoot {
                    new TableSection("Stemming") {
                        new TextCell {
                            Text = "Bekijk stemmingsverloop",
#if __ANDROID__
                    TextColor = Color.Black,
#endif
                            Command = new Command(async () => {
                                await Navigation.PushAsync(moodGraph);
                            }),
                            StyleId = "disclosure"
                        }
                    },
                    new TableSection("Activiteit") {
                        new TextCell {
                            Text = "Bekijk activiteit",
#if __ANDROID__
                    TextColor = Color.Black,
#endif

                            Command = new Command(async () => {
                                await Navigation.PushAsync(activityGraph);
                            }),
                            StyleId = "disclosure"
                        },
                        new TextCell {
                            Text = "Open dagboekje",
#if __ANDROID__
                    TextColor = Color.Black,
#endif

                            Command = new Command(async () => {
                                await Navigation.PushAsync(diaryPage);
                            }),
                            StyleId = "disclosure"
                        }
                    },
                },
                Intent = TableIntent.Menu
            };

            MessagingCenter.Subscribe<object, Subpages>(this, Visit, async (sender, page) => {
                switch (page)
                {
                    case Subpages.MoodGraph:
                        await Navigation.PushAsync(moodGraph);
                        break;
                    case Subpages.ActivityGraph:
                        await Navigation.PushAsync(activityGraph);
                        break;
                    case Subpages.DiaryPage:
                        await Navigation.PushAsync(diaryPage);
                        break;
                }
            });
        }



        protected override async void OnAppearing()
        {
            await coach.PerformChecks(true);

            base.OnAppearing();
        }
    }
}

