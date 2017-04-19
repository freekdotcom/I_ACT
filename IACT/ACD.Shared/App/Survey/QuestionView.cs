using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ACD.App
{
	public abstract partial class QuestionView : Frame
	{
        StackLayout layout;
		View answerView;

		protected QuestionView(int number, Question question)
		{
			BackgroundColor = Color.FromRgb(211, 211, 211);
			OutlineColor = Color.FromRgb(103, 59, 102);

			Content = layout = new StackLayout {
				VerticalOptions = LayoutOptions.Center,
				Spacing = 20,
				//Padding = new Thickness(20, 20),
				Children = {
					new Label {
						Text = number + ". " + question.Text,
						XAlign = TextAlignment.Center,
						HorizontalOptions = LayoutOptions.FillAndExpand,
						Font = Font.SystemFontOfSize(13)
					}
				}
			};

            /* Content = new Frame {
                BackgroundColor = Color.FromRgb(211, 211, 211),
                OutlineColor = Color.FromRgb(103, 59, 102),
                //Padding = 0,
                Content = layout = new StackLayout {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 20,
                    //Padding = new Thickness(20, 20),
                    Children = {
                        new StackLayout {
                            Orientation = StackOrientation.Horizontal,
                            Children = {
                                new Label {
				                    Text = number + ".",
				                    Font = Font.SystemFontOfSize(13)
			                    },
                                new Label {
				                    Text = question.Text,
				                    Font = Font.SystemFontOfSize(13)
			                    }
                            }
                        }
                    }
                }
            };*/
		}

		protected View AnswerView
		{
			get { return answerView; }
			set
			{
				if (answerView != null)
					layout.Children.Remove(answerView);
				layout.Children.Add(value);
				answerView = value;
			}
		}

		public abstract object Answer { get; }
	}

	public abstract class QuestionView<T> : QuestionView
		where T : Question
	{
		protected T Question { get; private set; }

		protected QuestionView(int number, T question) : base(number, question)
		{
			Question = question;
		}
	}
}

