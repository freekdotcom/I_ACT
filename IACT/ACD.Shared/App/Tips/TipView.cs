using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class TipView : StackLayout
    {
        public TipView()
        {
            Spacing = StyleKit.AutoSpacing.Large;

            var title = new Label {
                BindingContext = this,
                Style = StyleKit.AutoDarkLabelStyles.Display
            };

            var desc = new Label {
                BindingContext = this,
                Style = StyleKit.AutoDarkLabelStyles.Subhead
            };

            title.SetBinding(Label.TextProperty, "Tip.Title");
            desc.SetBinding(Label.TextProperty, "Tip.Description");

            Children.Add(title, desc);
        }

        public static readonly BindableProperty TipProperty = 
            BindableProperty.Create<TipView, Tip>(v => v.Tip, null);

        public Tip Tip
        {
            get
            {
                return (Tip)GetValue(TipProperty); 
            }
            set
            {
                SetValue(TipProperty, value);
            }
        }
    }
}
