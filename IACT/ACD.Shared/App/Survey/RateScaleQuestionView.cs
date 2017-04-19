using System;

using Xamarin.Forms;

namespace ACD.App
{
	public class RateScaleQuestionView : QuestionView<RateScaleQuestion>
	{
		Slider slider;
	    int min;

		public RateScaleQuestionView(int number, RateScaleQuestion question)
			: base(number, question)
		{
		    min = question.Min;

			slider = new Slider {
				Minimum = 0,
				Maximum = question.Max - min
			};

            var valueLabel = new Label {
                Font = Font.SystemFontOfSize(NamedSize.Large, FontAttributes.Bold),
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

			slider.ValueChanged += (sender, args) =>
			{
				if (!question.Gliding)
				{
				    double value = Math.Round(slider.Value);
				    if (value != slider.Value)
				        slider.Value = value;
				}

				valueLabel.Text = (slider.Value + min).ToString();
			};

		    slider.Value = (question.Min + question.Max) / 2.0 - min;

			AnswerView = new StackLayout {
				Children = {
					new StackLayout {
                        Orientation = StackOrientation.Horizontal,
                        //HorizontalOptions = LayoutOptions.FillAndExpand,
                        Children = {
                            new Label {
								Text = "Oneens",
                                HorizontalOptions = LayoutOptions.StartAndExpand
                            },
                            new Label {
								Text = "Eens"
                            }
                        }
                    },
					slider
				}
			};
		}

		public override object Answer {
			get
			{
				return slider.Value + min;
			}
		}
	}
}

