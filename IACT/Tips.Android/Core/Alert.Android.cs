using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Widget;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using ACD.App.Droid;

[assembly: Dependency(typeof(Alert))]

namespace ACD.App.Droid
{
    public class Alert : IAlert
    {
        class AlertDialogFragment : DialogFragment
        {
            public string Title;
            public string Body;
            public View Content;
            public List<AlertButton> Buttons;

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                // Use the Builder class for convenient dialog construction
                AlertDialog.Builder builder = new AlertDialog.Builder(Activity);


                builder.SetMessage(Body)
                       .SetNeutralButton("Oké", (Alert, args) =>
                       {

                       });
                // Create the AlertDialog object and return it
                return builder.Create();
            }
        }

        public async Task Show(string title, string body, View content, List<AlertButton> buttons)
        {
            var adf = new AlertDialogFragment
            {
                Title = title,
                Body = body,
                Content = content,
                Buttons = buttons
            };

            var FragmentManager = ((Activity)Forms.Context).FragmentManager;

            FragmentTransaction ft = FragmentManager.BeginTransaction();
            //Remove fragment else it will crash as it is already added to backstack
            Fragment prev = FragmentManager.FindFragmentByTag("alert");
            if (prev != null)
            {
                ft.Remove(prev);
            }

            ft.AddToBackStack(null);

            adf.Show(ft, "alert");

            return;
        }
    }
}
