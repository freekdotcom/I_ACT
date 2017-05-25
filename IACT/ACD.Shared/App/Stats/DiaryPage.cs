using System;

using Xamarin.Forms;

namespace ACD.App
{
    public class DiaryPage : ContentPage
    {
        public static readonly string EmptyEntry = "Nog leeg";

        public static readonly Color BarBackgroundColor = Color.FromRgb(242, 183, 57);
        public static readonly Color BarTextColor = Color.White;

        //ListView list;

        public DiaryPage(Coach coach)
        {
            Title = "Dagboekje";

            var diary = coach.UserProfile.Diary;
            var list = new ListView(ListViewCachingStrategy.RecycleElement) {
                ItemsSource = diary,
                ItemTemplate = new DataTemplate(() => {
                    var cell = new TextCell {
                        StyleId = "disclosure"
                    };
                    cell.SetBinding(TextCell.TextProperty,
                        (DiaryEntry de) => de.Text,
                        (string text) => string.IsNullOrWhiteSpace(text) ? EmptyEntry : text);
                    cell.SetBinding(TextCell.TextColorProperty,
                        (DiaryEntry de) => de.Text,
                        (string text) => string.IsNullOrWhiteSpace(text) ? Color.Gray.MultiplyAlpha(0.8) : Color.Black);
                    cell.SetBinding(TextCell.DetailProperty, "Day");
                    cell.SetBinding(TextCell.DetailProperty,
                        (DiaryEntry de) => de.Day,
                        (DateTime day) => day.ToString("D"));
                    return cell;
                })
            };

            list.ItemTapped += async (sender, args) => {
                var de = (DiaryEntry)args.Item;
                await Navigation.PushAsync(new DiaryEntryPage(de));
            };

            Content = new StackLayout {
                Spacing = 0,
                Padding = 0,
                Children = {
                    new StackLayout {
                        Spacing = StyleKit.AutoSpacing.Small,
                        Padding = StyleKit.AutoPaddingLight,
                        Children = {
                            new Label {
                                Text = "Hier kan je zien wat je allemaal in je dagboekje hebt geschreven. Iedere notitie hoort bij een dag. Druk op een notitie om deze helemaal te lezen of te bewerken.",
                                HorizontalTextAlignment = TextAlignment.Center,
                                Style = StyleKit.AutoDarkLabelStyles.Caption
                            }
                        }
                    },
                    list
                }
            };
        }
    }
}

