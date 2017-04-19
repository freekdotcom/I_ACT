using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class TipEntryPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromHex("68d7c6");
        public static readonly Color BarTextColor = Color.White;

        public TipEntryPage(Tip tip = null)
        {
            Title = "Tip bewerken";

            var bodyEditor = new Editor {
                Text = tip?.Description,
                HeightRequest = 150
            };
            var editorCell = new ViewCell {
                View = new StackLayout {
                    Padding = new Thickness(4, 4),
                    Children = {
                        bodyEditor
                    }
                },
                Height = 158
            };

            Content = new TableView {
                HasUnevenRows = true,
                Root = new TableRoot {
                    new TableSection("Uw tip") {
                        new ViewCell {
                            View = new StackLayout {
                                Children = {
                                    new Entry {
                                        Text = tip?.Title,
                                        Placeholder = "Vat hier uw tip in één zin samen...",
                                        VerticalOptions = LayoutOptions.CenterAndExpand
                                    }
                                }
                            }
                        }
                    },
                    new TableSection("Verdere uitleg") {
                        editorCell,
                        new ViewCell {
                            IsEnabled = false,
                            View = new StackLayout {
                                Padding = new Thickness(10, 5),
                                Children = { 
                                    new Label {
                                        Text = "Gebruik dit veld om zo veel extra tekst toe te voegen als je wilt. Deze wordt samen met je tip laten zien.",
                                        HorizontalTextAlignment = TextAlignment.Center,
                                        VerticalOptions = LayoutOptions.CenterAndExpand,
                                        Style = StyleKit.AutoDarkLabelStyles.Caption
                                    }
                                }
                            }
                        }
                    },
                    new TableSection {
                        new TextCell {
                            Text = "Opslaan",
                            StyleId = "disclosure"
                        }
                    }
                },
                Intent = TableIntent.Form
            };
        }
    }
}
