using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Color = Xamarin.Forms.Color;
using TextAlignment = Xamarin.Forms.TextAlignment;
using Rectangle = Xamarin.Forms.Rectangle;

using NControl.Abstractions;
using NGraphics;

namespace ACD.App
{
    public class GraphPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromRgb(242, 183, 57);
        public static readonly Color BarTextColor = Color.White;

        Coach coach;
        AbsoluteLayout container;
        StackLayout layout;
        GraphList list;
        Label yearLabel, explanationLabel;
        NControlView listContainer;
        IImage[] images;

        readonly double imageSize = Math.Min(StyleKit.AutoPaddingLight.Left, 42);

        public GraphPage(Coach coach, Monitor monitor, string name, string explanation, Action<DayFragment> onTap = null)
        {
            this.coach = coach;

            Title = name;

            list = new GraphList()
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.Transparent
            };

            list.ItemTapped += (sender, e) => onTap?.Invoke(e.Item as DayFragment);

            yearLabel = new Label
            {
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.EndAndExpand,
                Style = StyleKit.AutoDarkLabelStyles.Body,
                BindingContext = list
            };

            explanationLabel = new Label
            {
                Text = explanation,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalOptions = LayoutOptions.StartAndExpand,
                Style = StyleKit.AutoDarkLabelStyles.Caption
            };

            Content = new ScrollView
            {
                Content = container = new AbsoluteLayout
                {
                    Children = {
                        (layout = new StackLayout {
                            Padding = StyleKit.AutoPaddingLight,
                            Spacing = StyleKit.AutoSpacing.Small,
                            Children = {
                                yearLabel,
                                (listContainer = new NControlView {
                                    Content = new StackLayout {
                                        //BackgroundColor = MainApp.RandomColor(),
                                        Orientation = StackOrientation.Horizontal,
                                        VerticalOptions = LayoutOptions.Center,
                                        Padding = new Thickness(imageSize + 2, 0, 0, 0),
                                        Children = {
                                            /* (axis = new StackLayout {
                                                Orientation = StackOrientation.Vertical,
                                                Children = {
                                                    new Image {
                                                        Source = ImageSource.FromFile("face-smile.png"),
                                                        VerticalOptions = LayoutOptions.StartAndExpand
                                                    },
                                                    new Image {
                                                        Source = ImageSource.FromFile("face-sad.png")
                                                    }
                                                }
                                            }), */
                                            list
                                        }
                                    },
                                    DrawingFunction = (canvas, rect) => {
                                        var left = imageSize;
                                        var top = listHeightShrunkAfterTranslation;
                                        var bottom = rect.Height - listHeightShrunkAfterTranslation  - GraphCell.SpaceUnderGraph - 5;
                                        var right = rect.Width;
                                        canvas.DrawLine(left, top, left, bottom, Colors.Black, 2.0);
                                        canvas.DrawLine(left, bottom, right, bottom, Colors.Black, 2.0);
                                        if (images != null)
                                        {

                                            var padding = imageSize * 0.2;
                                            //TODO: Find solution to images missing
                                            canvas.DrawImage(images[0], 0, top, imageSize - padding, imageSize - padding);
                                            canvas.DrawImage(images[1], 0, bottom - imageSize + padding, imageSize - padding, imageSize - padding);
                                        }
                                    }
                                }),
                                explanationLabel
                            }
                        })
                    }
                }
            };

            listContainer.Invalidate();

            AbsoluteLayout.SetLayoutFlags(layout, AbsoluteLayoutFlags.WidthProportional | AbsoluteLayoutFlags.YProportional);
            AbsoluteLayout.SetLayoutBounds(layout, new Rectangle(0, 0.5, 1, AbsoluteLayout.AutoSize));

            SizeChanged += (sender, e) => SetListSize();
            SetListSize();

            list.ItemsSource = new DayFragmentCollection(monitor.GetEvents());

            yearLabel.SetBinding(Label.TextProperty, (GraphList gl) => gl.MedianDay, (DateTime day) => day.ToString("MMMM yyy"));

