using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public partial class ManageTipsPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromHex("68d7c6");
        public static readonly Color BarTextColor = Color.White;

        Coach coach;
        SelectTipsPage selectTipsPage;

        TextCell selectTips;

        public ManageTipsPage(Coach coach)
        {
            this.coach = coach;

            Title = "Tips instellen";
            NavigationPage.SetBackButtonTitle(this, "Terug");

            selectTipsPage = new SelectTipsPage(coach);

            selectTips = new TextCell {
                Text = "Tips selecteren",
                Command = new Command(async () => {
                    await Navigation.PushAsync(selectTipsPage);
                }),
                StyleId = "disclosure"
            };

            var times = coach.Scheduler.GetTimes();

            var timeSection = new TableSection("Tijden waarop je een tip ontvangt:");

            Action renderTimeSection = () => {
                timeSection.Clear();
                /* timeSection.Replace(
                    new TextCell {
                        Text = "Tijden waarop u een tip ontvangt:",
                        TextColor = Color.Yellow,
                        IsEnabled = false,
                        Detail = "bla",
                        DetailColor = Color.Green
                    }
                ); */
                foreach (TimeSpan time in times)
                {
                    timeSection.Add(new TimeCell(coach.Scheduler, time));
                }
                timeSection.Add(new TextCell {
                    Text = "Tijd toevoegen",
                    Command = new Command(async () => {
                        await ShowSelectTimeAlert(coach.Scheduler);
                    }),
                    StyleId = "disclosure"
                });
            };

            times.AsObservable().CollectionChanged += (sender, e) => renderTimeSection();
            renderTimeSection();

            Content = new TableView {
                Root = new TableRoot {
                    new TableSection {
                        selectTips
                    },
                    timeSection
                },
                Intent = TableIntent.Settings
            };
        }

        protected override async void OnAppearing()
        {
            await coach.PerformChecks(true);

            base.OnAppearing();
        }

        public class TimeCell : TextCell
        {
            public TimeCell(TipScheduler scheduler, TimeSpan time)
            {
                Text = "Om " + time.ToString(@"hh\:mm") + " uur";
                Command = new Command(async () => {
                    await ShowSelectTimeAlert(scheduler, time);
                });
                //StyleId = "disclosure";
                var edit = new MenuItem {
                    Text = "Wijzig"
                };
                edit.Clicked += async (sender, e) => {
                    await ShowSelectTimeAlert(scheduler, time);
                };
                var delete = new MenuItem {
                    Text = "Verwijder",
                    IsDestructive = true
                };
                delete.Clicked += (sender, e) => {
                    scheduler.RemoveTime(time);
                };
                ContextActions.Add(edit, delete);
            }
        }

        static async Task ShowSelectTimeAlert(TipScheduler scheduler, TimeSpan? maybeTime = null)
        {
            var isEdit = maybeTime != null;
            var time = maybeTime ?? DateTime.Now.TimeOfDay;

            var tp = new TimePicker {
                Format = "HH:mm",
                Time = time.MinutesOnly(),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = 60
            };
            var cancelButton = new AlertButton {
                Text = isEdit ? "Verwijder" : "Annuleer",
                IsDestructive = isEdit,
                Action = () => {
                    if (isEdit)
                        scheduler.RemoveTime(time);
                    return false;
                }
            };

            tp.PropertyChanged += (sender, e) => {
                if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
                    return; // TODO: change delete into cancel button
            };

            await Alert.Show(
                "Tijd " + (isEdit ? "bewerken" : "toevoegen"),
                "Voer hieronder de tijd in waarop je een tip wil ontvangen.",
                tp,
                cancelButton,
                new AlertButton {
                    Text = "Opslaan",
                    IsPreferred = true,
                    Action = () => {
                        var newTime = tp.Time.MinutesOnly();
                        if (!isEdit || newTime != time)
                        {
                            if (isEdit) scheduler.RemoveTime(time);
                            scheduler.AddTime(newTime);
                        }
                        return false;
                    }
                }
            );
        }
    }
}

