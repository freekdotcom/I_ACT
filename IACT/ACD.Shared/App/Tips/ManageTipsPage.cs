using System;
using System.Threading.Tasks;

using Xamarin.Forms;


namespace ACD.App
{
    public partial class ManageTipsPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromHex("68d7c6");
        public static readonly Color BarTextColor = Color.White;

        Label test = new Label();
        Coach coach;
        SelectTipsPage selectTipsPage;

        TextCell selectTips;

        public ManageTipsPage(Coach coach)
        {
            this.coach = coach;

            test.Text = "Test";

            Title = "Tips instellen";
            NavigationPage.SetBackButtonTitle(this, "Terug");

            selectTipsPage = new SelectTipsPage(coach);

            selectTips = createNewTextCells("Tips instellen");

            var times = coach.Scheduler.GetTimes();

            var timeSection = new TableSection(test.Text);

            Action renderTimeSection = () =>
            {
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
                    var newTimeCell = new TimeCell(coach.Scheduler, time);
#if __ANDROID__
                    newTimeCell.TextColor = Color.Black;
#endif
                    timeSection.Add(newTimeCell);
                }
                timeSection.Add((new TextCell
                {
                    Text = "Tijd toevoegen",
#if __ANDROID__
                    TextColor = Color.Black,
#endif
                    Command = new Command(async () =>
                    {
                        await ShowSelectTimeAlert(coach.Scheduler);
                      }),
                    StyleId = "disclosure"
                }));
            };

            times.AsObservable().CollectionChanged += (sender, e) => renderTimeSection();
            renderTimeSection();

            Content = new TableView
            {
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

        public TextCell createNewTextCells(string text)
        {
            return new TextCell
            {
                Text = text,
#if __ANDROID__
                TextColor = Color.Black,
#endif
                Command = new Command(async () =>
                {
                    await Navigation.PushAsync(selectTipsPage);
                }),
                StyleId = "disclosure"
            };
        }

        public class TimeCell : TextCell
        {
            public TimeCell(TipScheduler scheduler, TimeSpan time)
            {
                Text = "Om " + time.ToString(@"hh\:mm") + " uur";
                Command = new Command(async () =>
                {
                  await ShowSelectTimeAlert(scheduler, time);

                });
                //StyleId = "disclosure";
                var edit = new MenuItem
                {
                    Text = "Wijzig"
                };
                edit.Clicked += async (sender, e) =>
                {
                    await ShowSelectTimeAlert(scheduler, time);
                };
                var delete = new MenuItem
                {
                    Text = "Verwijder",
                    IsDestructive = true
                };
                delete.Clicked += (sender, e) =>
                {
                    scheduler.RemoveTime(time);
                };
                ContextActions.Add(edit, delete);
            }
        }


        static async Task ShowSelectTimeAndroid(TipScheduler scheduler, TimeSpan? maybeTime = null)
        {

            Func<int, TimePicker> createTP = (int hours) => new TimePicker
            {
                Time = TimeSpan.FromHours(hours),
                Format = "HH:mm"
            };            var tp = createTP(12);

            tp.Focus();

            scheduler.AddTime(tp.Time.MinutesOnly());

        }

        static async Task ShowSelectTimeAlert(TipScheduler scheduler, TimeSpan? maybeTime = null)
        {
            var isEdit = maybeTime != null;
            var time = maybeTime ?? DateTime.Now.TimeOfDay;

            var tp = new TimePicker
            {
                Format = "HH:mm",
                Time = time.MinutesOnly()
            };
            var cancelButton = new AlertButton
            {
                Text = isEdit ? "Verwijder" : "Annuleer",
                IsDestructive = isEdit,
                Action = () =>
                {
                    if (isEdit)
                        scheduler.RemoveTime(time);
                    return false;
                }
            };

            tp.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
                    return; // TODO: change delete into cancel button
            };

            await Alert.Show(
                "Tijd " + (isEdit ? "bewerken" : "toevoegen"),
                "Voer hieronder de tijd in waarop je een tip wil ontvangen.",
                tp,
                cancelButton,
                new AlertButton
                {
                    Text = "Opslaan",
                    IsPreferred = true,
                    Action = () =>
                    {
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

