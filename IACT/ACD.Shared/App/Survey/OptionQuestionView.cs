using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Xamarin.Forms;

namespace ACD.App
{
    public class OptionQuestionView : QuestionView<OptionQuestion>
    {
        List<Checkbox> options;

        public OptionQuestionView(int number, OptionQuestion question)
            : base(number, question)
        {
            var list = new StackLayout();
            options = new List<Checkbox>();

            foreach (var option in question.Options)
            {
                var cb = new Checkbox();

                var item = new StackLayout {
                    Orientation = StackOrientation.Horizontal,
                    Spacing = 15,
                    Children = {
                        new Frame { Content = cb, BackgroundColor = Color.White, Padding = 0 },
                        new Label {
                            Text = question.Options.IndexOf(option) + ". " + option
                        }
                    }
                };

                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += (s, e) => cb.Checked = !cb.Checked;
                item.GestureRecognizers.Add(tapGestureRecognizer);

                list.Children.Add(item);

                cb.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == Checkbox.CheckedProperty.PropertyName && cb.Checked)
                    {
                        foreach (var other in options.Where(c => c != cb))
                        {
                            other.Checked = false;
                        }
                    }
                };

                options.Add(cb);
            }

            AnswerView = list;
        }

        public override object Answer
        {
            get
            {
                return options.IndexOf(options.First(c => c.Checked));
            }
        }
    }
}
