using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using NControl.Abstractions;

namespace ACD.App
{
    public class DiaryEntryPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromRgb(242, 183, 57);
        public static readonly Color BarTextColor = Color.White;

        public DiaryEntryPage(DiaryEntry de)
        {
            Title = "Dagboeknotitie";

            var date = new Label {
                Style = StyleKit.AutoDarkLabelStyles.Display,
                Text = de.Day.ToString("D")
            };

            var editorStyles = StyleKit.AutoDarkStyles<Editor>();

            var text = new Editor {
                BindingContext = de,
                Style = editorStyles.Body
            };
            text.SetBinding(Editor.TextProperty, (DiaryEntry d) => d.Text, BindingMode.TwoWay);

            var textFrame = new Frame {
                Content = text,
                Padding = 3,
                OutlineColor = Color.Gray.MultiplyAlpha(0.6),
                HasShadow = false
            };

            var help = new Label {
                Text = "Hierboven kan je iets schrijven over wat je vandaag gedaan hebt en hoe je dat vond. Dit wordt automatisch opgeslagen en je kunt het op een later moment altijd teruglezen of bewerken.",
                HorizontalTextAlignment = TextAlignment.Center,
                Style = StyleKit.AutoDarkLabelStyles.Caption
            };

            var layout = new StackLayout {
                Padding = StyleKit.AutoPadding,
                Spacing = StyleKit.AutoSpacing.Medium,
                Children = {
                    date,
                    textFrame,
                    help
                }
            };

            help.SizeChanged += (sender, e) => {
                textFrame.HeightRequest = layout.Height - 2*layout.Padding.VerticalThickness - help.Height - date.Height;
            };

            Content = new ScrollView {
                Content = layout
            };

            /* var bodyEditor = new Editor {
                //HeightRequest = 150
            };
            var editorCell = new ViewCell {
                View = new StackLayout {
                    Padding = new Thickness(4, 4),
                    Children = {
                        bodyEditor
                    }
                },
                //Height = 158
            };

            bodyEditor.SetBinding(Editor.TextProperty, (DiaryEntry d) => d.Text, BindingMode.TwoWay);

            Content = new TableView {
                HasUnevenRows = true,
                Root = new TableRoot {
                    new TableSection("Datum") {
                        new TextCell {
                            Text = de.Day.ToString("D"),
                        }
                    },
                    new TableSection("Tekst") {
                        new ViewCell {
                            View = new StackLayout {
                                Padding = StyleKit.AutoPaddingLight,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                Children = {
                                    //bodyEditor,
                                    new Label {
                                        Text = "Hierboven kunt u iets schrijven over wat u vandaag gedaan heeft en hoe u dat vond. Dit wordt automatisch opgeslagen en u kunt het op een later moment altijd teruglezen of bewerken.",
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        VerticalOptions = LayoutOptions.CenterAndExpand,
                                        Style = StyleKit.AutoDarkLabelStyles.Caption
                                    }
                                }
                            }
                        }
                    }
                },
                Intent = TableIntent.Settings
            }; */
        }
    }
}
