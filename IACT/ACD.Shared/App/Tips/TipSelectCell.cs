using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class TipSelectCell : ViewCell
    {
        public TipSelectCell()
        {
            var check = new Checkbox {
                Scale = 1.2
            };
            check.SetBinding(Checkbox.CheckedProperty, "Enabled");

            var text = new Label {
                YAlign = TextAlignment.Center,
                Style = StyleKit.AutoDarkLabelStyles.Body
            };
            text.SetBinding(Label.TextProperty, "Title");

            var info = new ListButton {
                Text = "info",
                FontSize = 18
            };
            info.Clicked += (sender, args) =>
            {
                var parent = Parent;
                while (!(parent is Page))
                    parent = parent.Parent;
                ((Page)parent).Navigation.PushAsync(new TipInfoPage((Tip)BindingContext));
            };

            var layout = new Grid {
                ColumnDefinitions = {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            layout.Children.Add(new StackLayout {
                Orientation = StackOrientation.Horizontal,
                Padding = 10,
                Spacing = 15,
                Children = { check, text }
            }, 0, 0);
            layout.Children.Add(info, 1, 0);

            View = layout; 
        }
    }
}
