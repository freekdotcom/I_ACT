using System;
using System.Threading.Tasks;

using ACD.App.iOS;

namespace ACD.App
{
	public partial class CoachPage
	{
        private void PlatformSetup(Coach coach)
        {
            if (iOSNotificationScheduler.NeedPermission())
            {
                iOSNotificationScheduler.AskPermission();
            }
        }
	}
}
