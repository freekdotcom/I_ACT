using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ACD.App.iOS;

[assembly: Dependency(typeof(iOSAlert))]

namespace ACD.App.iOS
{
    public class iOSAlert : IAlert
    {
        public static readonly int AlertWidth = Device.Idiom == TargetIdiom.Phone ? 270 : 320;

        public async Task Show(string title, string body, View content, List<AlertButton> buttons)
        {
            if (buttons == null || buttons.Count == 0)
            {
                buttons = new List<AlertButton> {
                    new AlertButton {
                        Text = "Oké",
                        IsPreferred = true,
                        Action = () => false
                    }
                };
            }

            Func<Task> dismiss = null;

            var captionSize = (double)StyleKit.PhoneDarkLabelStyles.Caption.Setters.First(s => s.Property == Label.FontSizeProperty).Value;
            var titleSize = (double)StyleKit.PhoneDarkLabelStyles.Title.Setters.First(s => s.Property == Label.FontSizeProperty).Value;

            var top = new StackLayout {
                Padding = new Thickness(15, 20, 15, 20),
                Spacing = 3,
                Children = {
                    new Label {
                        Text = title,
                        Style = StyleKit.PhoneDarkLabelStyles.Title,
                        FontSize = Math.Max(16, titleSize),
                        HorizontalTextAlignment = TextAlignment.Center
                    },
                    new Label {
                        Text = body,
                        Style = StyleKit.PhoneDarkLabelStyles.Body,
                        //FontSize = ,
                        FontSize = Math.Max(14, captionSize),
                        HorizontalTextAlignment = TextAlignment.Center
                    } ,
                    new ContentView {
                        Padding = new Thickness(0,5,0,-10),
                        VerticalOptions = LayoutOptions.EndAndExpand,
                        Content = content
                    } 
                }
            };

            var buttonViews = buttons.Select(ab => new Button {
                FontSize = Math.Max(16, titleSize),
                Text = ab.Text,
                FontAttributes = ab.IsPreferred ? FontAttributes.Bold : FontAttributes.None,
                TextColor = ab.IsDestructive ? Color.Red : Color.Default,
                Command = new Command(async () => {
                    var cont = true;
                    if (ab.Action != null)
                        cont = ab.Action();
                    if (ab.ActionAsync != null)
                        cont = cont && await ab.ActionAsync();
                    if (!cont)
                        await dismiss();
                })
            }).ToList();

            var grid = new Grid {
                RowDefinitions = {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                ColumnSpacing = 0,
                RowSpacing = 0
            };
            buttons.ForEach(button => {
                grid.ColumnDefinitions.Add(
                    new ColumnDefinition {
                        Width = AlertWidth / buttonViews.Count
                    }
                );
            });

            for (int i = 0; i < buttonViews.Count; i++)
            {
                grid.Children.Add(new BorderView {
                    BorderColor = Color.FromRgba(0,0,0,0.2),
                    Thickness = new Thickness(0, 1, (i + 1 < buttonViews.Count) ? 1 : 0, 0)
                }, i, 1);
                grid.Children.Add(buttonViews[i], i, 1);
            }
            grid.Children.Add(top, 0, buttons.Count, 0, 1);

            var box = new Frame {
                WidthRequest = AlertWidth,
                BackgroundColor = Color.FromRgba(1,1,1,0.96),
                Padding = 0,
                Content = grid
            };
            var outer = new AbsoluteLayout {
                BackgroundColor = Color.FromRgba(0,0,0,0.65),
                Opacity = 0,
                Children = { box }
            };
            AbsoluteLayout.SetLayoutFlags(box, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(box,
                new Rectangle(0.5, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

            var page = new ContentPage {
                Content = /* new ScrollView { Content = */ outer // }
            };

            var tcs = new TaskCompletionSource<object>();

            var topVC = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topVC.PresentedViewController != null) {
                topVC = topVC.PresentedViewController;
            }

            var vc = page.CreateViewController();
            topVC.Add(vc.View);
            var innerView = vc.View.Subviews[0].Subviews[0];
            vc.View.RemoveFromSuperview();

            dismiss = async () => {
                dismiss = async () => {};
                await outer.FadeTo(0, 50);
                innerView.RemoveFromSuperview();
                tcs.SetResult(null);
            };

            topVC.Add(innerView);

            var kbh = new KeyboardHelper();
            kbh.KeyboardChanged += async (sender, e) => {
                await box.TranslateTo(0, e.Visible ? (-e.Height / 2f) : 0, 100, Easing.CubicInOut);
            };

            await outer.FadeTo(1, 100);

            await tcs.Task;
        }
    }
}

