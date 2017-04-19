using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
    public class Checkbox : Image
    {
        public static readonly BindableProperty CheckedProperty =
            BindableProperty.Create<Checkbox, bool>(p => p.Checked, false);

        public bool Checked
        {
            get
            {
                return (bool)GetValue(CheckedProperty);
            }
            set
            {
				Opacity = value ? 1 : 0;
                SetValue(CheckedProperty, value);
            }
        }

		public Checkbox()
		{
			Source = ImageSource.FromResource("ACD.App." + Device.OnPlatform("iOS.", "Droid.", "WinPhone.") + "check.png");
			Opacity = 0;
			PropertyChanged += (sender, args) =>
			{
				if (args.PropertyName == CheckedProperty.PropertyName)
				{
					Opacity = Checked ? 1 : 0;
				}

                if (args.PropertyName == OpacityProperty.PropertyName)
                {
                    var opacity = Checked ? 1 : 0;
                    if (Opacity != opacity)
                        Opacity = opacity;
                }
			};
		}
    }
}
