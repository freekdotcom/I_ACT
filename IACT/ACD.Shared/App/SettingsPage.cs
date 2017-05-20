using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;

namespace ACD.App
{
	public class SettingsPage : ContentPage
	{
        public static readonly Color BarBackgroundColor = Color.FromHex("fa5755");
        public static readonly Color BarTextColor = Color.White;

        public string NavTitle { get; private set; }

        Coach coach;

        public SettingsPage(Coach coach)
		{
            this.coach = coach;

			Title = NavTitle = "Instellingen";
            //Icon = "settings.png";
            Icon = "icon";

            var tpStyles = StyleKit.AutoDarkStyles<TimePicker>();

            var moodMonitor = coach.Monitors.Where(m => m is MoodMonitor).First();
            var morningCheck = new TimePicker {
                BindingContext = moodMonitor,
                Format = "HH:mm",
                Style = tpStyles.Body
			};
			morningCheck.SetBinding(TimePicker.TimeProperty, "MorningCheck", BindingMode.TwoWay);
            var eveningCheck = new TimePicker {
                BindingContext = moodMonitor,
                Format = "HH:mm",
                Style = tpStyles.Body
            };
            eveningCheck.SetBinding(TimePicker.TimeProperty, "EveningCheck", BindingMode.TwoWay);

            var activityMonitor = coach.Monitors.Where(m => m is ActivityMonitor).First();
            var activityCheck = new TimePicker {
                BindingContext = activityMonitor,
                Format = "HH:mm",
                Style = tpStyles.Body
            };
            activityCheck.SetBinding(TimePicker.TimeProperty, "Check", BindingMode.TwoWay);

            Label exportLabel;
            var exportCell = new ViewCell {
                View = new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(15, 5),
                    Children = {
                        (exportLabel = new Label { Text = "Exporteer gebruikersprofiel",
#if __ANDROID__
                    TextColor = Color.Black,
#endif
                            VerticalTextAlignment = TextAlignment.Center,
                            Style = StyleKit.AutoDarkLabelStyles.Body })
                    }
                },
                StyleId = "disclosure"
            };
            exportCell.Tapped += (sender, e) => {
                MessagingCenter.Send<View, string>(exportLabel, "Share", coach.UserProfile.Serialize(coach));
            };

