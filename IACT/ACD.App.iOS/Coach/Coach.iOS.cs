using System;
using System.Threading.Tasks;
using System.IO;

using Foundation;
using UIKit;
using CoreGraphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace ACD
{
    public partial class Coach
    {
        private static async Task PlatformChecks()
        {
            await CheckCrashReport();

            //if (iOSNotificationScheduler.NeedPermission())
            //{
            //    await iOSNotificationScheduler.AskPermission();
            //}
        }

        private static async Task CheckCrashReport ()
        {
            const string errorFilename = "Fatal.log";
            var libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Resources);
            var errorFilePath = Path.Combine(libraryPath, errorFilename);

            if (!File.Exists(errorFilePath))
            {
                return;
            }

            var errorText = File.Exists (errorFilePath) ? File.ReadAllText (errorFilePath) : "";
            var alertText = errorText.Length > 140 ? errorText.Substring (0, 140) + "..." : errorText;
            await Alert.Show("Er ging iets fout:", "Een deel van de fout is hieronder weergegeven. "
                + "Druk op \"Meld crash\" om deze naar de ontwikkelaar te versturen. Er wordt geen persoonlijke informatie verstuurd.\n\n"
                + alertText, null,
                new AlertButton
                {
                    Text = "Sluiten",
                    Action = () => false
                },
                new AlertButton
                {
                    Text = "Meld crash",
                    IsPreferred = true,
                    ActionAsync = async () =>
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("I-ACT", "I-ACT-tijdelijk@nardilam.nl"));
                        message.To.Add(new MailboxAddress("I-ACT ontwikkelaar", "mail@nardilam.nl"));
                        message.Subject = "Crash report";
                        message.Body = new TextPart("plain")
                        {
                            Text = errorText
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

                        await Alert.Show(
                            "Dank je wel!",
                            "We zullen je melding gebruiken om de app te verbeteren.",
                            null, new AlertButton
                            {
                                Text = "Sluiten",
                                Action = () => false
                            }
                        );

                        return false;
                    }
                }
            );
            
            File.Delete (errorFilePath);
        }
    }
}
