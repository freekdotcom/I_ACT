using Xamarin.Forms;

namespace ACD.App
{
	public class ExtButton : Button
	{
		public ExtButton()
		{
			Device.OnPlatform(Android: () =>
			{
				HorizontalOptions = LayoutOptions.Center;
			});
		}
	}
}

