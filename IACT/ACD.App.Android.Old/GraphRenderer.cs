using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Android.Content;
using Android.Graphics;
using Color = Android.Graphics.Color;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using ACD.App;
using ACD.App.Droid;

[assembly: ExportRenderer(typeof(Graph), typeof(GraphRenderer))]

namespace ACD.App.Droid
{
    public class GraphView : Android.Views.View
    {
        public Graph Graph { get; set; }

        Paint paint = new Paint {
            Color = Color.Black
        };

        public GraphView(Context context, Graph graph)
            : base(context)
        {
            Graph = graph;

            paint.SetStyle(Paint.Style.Stroke);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            double paddedWidth = Width - Graph.Padding.HorizontalThickness,
                   paddedHeight = Height - Graph.Padding.VerticalThickness;

            paint.AntiAlias = true;

            //canvas.DrawColor(Color.Aqua);

            paint.StrokeWidth = 2;
            paint.SetPathEffect(new DashPathEffect(new float[] { 15, 10 }, 0));

            for (int i = 0; i <= 5; i++)
            {
                canvas.DrawLine(0, 1 + i * (Height - 2) / 5, Width, 1 + i * (Height - 2) / 5, paint);
            }

            if (Graph.X.Length > 0 && Graph.Y.Length > 0)
            {
                paint.StrokeWidth = 6;
                paint.SetPathEffect(null);

                canvas.Translate((float)Graph.Padding.Left, (float)Graph.Padding.Top);
                canvas.Scale((float)paddedWidth / Width, (float)paddedHeight / Height);

                var path = new Path();
                path.MoveTo(Width * Graph.X[0] / Graph.XAxis, Height - Height * Graph.Y[0] / Graph.YAxis);
                for (int i = 0; i < Graph.X.Length; i++)
                {
                    path.LineTo(Width * Graph.X[i] / Graph.XAxis, Height - Height * Graph.Y[i] / Graph.YAxis);
                    path.AddCircle(Width * Graph.X[i] / Graph.XAxis, Height - Height * Graph.Y[i] / Graph.YAxis, 3, Path.Direction.Cw);
                }
                canvas.DrawPath(path, paint);
            }
        }
    }

    public class GraphRenderer : ViewRenderer<Graph, GraphView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Graph> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            var view = new GraphView(Context, Element);
            SetNativeControl(view);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == Checkbox.CheckedProperty.PropertyName)
            {
                
            }
        }
    }
}