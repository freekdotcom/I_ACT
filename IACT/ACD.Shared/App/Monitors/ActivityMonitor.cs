using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ACD.App
{
    public class ActivityMonitor : Monitor
    {
        public static readonly string Key = "activity";

        TimeSpan check;
        public TimeSpan Check
        {
            get
            {
                return check;
            }
            set
            {
                Times.Remove(check);
                check = value.MinutesOnly();
                Times.Add(check);
                Save();
            }
        }

        public ActivityMonitor(Profile profile) : base(profile, ActivityMonitor.Key)
        {
            check = Times.LastOrDefault();
        }

        protected override Notification MakeNotification(TimeSpan time)
        {
            var fullTime = DateTime.Today + time;
            if (fullTime <= DateTime.Now)
                fullTime = fullTime.AddDays(1);
            return new Notification {
                Title = "Dagevaluatie",
                Body = "Hoe was uw dag vandaag? Open de app om uw dag te evalueren.",
                Open = "0",
                Action = "beoordelen",
                Time = fullTime,
                Repeat = TimeSpan.FromDays(1)
            };
        }

        protected override async Task<double> GetValueFromUser()
        {
            var activitySlider = new Slider {
                Minimum = 0,
                Maximum = 1,
                Value = 0.5
            };

            var error = new Label {
                Text = "Sleep de meter naar links of rechts om aan te geven hoe tevreden je bent.",
                HorizontalTextAlignment = TextAlignment.Center,
                Style = StyleKit.AutoDarkLabelStyles.Caption,
                //FontSize = 12,
                TextColor = Color.Red,
                IsVisible = false
            };
                    
            activitySlider.ValueChanged += (sender, e) => {
                if (e.NewValue > 0.4 && e.NewValue < 0.6)
                {
                    if (e.OldValue <= 0.4 || e.OldValue >= 0.6)
                        activitySlider.Value = e.OldValue;
                    else
                        activitySlider.Value = e.NewValue < e.OldValue ? 0.4 : 0.6;
                }
                error.IsVisible = false;
            };

            await Alert.Show(
                "Hoe was je dag?",
                "Geef hieronder aan hoe tevreden je bent over de hoeveelheid activiteit die je vandaag ondernomen hebt.",
                new StackLayout {
                    Children = {
                        new StackLayout {
                            Orientation = StackOrientation.Horizontal,
                            //HorizontalOptions = LayoutOptions.FillAndExpand,
                            Children = {
                                new Image {
                                    Source = ImageSource.FromFile("face-sad.png"),
                                    HorizontalOptions = LayoutOptions.StartAndExpand
                                },
                                new Image {
                                    Source = ImageSource.FromFile("face-smile.png")
                                }
                            }
                        },
                        activitySlider,
                        error
                    }
                },
                new AlertButton {
                    Text = "Opslaan",
                    IsPreferred = true,
                    Action = () => {
                        if (activitySlider.Value != 0.5)
                            return false;
                        error.IsVisible = true;
                        error.IsVisible = false;
                        error.IsVisible = true;
                        return true;
                    }
                }
            );

            bool later = false;

            await Alert.Show(
                "Dagboekje",
                "Heb je nog opmerkingen over je activiteiten vandaag? Die kan je in je dagboekje vastleggen.",
                null,
                new AlertButton {
                    Text = "Later",
                    Action = () => {
                        later = true;
                        return false;
                    }
                },
                new AlertButton {
                    Text = "Naar dagboekje",
                    IsPreferred = true,
                    Action = () => {
                        MessagingCenter.Send(this as object, CoachPage.NavigateMessage, CoachPage.Subpages.StatsPage);
                        MessagingCenter.Send(this as object, StatsPage.Visit, StatsPage.Subpages.DiaryPage);
                        return false;
                    }
                }
            );

            if (later)
                await Alert.Show("Dagboekje", "Je kunt op een later moment altijd nog iets in je dagboekje schrijven. Deze mogelijkheid vind je onder \"Voortgang\".");

            return activitySlider.Value;
        }
    }
}