            Content = new TableView {
                Root = new TableRoot {
                    new TableSection("Tijden stemmingscheck") {
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                Padding = new Thickness(15, 5),
                                Children = {
                                    new Label { Text = "'s Ochtends om: ",
#if __ANDROID__
                    TextColor = Color.Black,
#endif
                                        VerticalTextAlignment = TextAlignment.Center,
                                        Style = StyleKit.AutoDarkLabelStyles.Body },
                                    morningCheck
                                }
                            }
                        },
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                Padding = new Thickness(15, 5),
                                Children = {
                                    new Label { Text = "'s Avonds om: ",
#if __ANDROID__
                    TextColor = Color.Black,
#endif
                                        VerticalTextAlignment = TextAlignment.Center,
                                        Style = StyleKit.AutoDarkLabelStyles.Body },
                                    eveningCheck
                                }
                            }
                        },
                    },
                    new TableSection("Tijd dagevaluatie") {
                        new ViewCell {
                            View = new StackLayout {
                                Orientation = StackOrientation.Horizontal,
                                Padding = new Thickness(15, 5),
                                Children = {
                                    new Label { Text = "'s Avonds om: ",
#if __ANDROID__
                    TextColor = Color.Black,

#endif
                                        VerticalTextAlignment = TextAlignment.Center,
                                        Style = StyleKit.AutoDarkLabelStyles.Body },
                                    activityCheck
                                }
                            }
                        },
                    },


                    new TableSection("Toegangsinstellingen") {
                        new TextCell {
                            Text = "Pincode opnieuw instellen",
#if __ANDROID__
                    TextColor = Color.Black,
#endif

                            Command = new Command(async () => {
                                await Navigation.PushAsync(new PasswordPage(coach));
                            }),
                            StyleId = "disclosure"
                        },
                        new TextCell {
                            Text = "Emailadres opnieuw instellen",
                            TextColor = Color.Black,

                            Command = new Command(async () => {
                                await Navigation.PushAsync(new EmailPage(coach));
                            }),
                            StyleId = "disclosure"
                        },
                    },
                    new TableSection("Geavanceerd") {
                        exportCell,
                        //new TextCell {
                        //    Text = "Forceer nieuwe tip",
                        //    Command = new Command(async () => {
                        //        coach.Scheduler.ForceNextTip();
                        //        await Alert.Show(
                        //            "Nieuwe tip",
                        //            "Er staat een nieuwe tip voor u klaar!",
                        //            null,
                        //            new AlertButton
                        //            {
                        //                Text = "Later",
                        //                Action = () => false
                        //            },
                        //            new AlertButton
                        //            {
                        //                Text = "Naar tip",
                        //                IsPreferred = true,
                        //                Action = () => {
                        //                    Navigation.PushAsync(new TipPage(coach, "Nieuwe tip"));
                        //                    return false;
                        //                }
                        //            }
                        //        );
                        //    })
                        //},
                        //new TextCell {
                        //    Text = "Test stemmingscheck",
                        //    Command = new Command(async () => {
                        //        var monitor = new MoodMonitor(coach.UserProfile);
                        //        await monitor.Perform();
                        //    })
                        //},
                        //new TextCell {
                        //    Text = "Test dagevaluatie",
                        //    Command = new Command(async () => {
                        //        var monitor = new ActivityMonitor(coach.UserProfile);
                        //        await monitor.Perform();
                        //    })
                        //},
                    }
                },
                Intent = TableIntent.Settings
			};
		}

        protected override async void OnAppearing()
        {
            await coach.PerformChecks(true);
        }

		class PasswordPage : ContentPage
		{
            public PasswordPage(Coach coach)
			{
                Show(coach);
            }

            async void Show(Coach coach)
            {
                StackLayout layout;

                Content = layout = new StackLayout {
                    Spacing = StyleKit.AutoSpacing.Large,
                    Padding = StyleKit.AutoPadding,
                    VerticalOptions = LayoutOptions.StartAndExpand
                };

                var titleLabel = new Label {
                    Style = StyleKit.AutoDarkLabelStyles.Display
                };

                var explainLabel = new Label {
                    Style = StyleKit.AutoDarkLabelStyles.Body
                };

                var topLayout = new StackLayout {
                    Spacing = StyleKit.AutoSpacing.Medium,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Children = {
                        titleLabel,
                        explainLabel
                    }
                };

                layout.Children.Add(topLayout);

                var error = new Label {
                    HorizontalTextAlignment = TextAlignment.Center,
                    Style = StyleKit.AutoDarkLabelStyles.Caption,
                    TextColor = Color.Red,
                    IsVisible = false
                };

                var nextButton = new Button {
                    Text = "Opslaan",
                    TextColor = Color.White,
                    Font = Font.SystemFontOfSize(16),
                    BackgroundColor = Color.FromHex("fa5755"),
                    VerticalOptions = LayoutOptions.EndAndExpand
                };

                var midLayout = new StackLayout {
                    Spacing = StyleKit.AutoSpacing.Medium,
                    VerticalOptions = LayoutOptions.StartAndExpand
                };

                layout.Children.Add(midLayout);

                layout.Children.Add(error);
                layout.Children.Add(nextButton);

                var passEntry = new Entry {
                    Placeholder = "Uw gewenste pincode hier...",
                    Keyboard = Keyboard.Numeric
                };

                titleLabel.Text = "Pincode instellen";
                explainLabel.Text = "Vul hieronder a.u.b. een makkelijk te onthouden code van 4 cijfers in om de gegevens binnen de app persoonlijk te houden.";
                midLayout.Children.Add(
                    passEntry
                );

                string passText = null;
                error.Text = "Vul a.u.b. een geldige pincode in.";
                passEntry.TextChanged += (sender, e) => {
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
                Preferences.Set(coach.UserProfile.ID + "_password", password);

                await Navigation.PopAsync();
			}
		}

        class EmailPage : ContentPage
        {
            public EmailPage(Coach coach)
            {
                Show(coach);
            }

            async void Show(Coach coach)
            {
                StackLayout layout;

                Content = new ScrollView {
                    Content = layout = new StackLayout {
                        Spacing = StyleKit.AutoSpacing.Large,
                        Padding = StyleKit.AutoPadding,
                        VerticalOptions = LayoutOptions.StartAndExpand
                    }
                };

                var titleLabel = new Label {
                    Style = StyleKit.AutoDarkLabelStyles.Display
                };

                var explainLabel = new Label {
                    Style = StyleKit.AutoDarkLabelStyles.Body
                };

                var topLayout = new StackLayout {
                    Spacing = StyleKit.AutoSpacing.Medium,
                    VerticalOptions = LayoutOptions.StartAndExpand,
                    Children = {
                        titleLabel,
                        explainLabel
                    }
                };

                layout.Children.Add(topLayout);

                var error = new Label {
                    HorizontalTextAlignment = TextAlignment.Center,
                    Style = StyleKit.AutoDarkLabelStyles.Caption,
                    TextColor = Color.Red,
                    IsVisible = false
                };

                var nextButton = new Button {
                    Text = "Opslaan",
                    TextColor = Color.White,
                    Font = Font.SystemFontOfSize(16),
                    BackgroundColor = Color.FromHex("fa5755"),
                    VerticalOptions = LayoutOptions.EndAndExpand
                };

                var midLayout = new StackLayout {
                    Spacing = StyleKit.AutoSpacing.Medium,
                    VerticalOptions = LayoutOptions.StartAndExpand
                };

                layout.Children.Add(midLayout);

                layout.Children.Add(error);
                layout.Children.Add(nextButton);

                var mailEntry = new Entry {
                    Placeholder = "Uw e-mailadres hier...",
                    Keyboard = Keyboard.Email,
                    Text = Preferences.GetOr<string>(coach.UserProfile.ID + "_email", null)
                };

                titleLabel.Text = "E-mail instellen";
                explainLabel.Text = "Mocht je je pincode vergeten, willen wij je graag een nieuwe sturen zodat je de app kunt blijven gebruiken. Daarvoor hebben we jouw e-mailadres nodig.\nVul hieronder a.u.b. een e-mailadres in waar je toegang tot hebt, zodat je ook altijd toegang hebt tot de app.";
                midLayout.Children.Add(
                    mailEntry
                );

                string mailText = null;

                mailEntry.TextChanged += (sender, e) => {
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

                Preferences.Set(coach.UserProfile.ID + "_email", mailText);

                await Navigation.PopAsync();
            }
        }
	}
}

