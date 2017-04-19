using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class TipInfoPage : ContentPage
    {
        public static readonly Color BarBackgroundColor = Color.FromHex("68d7c6");
        public static readonly Color BarTextColor = Color.White;

        public TipInfoPage(Tip tip)
        {
            Title = "Voorbeeld van tip";
            Content = new ScrollView {
                Content = new TipView {
                    Tip = tip,
                    Padding = StyleKit.AutoPadding
                }
			};
        }
    }
}
