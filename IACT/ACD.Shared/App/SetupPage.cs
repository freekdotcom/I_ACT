using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Java.Util;
using Xamarin.Forms;

namespace ACD.App
{
    public partial class SetupPage : NavigationPage
    {
        public static readonly Color BarBackgroundColor = Color.FromHex("fa5755");
        public static readonly Color BarTextColor = Color.White;

        StackLayout layout;
        View full;
        SelectTipsList tipsList;
        TipStore tipStore;
        Label error;
        Button nextButton;

        public SetupPage()
        {
            ((NavigationPage)this).BarBackgroundColor = SetupPage.BarBackgroundColor;
            ((NavigationPage)this).BarTextColor = SetupPage.BarTextColor;

            var contentPage = new ContentPage
            {
                Title = "App instellen",
                Content = (full = new StackLayout
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Children = { new ScrollView { Content =
                        (layout = new StackLayout {
                            Spacing = StyleKit.AutoSpacing.Large,
                            Padding = StyleKit.AutoPadding,
                            VerticalOptions = LayoutOptions.StartAndExpand
                        })
                    }
                    }
                })
            };

            full.SizeChanged += (sender, e) =>
            {
                layout.HeightRequest = full.Height - layout.Padding.VerticalThickness;
            };

            PushAsync(contentPage);

            Start();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected async void Start()
        {
            MainApp.BackEnabled = false;

            var profile = await Profile.New();
            tipStore = MainApp.LoadTipStore(profile);

            var titleLabel = new Label
            {
                Text = "Welkom!",
                Style = StyleKit.AutoDarkLabelStyles.Display
            };

            var explainLabel = new Label
            {
                Text = "Welkom bij je nieuwe app! Om te beginnen willen we je vragen om een aantal dingen in te stellen zodat de app beter op je persoonlijk afgestemd kan worden.",
                Style = StyleKit.AutoDarkLabelStyles.Body
            };

            var tpStyles = StyleKit.AutoDarkStyles<TimePicker>();
            var entryStyles = StyleKit.AutoDarkStyles<Entry>();

            var topLayout = new StackLayout
            {
                Spacing = StyleKit.AutoSpacing.Medium,
                VerticalOptions = LayoutOptions.Start,
                Children = {
                    titleLabel,
                    explainLabel
                }
            };

            layout.Children.Add(topLayout);

            error = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                Style = StyleKit.AutoDarkLabelStyles.Caption,
                TextColor = Color.Red,
                IsVisible = false,

            };

            nextButton = new Button
            {
                Text = "Volgende",
                Style = StyleKit.AutoLightButtonStyles.Body,
                TextColor = Color.White,
                HeightRequest = 4 * StyleKit.AutoSpacing.Small,
                BackgroundColor = Color.FromHex("fa5755"),
                VerticalOptions = LayoutOptions.EndAndExpand
            };

            var midLayout = new StackLayout
            {
                Spacing = StyleKit.AutoSpacing.Medium,
                VerticalOptions = LayoutOptions.Start
            };

            layout.Children.Add(midLayout);

            layout.Children.Add(error);
            layout.Children.Add(nextButton);

            nextButton.IsEnabled = true;
            await nextButton.GetEventAsync<EventArgs>("Clicked");
            nextButton.IsEnabled = false;

            await PlatformSetup(titleLabel, explainLabel, nextButton);

            titleLabel.Text = "Tips selecteren";
            explainLabel.Text = "Hier kan je instellen welke tips je nuttig lijken en je graag zou willen ontvangen." +
                "\nVink deze tips aan en druk daarna op volgende om verder te gaan. Druk op de tekst van een tip om hem helemaal te lezen.";

            tipsList = new SelectTipsList
            {
                ItemsSource = tipStore.GetAll(),
                InputTransparent = false,
            };

