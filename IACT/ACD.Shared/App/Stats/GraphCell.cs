using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using TextAlignment = Xamarin.Forms.TextAlignment;

using NControl.Abstractions;
using NGraphics;
using NPoint = NGraphics.Point;
using NRect = NGraphics.Rect;
using NSize = NGraphics.Size;
using NFont = NGraphics.Font;

namespace ACD.App
{
    class GraphCell : ViewCell
    {
        static readonly double CaptionSize = (double)StyleKit.AutoDarkLabelStyles.Caption
            .Setters.First(s => s.Property == Label.FontSizeProperty).Value;
        public static readonly double SpaceUnderGraph = CaptionSize * 4;
        public static readonly BindableProperty DataProperty =
            BindableProperty.Create<GraphCell, DayFragment>(c => c.Data, null);

        NControlView dataView;

        public DayFragment Data
        {
            get { return (DayFragment)GetValue(DataProperty); }
            set
            {
                SetValue(DataProperty, value);
                dataView.Invalidate();
            }
        }

        DayFragment prevFragment = null;
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            dataView.Invalidate();
            Data.PropertyChanged += InvalidateOnDataChange;
            Data.Overview.CollectionChanged += InvalidateOnDataChange;
            if (prevFragment != null && prevFragment != Data)
                prevFragment.PropertyChanged -= InvalidateOnDataChange;
            prevFragment = Data;
        }

        void InvalidateOnDataChange<T>(object sender, T e)
        {
            dataView.Invalidate();
        }

        NPoint CalculateEventPoint(IReadOnlyList<MonitorEvent> events, int index, NRect frame)
        {
            var x = (index + 1) * frame.Width / (events.Count + 1);
            var y = (1 - events[index].Value) * frame.Height;
            return new NPoint(x, y);
        }

        IEnumerable<NPoint> CalculateEventPoints(IReadOnlyList<MonitorEvent> events, NRect frame, int offset = 0)
        {
            return events != null ? events.Select((ev, i) => CalculateEventPoint(events, i, frame))
                .Select(p => p.WithX(p.X + offset * frame.Width)) : new List<NPoint>();
        }

