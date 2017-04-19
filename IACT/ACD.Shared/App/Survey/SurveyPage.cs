using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class SurveyPage : ContentPage
    {
        Layout<View> layout;
        Coach coach;
        bool surveyInProgress = false;
        Survey survey;

        public SurveyPage(Coach coach, Survey survey)
        {
            this.coach = coach;
            this.survey = survey;

            Title = "Feedback";
            Content = layout = new StackLayout {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            StartSurvey();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await coach.Authenticate(Navigation);
        }

        private async void StartSurvey()
        {
            var titleLabel = new Label {
                Text = survey.Title,
                Font = Font.SystemFontOfSize(17, FontAttributes.Bold),
                XAlign = TextAlignment.Center
            };

            var topLayout = new StackLayout {
                Spacing = 25,
                Padding = new Thickness(10, 0),
                Children = {
                    titleLabel,
                    new Label {
                        Text = survey.Description,
						Font = Font.SystemFontOfSize(12)
                    }
                }
            };

            var nextButton = new ExtButton {
                Text = survey.Introduction != null && survey.Introduction.Count > 0 ? "Volgende" : "Starten",
				TextColor = Color.White,
				BackgroundColor = Color.FromRgb(103, 59, 102)
            };

            var currentContent = new StackLayout {
                Spacing = 20,
                Padding = new Thickness(15, 30),
				VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    topLayout,
					nextButton
				}
            };

            layout.Children.Replace(currentContent);

            await nextButton.GetEventAsync<EventArgs>("Clicked");

            Title = survey.Title;

            var introAnswers = new List<object>();

            int questionCounter = 0;
            for (int i = 0; survey.Introduction != null && i < survey.Introduction.Count; i++)
            {
                topLayout.Children.Replace(
                    new Label {
                        Text = survey.Introduction[i].Title,
                        Font = Font.SystemFontOfSize(15, FontAttributes.Bold),
                        XAlign = TextAlignment.Center
                    },
                    new Label {
                        Text = survey.Introduction[i].Text,
                        Font = Font.SystemFontOfSize(12)
                    }
                );

                QuestionView qv = null;
                var question = survey.Introduction[i].ExampleQuestion;
                if (question != null)
                {
                    qv = QuestionView.Create(++questionCounter, question);
                    qv.VerticalOptions = LayoutOptions.Center;
                    topLayout.Children.Add(qv);
                }

                if (i == survey.Introduction.Count - 1)
                    nextButton.Text = "Starten";

                await nextButton.GetEventAsync<EventArgs>("Clicked");

                if (survey.Introduction[i].SaveAnswer && qv != null)
                    introAnswers.Add(qv.Answer);
            }

            surveyInProgress = true;

            topLayout.Children.Clear();
            nextButton.Text = "Volgende";

            var time = DateTime.Now;
            var answers = new List<object>(survey.Questions.Count);

            foreach (var question in survey.Questions)
            {
                var qv = QuestionView.Create(survey.Questions.IndexOf(question) + 1, question);
                qv.VerticalOptions = LayoutOptions.Center;

                topLayout.Children.Add(qv);

                await nextButton.GetEventAsync<EventArgs>("Clicked");

                answers.Add(qv.Answer);

                topLayout.Children.Clear();
            }

            string remarks = null;
            if (survey.Remarks)
            {
                var editor = new Editor {
                    
                };

                topLayout.Children.Replace(
                    new Label {
                        Text = "Hieronder kunt u nog overige aanvullende opmerkingen invullen, als u die heeft:",
                        Font = Font.SystemFontOfSize(12)
                    },
                    editor
                );

                await nextButton.GetEventAsync<EventArgs>("Clicked");

                remarks = editor.Text;
            }

            var response = new SurveyResponse {
                SurveyID = survey.ID,
                Answers = answers,
                Time = time
            };

            if (introAnswers.Count > 0)
                response.IntroAnswers = introAnswers;
            if (!string.IsNullOrWhiteSpace(remarks))
                response.Remarks = remarks;

            coach.UserProfile.AddSurveyResponse(response);

            var layout2 = new StackLayout {
                Spacing = 35,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Children = {
                    new Label {
                        Text = "U bent nu klaar met het invullen!",
                        Font = Font.SystemFontOfSize(16, FontAttributes.Bold),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        XAlign = TextAlignment.Center
                    },
                    new Label {
                        Text = "Kom later nog eens terug om uw stemming in de gaten te houden.",
                        Font = Font.SystemFontOfSize(14),
                        HorizontalOptions = LayoutOptions.Center,
                        XAlign = TextAlignment.Center
                    }
                }
            };

            var statsButton = new ExtButton {
                Text = "Voortgang bekijken",
                TextColor = Color.White,
                BackgroundColor = Color.FromRgb(103, 59, 102)
            };
            statsButton.Clicked += (sender, args) =>
            {
                Navigation.PopAsync();
                MessagingCenter.Send<object, int>(this, CoachPage.NavigateMessage, 2);
            };

            currentContent.Children.Replace(new StackLayout {
                Spacing = 40,
                Padding = new Thickness(20, 50, 20, 0),
                VerticalOptions = LayoutOptions.CenterAndExpand,
                Children = {
                    /* new Image {
						Source = ImageSource.FromResource(App.GetResourceName("list.png")),
                        HorizontalOptions = LayoutOptions.Center
                    }, */
                    layout2,
					statsButton
				}
            });

            surveyInProgress = false;
            Title = "Feedback";
        }
    }
}
