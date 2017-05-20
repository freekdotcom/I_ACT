using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

using Xamarin.Forms;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

using ACD.App;

using System.Diagnostics;

namespace ACD
{
    /*
     * Ties together all the components required by the application.
     */
    public partial class Coach
    {
        public TipStore Tips { get; private set; }
        public TipScheduler Scheduler { get; private set; }
        public Profile UserProfile { get; private set; }
        public List<Monitor> Monitors { get; private set; }

        public Coach(Profile userProfile, TipStore tips, TipScheduler tipScheduler, List<Monitor> monitors)
        {
            UserProfile = userProfile;
            Tips = tips;
            Scheduler = tipScheduler;
            Monitors = monitors;

            NotificationCenter.NotificationRecieved += async (sender, e) =>
            {
                await PerformChecks(false);
            };
        }

        public Coach(Profile userProfile, TipStore tips)
            : this(userProfile, tips,
                new TipScheduler(userProfile, tips),
                new List<Monitor>
                {
                    new MoodMonitor(userProfile),
                    new ActivityMonitor(userProfile)
                }
            )
        {
        }

        TextStyles<Entry> entryStyles = StyleKit.AutoDarkStyles<Entry>();
        private bool authenticated = false;
        public async Task Authenticate(INavigation nav = null)
        {
            if (authenticated)
                return;

            MainApp.BackEnabled = false;

            var entry = new Entry
            {
                Keyboard = Keyboard.Numeric,
                IsPassword = true,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                WidthRequest = (double)entryStyles.Body.Setters.First(s => s.Property == Entry.FontSizeProperty).Value * 4,
                Style = entryStyles.Body
            };

            var error = new Label
            {
                Text = "Verkeerde pincode. Probeer het nog eens.",
                HorizontalTextAlignment = TextAlignment.Center,
                Style = StyleKit.AutoDarkLabelStyles.Caption,
                //FontSize = 12,
                TextColor = Color.Red,
                IsVisible = false
            };

            entry.TextChanged += (sender, e) =>
            {
                if (e.NewTextValue?.Length > 4)
                    entry.Text = e.OldTextValue;
                else
                    error.IsVisible = false;
            };

            await Alert.Show(
                "Welkom!",
                "Vul om door te gaan a.u.b. hieronder je pincode in.",
                new StackLayout
                {
                    Children = {
                        entry,
                        error
                    },
                    Padding = 0,
                    Spacing = StyleKit.PhoneSpacing.Small
                },
                new AlertButton
                {
                    Text = "Vergeten",
                    ActionAsync = async () =>
                    {
                        var canceled = false;
                        var email = Preferences.Get<string>(UserProfile.ID + "_email");
                        await Alert.Show(
                            "Pincode resetten",
                            $"Omdat je je pincode vergeten bent kunnen wij je een nieuwe toesturen via e-mail. Deze wordt dan gestuurd naar het adres \"{email}\".",
                            null,
                            new AlertButton
                            {
                                Text = "Nee, dank je",
                                Action = () =>
                                {
                                    canceled = true;
                                    return false;
                                }
                            },
                            new AlertButton
                            {
                                Text = "Ja, graag",
                                IsPreferred = true,
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                                ActionAsync = async () =>
                                {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
                                    var message = new MimeMessage();
                                    message.From.Add(new MailboxAddress("I-ACT", "I-ACT-tijdelijk@nardilam.nl"));
                                    message.To.Add(new MailboxAddress("I-ACT gebruiker", email));
                                    message.Subject = "Nieuwe pincode I-ACT";

                                    var newPass = SetTemporaryPassword();

                                    message.Body = new TextPart("plain")
                                    {
                                        Text = $@"Beste mevrouw/meneer,

U heeft via de I-ACT app aangegeven dat u uw pincode voor die app vergeten bent en graag een nieuwe zou willen. Uw nieuwe pincode is:

{newPass}

We hopen dat u de app nu verder plezierig kunt blijven gebruiken.

Met vriendelijke groet,
I-ACT"
                                    };
                                    using (var client = new SmtpClient())
                                    {
                                        client.Connect("smtp.gmail.com", 465, true);

                                        // Note: since we don't have an OAuth2 token, disable
                                        // the XOAUTH2 authentication mechanism.
                                        client.AuthenticationMechanisms.Remove("XOAUTH2");

                                        // Note: only needed if the SMTP server requires authentication
                                        client.Authenticate("i-act-tijdelijk@nardilam.nl", "suwwrmtzfkulsoix");

                                        client.Send(message);
                                        client.Disconnect(true);
                                    }
                                    if (!canceled)
                                    {
                                        await Alert.Show(
                                            "Pincode gereset!",
                                            "We hebben een nieuwe pincode naar je e-mail toegestuurd. Je kunt die nu gebruiken om in te loggen.\nGa daarna a.u.b. naar de instellingen van de app om een eigen pincode in te stellen voordat je de app verder gebruikt."
                                        );
                                    }
                                    return false;
                                }
                                
                            }
                        
                        );
                        if (!canceled)
                        {
                            await Alert.Show(
                                "Pincode gereset!",
                                "We hebben een nieuwe pincode naar je e-mail toegestuurd. Je kunt die nu gebruiken om in te loggen.\nGa daarna a.u.b. naar de instellingen van de app om een eigen pincode in te stellen voordat je de app verder gebruikt."
                            );
                        }
                        return true;
                    }
                },
                new AlertButton
                {
                    Text = "Klaar",
                    IsPreferred = true,
                    Action = () =>
                    {
                        if (CheckPassword(UserProfile, entry.Text))
                        {
                            authenticated = true;
                            MainApp.BackEnabled = true;
                            return false;
                        }
                        entry.Focus();
                        error.IsVisible = true;
                        error.IsVisible = false;
                        error.IsVisible = true;
                        return true;
                    }
                }
            );
        }

