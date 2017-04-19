using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class Graph : View
    {
        public new float[] X { get; set; }
        public new float[] Y { get; set; }
        public float XAxis { get; set; }
        public float YAxis { get; set; }
        public Thickness Padding { get; set; }

        public Graph()
        {
            Padding = new Thickness(0);
            /* RowDefinitions = new RowDefinitionCollection {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            };
            ColumnDefinitions = new ColumnDefinitionCollection {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            };

            var xLabelLayout = new AbsoluteLayout();
            foreach (var label in xLabels)
            {
                var xValue = label.Key;
                var view = label.Value;
                xLabelLayout.Children.Add(view,
                    new Rectangle(xValue / xAxis, 0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize),
                    AbsoluteLayoutFlags.PositionProportional);
            }

            var yLabelLayout = new AbsoluteLayout();
            foreach (var label in yLabels)
            {
                var yValue = label.Key;
                var view = label.Value;
                yLabelLayout.Children.Add(view,
                    new Rectangle(0, 1 - yValue / yAxis, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize),
                    AbsoluteLayoutFlags.PositionProportional);
            }

            Children.Add(yLabelLayout, 0, 0); // Left, First element
            Children.Add(new Label { Text = "text is text is text is\ntext is text is text is\ntext is text is text is\n" }, 1, 0); // Right, First element
            Children.Add(xLabelLayout, 1, 1); // Right, Second element */
        }
    }
}