            error.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() => error.IsVisible = false)
            });

            //Oorzaak op android
            #if _IOS_
                tipsList.ItemAppearing += (sender, e) => error.IsVisible = false;
            #endif
            midLayout.Children.Add(tipsList);

            if (Device.Idiom == TargetIdiom.Phone)
            {
                midLayout.Padding = StyleKit.AutoPadding.With(top: 0, bottom: 0).Negative();
            }

            error.Text = "Vink alstublieft minstens drie tips aan om verder te gaan.";
            do
            {
                nextButton.IsEnabled = true;
                await nextButton.GetEventAsync<EventArgs>("Clicked");
                error.IsVisible = true;
                nextButton.IsEnabled = false;
                error.IsVisible = true;

            } while (tipStore.Count(tip => tip.Enabled) < 3);

            midLayout.Padding = 0;

            List<TimeSpan> tipsTimes = new List<TimeSpan>();

            #if __ANDROID__
            tipsList.ItemAppearing += (sender, e) => error.IsVisible = false;
            #endif

            error.IsVisible = false;

            Func<int, TimePicker> createTP = (int hours) => new TimePicker
            {
                Time = TimeSpan.FromHours(hours),
                Format = "HH:mm",
#if __ANDROID__
                TextColor = Color.Black,
#endif
            };

            var timePickers = new List<TimePicker>();

            var timeGrid = new Grid
            {
                VerticalOptions = LayoutOptions.Fill,
                RowDefinitions = { },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
                }
            };

            Action<TimePicker> addTP = (tp) =>
            {
                int index = timePickers.Count;
                timePickers.Add(tp);
                tp.TextColor = Color.Black;
                timeGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                Label timeGridLabel = new Label
                {
                    Text = "Om: ",
                    Style = StyleKit.AutoDarkLabelStyles.Body,
                    #if __ANDROID__
                    TextColor = Color.Black,
                    #endif
                    HorizontalTextAlignment = TextAlignment.End,
                    VerticalTextAlignment = TextAlignment.Center
                };

                timeGridLabel.TextColor = Color.Black;

                timeGrid.Children.Add(timeGridLabel, 0, index);
                timeGrid.Children.Add(tp, 1, index);
            };

            addTP(createTP(8));
            addTP(createTP(13));

            var addTime = new Button
            {
                Text = "Nog een tijd toevoegen",
                Command = new Command(async () =>
                {
                    var tp = createTP(12);
                    addTP(tp);
                    tp.Focus();
                    //await scrollTimes.ScrollToAsync(tp, ScrollToPosition.MakeVisible, true);
                }),
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Fill,
                HeightRequest = 50
            };

            var scrollTimes = new ScrollView
            {
                Content = new StackLayout
                {
                    Children = { timeGrid, addTime }
                },
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            titleLabel.Text = "Tips selecteren";
            explainLabel.Text = "Deze tips worden gedurende de dag vernieuwd. Hieronder kan je aangeven op welke tijden je graag een tip wil ontvangen.";
            midLayout.Children.Clear();
            midLayout.Children.Add(scrollTimes);

            nextButton.IsEnabled = true;
            await nextButton.GetEventAsync<EventArgs>("Clicked");
            nextButton.IsEnabled = false;

            foreach (var tp in timePickers)
            {
                tipsTimes.Add(tp.Time);
            }

            var tipScheduler = new TipScheduler(profile, tipStore, tipsTimes);

            var morningTime = new TimePicker
            {
                Time = TimeSpan.FromHours(9),
                Format = "HH:mm",
                Style = tpStyles.Body
            };

            var eveningTime = new TimePicker
            {
                Time = TimeSpan.FromHours(18),
                Format = "HH:mm",
                Style = tpStyles.Body
            };

            timeGrid = new Grid
            {
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            timeGrid.Children.Add(new Label
            {
                Text = "'s Ochtends om: ",
                Style = StyleKit.AutoDarkLabelStyles.Body,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
            timeGrid.Children.Add(morningTime, 1, 0);
            timeGrid.Children.Add(new Label
            {
                Text = "'s Avonds om: ",
                Style = StyleKit.AutoDarkLabelStyles.Body,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 1);
            timeGrid.Children.Add(eveningTime, 1, 1);

            titleLabel.Text = "Stemmingscheck";
            explainLabel.Text = "Deze app zal je tweemaal daags vragen hoe het met je gaat. Als je een voorkeur hebt voor de tijd waarop dat gebeurt kan je deze hieronder opgeven.";
            midLayout.Children.Replace(timeGrid);

            nextButton.IsEnabled = true;
            await nextButton.GetEventAsync<EventArgs>("Clicked");
            nextButton.IsEnabled = false;

            var moodMonitor = new MoodMonitor(profile);
            moodMonitor.MorningCheck = morningTime.Time;
            moodMonitor.EveningCheck = eveningTime.Time;

            eveningTime = new TimePicker
            {
                Time = TimeSpan.FromHours(20),
                Format = "HH:mm",
                Style = tpStyles.Body
            };

            timeGrid = new Grid
            {
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };
            timeGrid.Children.Add(new Label
            {
                Text = "'s Avonds om: ",
                Style = StyleKit.AutoDarkLabelStyles.Body,
                HorizontalTextAlignment = TextAlignment.End,
                VerticalTextAlignment = TextAlignment.Center
            }, 0, 0);
            timeGrid.Children.Add(eveningTime, 1, 0);

            titleLabel.Text = "Dagevaluatie";
            explainLabel.Text = "Deze app zal je ook aan het einde van de dag vragen je activiteiten die dag te beoordelen. Als je een voorkeur hebt voor de tijd waarop dat gebeurt kan je deze hieronder opgeven.";
            midLayout.Children.Replace(timeGrid);

            nextButton.IsEnabled = true;
            await nextButton.GetEventAsync<EventArgs>("Clicked");
            nextButton.IsEnabled = false;

            var activityMonitor = new ActivityMonitor(profile);
            activityMonitor.Check = eveningTime.Time;

            midLayout.Children.Clear();

            /* For scrolling when keyboard is up */
            //page.Content = new ScrollView { Content = layout, InputTransparent = true };

            var passEntry = new Entry
            {
                Placeholder = "Uw gewenste pincode hier...",
                Keyboard = Keyboard.Numeric,
                Style = entryStyles.Body
            };

            titleLabel.Text = "Pincode instellen";
            explainLabel.Text = "Vul hieronder a.u.b. een makkelijk te onthouden code van 4 cijfers in om de gegevens binnen de app persoonlijk te houden.";
            midLayout.Children.Add(
                passEntry
            );

            string passText = null;
            error.Text = "Vul a.u.b. een geldige pincode in.";
            passEntry.TextChanged += (sender, e) =>
            {
                if (e.NewTextValue?.Length > 4)
                    passEntry.Text = e.OldTextValue;
                else
                    error.IsVisible = false;
            };

            do
            {
                nextButton.IsEnabled = true;
                await nextButton.GetEventAsync<EventArgs>("Clicked");
                nextButton.IsEnabled = false;
                passText = passEntry.Text;
                error.IsVisible = true;
            } while (!Coach.ValidPassword(passText));

            error.IsVisible = false;

            var password = Coach.HashPassword(passText);
            Preferences.Set(profile.ID + "_password", password);

            midLayout.Children.Remove(passEntry);

            var mailEntry = new Entry
            {
                Placeholder = "Uw e-mailadres hier...",
                Keyboard = Keyboard.Email,
                Style = entryStyles.Body
            };

            titleLabel.Text = "E-mail instellen";
            explainLabel.Text = "Mocht je je pincode vergeten, willen wij je graag een nieuwe sturen zodat je de app kunt blijven gebruiken. Daarvoor hebben we jouw e-mailadres nodig.\nVul hieronder a.u.b. een e-mailadres in waar je toegang tot hebt, zodat je ook altijd toegang hebt tot de app.";
            midLayout.Children.Add(
                mailEntry
            );

            string mailText = null;
            /* error.Text = "Vul a.u.b. een geldige pincode in.";
            passEntry.TextChanged += (sender, e) => {
                if (e.NewTextValue?.Length > 4)
                    passEntry.Text = e.OldTextValue;
                else
                    error.IsVisible = false;
            }; */

            mailEntry.TextChanged += (sender, e) =>
            {
                var trimmed = mailEntry.Text.Trim();
                if (trimmed != mailEntry.Text)
                    mailEntry.Text = trimmed;
            };

            //do
            //{
            nextButton.IsEnabled = true;
            await nextButton.GetEventAsync<EventArgs>("Clicked");
            nextButton.IsEnabled = false;
            mailText = mailEntry.Text;
            //error.IsVisible = true;
            //} while (!Coach.ValidPassword(passText));

            error.IsVisible = false;

            Preferences.Set(profile.ID + "_email", mailText);

            midLayout.Children.Remove(mailEntry);

            titleLabel.Text = "Dank je wel!";
            explainLabel.Text = "Je bent klaar met instellen!\nDruk op volgende om de app te gaan gebruiken.";

            Preferences.Set("user", profile.ID);

            MainApp.BackEnabled = true;

            nextButton.IsEnabled = true;
            await nextButton.GetEventAsync<EventArgs>("Clicked");
            nextButton.IsEnabled = false;

            Navigation.PopModalAsync();

            MessagingCenter.Send(this, CoachPage.CoachMessage,
                new Coach(profile, tipStore, tipScheduler, new List<Monitor> { moodMonitor, activityMonitor }));
        }
    }
}