            GetImages();
        }

        async void GetImages()
        {
            images = new IImage[] {
                #if __ANDROID__
                await new FileImageSource { File = "face_smile.png" }.ToIImage(),
                await new FileImageSource { File = "face_sad.png" }.ToIImage(),
                #endif

                await new FileImageSource { File = "face-smile.png" }.ToIImage(),
                await new FileImageSource { File = "face-sad.png" }.ToIImage(),
            };
            listContainer.Invalidate();
        }

        double listHeightShrunkAfterTranslation = 0;
        void SetListSize()
        {
            var width = Width - layout.Padding.HorizontalThickness - imageSize;
            var height = Math.Min(GraphList.MaxHWRatio * width, Height - layout.Padding.VerticalThickness - yearLabel.Height - layout.Spacing);
            list.WidthRequest = height;
            list.HeightRequest = width;
            Device.BeginInvokeOnMainThread(() => {
                /* However, width and height get reversed only later by the rotation:
                 * therefore the other elements need to be translated and some
                 * cropping needs to be applied. */
                var translation = (list.Height - list.Width) / 2;
                yearLabel.TranslationY = translation;
                explanationLabel.TranslationY = -translation;
                listHeightShrunkAfterTranslation = translation;
                listContainer.Invalidate();
                container.HeightRequest = layout.Height - 2*translation;
            });
        }

        protected override async void OnAppearing()
        {
            listContainer.Invalidate();

            await coach.PerformChecks(true);

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            list.Reset();
        }
    }

    class GraphList : ListView
    {
        public static readonly double MaxHWRatio = 0.8;
        public static readonly double MinDaysOnScreen = 4.5;

        public static readonly BindableProperty MedianDayProperty =
            BindableProperty.Create<GraphList, DateTime>(c => c.MedianDay, DateTime.Today);

        public GraphList()
            : base(ListViewCachingStrategy.RecycleElement)
        {
            ItemTemplate = new DataTemplate(typeof(GraphCell));
            ItemTemplate.SetBinding(GraphCell.DataProperty, ".");

            Rotation = 90;
            //BackgroundColor = MainApp.RandomColor();
            SeparatorVisibility = SeparatorVisibility.None;
            SeparatorColor = Color.White;

            PropertyChanged += (sender, e) => {
                if (e.PropertyName == HeightProperty.PropertyName)
                {
                    var whRatio = Math.Sqrt(Width / Height / MaxHWRatio);
                    RowHeight = (int)(whRatio * Height / MinDaysOnScreen);
                }
                if (e.PropertyName == ItemsSourceProperty.PropertyName)
                {
                    DataChange(ItemsSource.Cast<DayFragment>());
                }
            };

            ItemTapped += (sender, e) => {
                if (e.Item == null) return;
                (sender as ListView).SelectedItem = null;
            };

            ItemAppearing += VisibilityChange(true);
            ItemDisappearing += VisibilityChange(false);
        }

        SortedSet<DateTime> visibleDays = new SortedSet<DateTime>();

        private void DataChange(IEnumerable<DayFragment> data)
        {
            visibleDays.Clear();
            if (data.Any())
                visibleDays.Add(data.First().Day);
            SetMedianDay();
        }
        
        private EventHandler<ItemVisibilityEventArgs> VisibilityChange(bool visible)
        {
            return new EventHandler<ItemVisibilityEventArgs>((sender, args) => {
                if (args.Item == null) return;

                var day = (args.Item as DayFragment).Day;
                if (visible) visibleDays.Add(day);
                else visibleDays.Remove(day);

                SetMedianDay();
            });
        }

        private void SetMedianDay()
        {
            if (visibleDays.Any())
                MedianDay = visibleDays.First() + (visibleDays.Last() - visibleDays.First()).Divide(2);
        }

        public DateTime MedianDay
        {
            get { return (DateTime)GetValue(MedianDayProperty); }
            private set { SetValue(MedianDayProperty, value); }
        }

        public void Reset()
        {
            var data = ItemsSource.OfType<DayFragment>();
            if (data.Any())
                ScrollTo(data.First(), ScrollToPosition.Start, false);
            DataChange(data);
        }
    }
}