        public GraphCell() : base()
        {
            StackLayout dateLayout;
            Label dateLabel;

            /* Used when drawing. */
            var points = new List<NPoint>();
            var pointSize = new NSize(CaptionSize * 5/6);
            var halfPoint = pointSize / 2;

            View = dataView = new NControlView {
                Content = new StackLayout {
                    //BackgroundColor = MainApp.RandomColor(),
                    Orientation = StackOrientation.Horizontal,
                    Children = {
                        (dateLayout = new StackLayout {
                            HorizontalOptions = LayoutOptions.EndAndExpand,
                            //Padding = new Thickness(7, 0),
                            WidthRequest = SpaceUnderGraph,
                            Children = {
                                (dateLabel = new Label {
                                    HorizontalTextAlignment = TextAlignment.Center,
                                    VerticalOptions = LayoutOptions.CenterAndExpand,
                                    Rotation = -90,
                                    Style = StyleKit.AutoDarkLabelStyles.Caption,
                                    //FontSize = 12,
                                    BindingContext = this
                                })
                            }
                        })
                    }
                },
                DrawingFunction = (canvas, rect) => {
                    /* Transforms for natural drawing - as in the end, the whole graph is rotated 90 degrees! */
                    canvas.Rotate(-90);
                    canvas.Translate(-rect.Height, halfPoint.Height);
                    rect = new NRect(NPoint.Zero,
                        new NSize(rect.Size.Height, rect.Size.Width - dateLayout.Width - pointSize.Height));

                    //canvas.DrawLine(rect.BottomLeft, rect.BottomRight, Colors.Black, 2);

                    if (Data == null)
                        return;

                    points.Clear();

                    /* Add past points */
                    var yesterday = Data.Yesterday;
                    if (yesterday?.Events?.Count == 1)
                    {
                        var beforeYesterday = yesterday?.Yesterday;
                        points.AddRange(CalculateEventPoints(beforeYesterday?.Events, rect, -2));
                    }
                    points.AddRange(CalculateEventPoints(yesterday?.Events, rect, -1));
                    /* Add today's points */
                    points.AddRange(CalculateEventPoints(Data.Events, rect));
                    /* Add future points */
                    var tomorrow = Data.Tomorrow;
                    points.AddRange(CalculateEventPoints(tomorrow?.Events, rect, 1));
                    if (tomorrow?.Events?.Count == 1)
                    {
                        var afterTomorrow = tomorrow?.Tomorrow;
                        points.AddRange(CalculateEventPoints(afterTomorrow?.Events, rect, 2));
                    }

                    if (Data.Events.Count > 0)
                    {
                        var path = new Path(pen: new Pen(Colors.Black, pointSize.Width / 4));
                        path.MoveTo(points[0]);
                        for (int i = 1; i < points.Count; i++)
                        {
                            path.Operations.Add(SplineTo(
                                from: points[i - 1],
                                to: points[i],
                                beforeFrom: i >= 2
                                    ? points[i - 2]
                                    : points[i - 1] - (points[i] - points[i - 1]).WithY(0),
                                afterTo: i <= points.Count - 2
                                    ? points[i + 1]
                                    : points[i] + (points[i] - points[i - 1]).WithY(0)
                            ));
                        }
                        path.Draw(canvas);
                    }

                    foreach (var p in points)
                    {
                        if (p.X >= -halfPoint.Width && p.X <= rect.Width + halfPoint.Width)
                        {
                            canvas.FillEllipse(p - halfPoint, pointSize, Colors.Black);
                            canvas.FillEllipse(p - halfPoint * 4 / 5, pointSize * 4 / 5, Colors.White);
                        }
                    }
                    
                    /* canvas.DrawText(Math.Round(data.Value * 5 + 1, 1).ToString("F1"),
                            rect.Center, new NFont(), Colors.Black); */
                }
            };

            View.SizeChanged += (sender, e) => dataView.Invalidate();

            dateLabel.SetBinding<GraphCell, DateTime, string>(Label.TextProperty, cell => cell.Data.Day, dt => dt.ToString("ddd\ndd/MM"));
            dateLabel.SizeChanged += (sender, e) => dataView.Invalidate();
        }

        /* From: http://scaledinnovation.com/analytics/splines/aboutSplines.html
         function getControlPoints(x0,y0,x1,y1,x2,y2,t){
            var d01=Math.sqrt(Math.pow(x1-x0,2)+Math.pow(y1-y0,2));
            var d12=Math.sqrt(Math.pow(x2-x1,2)+Math.pow(y2-y1,2));
            var fa=t*d01/(d01+d12);   // scaling factor for triangle Ta
            var fb=t*d12/(d01+d12);   // ditto for Tb, simplifies to fb=t-fa
            var p1x=x1-fa*(x2-x0);    // x2-x0 is the width of triangle T
            var p1y=y1-fa*(y2-y0);    // y2-y0 is the height of T
            var p2x=x1+fb*(x2-x0);
            var p2y=y1+fb*(y2-y0);  
            return [p1x,p1y,p2x,p2y];
         } */

        static PathOp SplineTo(NPoint from, NPoint to, NPoint beforeFrom, NPoint afterTo, double tension = 0.33)
        {
            var d01 = beforeFrom.DistanceTo(from);
            var d12 = from.DistanceTo(to);
            var d23 = to.DistanceTo(afterTo);
            var f1 = tension * d12 / (d01 + d12);
            var f2 = tension * d12 / (d12 + d23);
            return new NGraphics.CurveTo(
                from + f1 * (to - beforeFrom),
                to - f2 * (afterTo - from),
                to
            );
        }
    }
}

