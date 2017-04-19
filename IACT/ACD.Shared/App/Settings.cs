using System;

namespace ACD.App
{
	public class Settings
	{
		private Settings() {}

		private static Settings settings = new Settings();

		public static Settings Get()
		{
			return settings;
		}

		public TimeSpan MoodTime
		{
			get
			{
				return Preferences.Get<TimeSpan>("moodTime");
			}

			set
			{
				int nID = Preferences.GetOr("moodNotification", -1);
				if (nID != -1)
					NotificationCenter.Cancel(nID);

				Preferences.Set("moodTime", value);
				var time = DateTime.Now.Date.Add(value);
				if (time <= DateTime.Now)
					time = time.AddDays(1);
				var moodNt = new Notification {
					Title = "I-ACT",
					Body = "Hoe gaat het met je? Open de stemmingsmeter om je stemming door te geven.",
					Open = "0",
					Action = "Naar stemmingsmeter",
					Time = time,
					Repeat = TimeSpan.FromDays(1)
				};
				NotificationCenter.Schedule(moodNt);
				Preferences.Set("moodNotification", moodNt.ID);
			}
		}
	}
}

