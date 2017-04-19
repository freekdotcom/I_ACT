using System;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace ACD.App
{
	public partial class SetupPage
	{
        private async Task PlatformSetup(Label titleLabel, Label explainLabel, Button nextButton)
        {
            if (iOSNotificationScheduler.NeedPermission())
            {
                titleLabel.Text = "Notificaties";
                explainLabel.Text = "Om je goed van dienst te zijn, geeft deze app je af en toe een melding. Hiervoor hebben we je toestemming nodig. Druk alsjeblieft op \"Volgende\" en daarna op \"OK\" om deze app toestemming te geven om notificaties te laten zien.";

                //do
                //{
                nextButton.IsEnabled = true;
                await nextButton.GetEventAsync<EventArgs>("Clicked");
                nextButton.IsEnabled = false;

                await iOSNotificationScheduler.AskPermission();
                //} while (iOSNotificationScheduler.NeedPermission());
            }
        }
	}
}
