using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ACD.App
{
    public class MoodMonitor : Monitor
    {
        public static readonly string Key = "mood";

        TimeSpan morningCheck;
        public TimeSpan MorningCheck
        {
            get
            {
                return morningCheck;
            }
            set
            {
                Times.Remove(morningCheck);
                morningCheck = value.MinutesOnly();
                Times.Add(morningCheck);
                Save();
            }
        }

        TimeSpan eveningCheck;
        public TimeSpan EveningCheck
        {
            get
            {
                return eveningCheck;
            }
            set
            {
                Times.Remove(eveningCheck);
                eveningCheck = value.MinutesOnly();
                Times.Add(eveningCheck);
                Save();
            }
        }

        public MoodMonitor(Profile profile) : base(profile, MoodMonitor.Key)
        {
            morningCheck = Times.FirstOrDefault();
            eveningCheck = Times.LastOrDefault();
        }

        protected override Notification MakeNotification(TimeSpan time)
        {
            var fullTime = DateTime.Today + time;
            if (fullTime <= DateTime.Now)
                fullTime = fullTime.AddDays(1);
            return new Notification {
                Title = "Stemmingscheck",
                Body = "Hoe gaat het met je? Open de app om je stemming door te geven.",
                Open = "0",
                Action = "beoordelen",
                Time = fullTime,
                Repeat = TimeSpan.FromDays(1)
            };
        }

        protected override async Task<double> GetValueFromUser()
        {
            var moodSlider = new Slider {
                Minimum = 0,
                Maximum = 1,
                Value = 0.5
            };

            var error = new Label {
                Text = "Sleep de meter naar links of rechts om aan te geven hoe je je voelt.",
                HorizontalTextAlignment = TextAlignment.Center,
                Style = StyleKit.AutoDarkLabelStyles.Caption,
                //FontSize = 12,
                TextColor = Color.Red,
                IsVisible = false
            };

            moodSlider.ValueChanged += (sender, e) => {
                if (e.NewValue > 0.4 && e.NewValue < 0.6)
                {
                    if (e.OldValue <= 0.4 || e.OldValue >= 0.6)
                        moodSlider.Value = e.OldValue;
                    else
                        moodSlider.Value = e.NewValue < e.OldValue ? 0.4 : 0.6;
                }
                error.IsVisible = false;
            };

            AbsoluteLayout.SetLayoutFlags(moodSlider, AbsoluteLayoutFlags.SizeProportional);
            AbsoluteLayout.SetLayoutBounds(moodSlider, new Rectangle(0, 0, 1, 1));

            var block = new BoxView {
                BackgroundColor = Color.Red
            };

            AbsoluteLayout.SetLayoutFlags(block, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(block, new Rectangle(0.495, 0.5, 14, 2));

            await Alert.Show(
                "Hoe gaat het?",
                "Geef met de slider hieronder uw huidige stemming aan.",
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
                    new AbsoluteLayout {
                        Children = {
                            block, // TODO: better way to indicate neutral zone
                            moodSlider
                        }
                    },
                        error
                    }
                },
                new AlertButton {
                    Text = "Opslaan",
                    IsPreferred = true,
                    Action = () => {
                        if (moodSlider.Value != 0.5)
                            return false;
                        error.IsVisible = true;
                        error.IsVisible = false;
                        error.IsVisible = true;
                        return true;
                    }
                }
            );

            await Alert.Show(
                "Dank je wel!",
                "Regelmatig invullen zorgt er voor dat de app je beter kan helpen. Kijk ook af en toe even naar het verloop van je stemming de laatste tijd.",
                null,
                new AlertButton {
                    Text = "Nee, dank je",
                    Action = () => {
                        return false;
                    }
                },
                new AlertButton {
                    Text = "Naar overzicht",
                    IsPreferred = true,
                    Action = () => {
                        MessagingCenter.Send(this as object, CoachPage.NavigateMessage, CoachPage.Subpages.StatsPage);
                        MessagingCenter.Send(this as object, StatsPage.Visit, StatsPage.Subpages.MoodGraph);
                        return false;
                    }
                }
            );

            return moodSlider.Value;
        }
    }
}