        SemaphoreSlim checkSemaphore = new SemaphoreSlim(1, 1);

        public async Task PerformChecks(bool authenticate)
        {
            await checkSemaphore.WaitAsync();

            await PlatformChecks();

            if (authenticate)
                await Authenticate();

            var diary = UserProfile.Diary;
            var newestEntry = diary.FirstOrDefault();
            var today = DateTime.Today;
            if (newestEntry == null || newestEntry.Day != today)
            {
                var diff = newestEntry == null ? 1 : Math.Min((today - newestEntry.Day).Days, 2);
                for (int i = 0; i < diff; i++)
                    diary.Add(new DiaryEntry(today.AddDays(-i)));
            }
            var oldEntries = diary.Skip(2).Where(entry => string.IsNullOrWhiteSpace(entry.Text));
            foreach (var entry in oldEntries.ToList())
                diary.Remove(entry);

            var newTip = Scheduler.CheckSchedule();

            foreach (var monitor in Monitors)
            {
                if (monitor.Check())
                {
                    await Authenticate();
                    await monitor.Perform();
                }
            }

            // TODO: test
            var onTipPage = (MainApp.Navigation.CurrentPage as CoachPage)?.CurrentPage is TipPage;
            if (newTip && !onTipPage)
            {
                await Alert.Show(
                    "Nieuwe tip",
                    "Er staat een nieuwe tip voor je klaar!",
                    null,
                    new AlertButton
                    {
                        Text = "Later",
                        Action = () => false
                    },
                    new AlertButton
                    {
                        Text = "Naar tip",
                        IsPreferred = true,
                        Action = () =>
                        {
                            MainApp.Navigation.PushAsync(new TipPage(this, "Nieuwe tip"));
                            return false;
                        }
                    }
                );
            }

            checkSemaphore.Release();
        }

        public static byte[] HashPassword(string pass)
        {
            return SHA256.Create().ComputeHash(ASCIIEncoding.UTF8.GetBytes(pass));
        }

        private static bool CheckPassword(Profile profile, string pass)
        {
            if (pass == null)
                return false;
            return HashPassword(pass).SequenceEqual(Preferences.Get<byte[]>(profile.ID + "_password"));
        }

        public static bool ValidPassword(string pass)
        {
            int n;
            return !string.IsNullOrWhiteSpace(pass) && pass.Length == 4 && int.TryParse(pass, out n);
        }

        static Random rng = new Random();
        private string SetTemporaryPassword()
        {
            var passText = rng.Next(0, 10000).ToString("0000");
            var password = Coach.HashPassword(passText);
            Preferences.Set(UserProfile.ID + "_password", password);
            return passText;
        }
    }
}
