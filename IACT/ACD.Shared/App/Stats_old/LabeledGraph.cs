using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
	public class LabeledGraph : AbsoluteLayout
    {
        public LabeledGraph(float[] x, float[] y, float xAxis, float yAxis, Dictionary<float, View> xLabels, Dictionary<float, View> yLabels)
        {
            int w = 30,
                h = 30;

			/* var leftLayout = new AbsoluteLayout();
			var rightLayout = new AbsoluteLayout();

			Orientation = StackOrientation.Horizontal;
			Children.Replace(
				leftLayout,
				new ScrollView {
					Orientation = ScrollOrientation.Horizontal,
					Content = rightLayout
				}
			); */

            var graph = new Graph {
                X = x,
                Y = y,
                XAxis = xAxis,
                YAxis = yAxis,
				Padding = new Thickness(Device.OnPlatform(w/4, w/2, w/2), h/2)//xLabels.Values.First().Width / 2, yLabels.Values.First().Width / 2, xLabels.Values.Last().Width / 2, yLabels.Values.Last().Width / 2)
            };

			Children.Add(graph, new Rectangle(-w/2 + w, 0 + h/2, 200, 200));

            foreach (var label in yLabels)
            {
                var yValue = label.Key;
                var view = label.Value;
                Children.Add(view,
					new Rectangle(-w/2 - 5, 200 - (200) * yValue / yAxis, w, h));
            }

            foreach (var label in xLabels)
            {
                var xValue = label.Key;
                var view = label.Value;
                Children.Add(view,
					new Rectangle(-w/2 + 3*w/4 + (200 - w/2) * xValue / xAxis, 210 + h/2, w, h));
            }

			/*rightLayout.Children.Add(graph, new Rectangle(0, 0 + h/2, 200, 200));

			foreach (var label in yLabels)
			{
				var yValue = label.Key;
				var view = label.Value;
				leftLayout.Children.Add(view,
					new Rectangle(-w/2 - 5, 200 - (200) * yValue / yAxis, w, h));
			}

			foreach (var label in xLabels)
			{
				var xValue = label.Key;
				var view = label.Value;
				rightLayout.Children.Add(view,
					new Rectangle(-w/5 + (200 - w/2) * xValue / xAxis, 210 + h/2, w, h));
			}*/
        }
    }
}
