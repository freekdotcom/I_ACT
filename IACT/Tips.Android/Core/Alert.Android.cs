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
using System.Diagnostics.Contracts;

[assembly: Dependency(typeof(Alert))]

namespace ACD.App.Droid
{
    public class Alert : IAlert
    {
        public static readonly int AlertWidth = Device.Idiom == TargetIdiom.Phone ? 270 : 320;

        class AlertDialogFragment : DialogFragment
        {
            public string Title;
            public string Body;
            public View Content;
            public List<AlertButton> Buttons;
            public Func<Task> tcs;


            public Dialog AndroidCustomAlert(Activity activ)
            {
                Contract.Ensures(Contract.Result<Dialog>() != null);

                Android.Views.LayoutInflater inflater = Android.Views.LayoutInflater.From(activ);
                Android.Views.View view = inflater.Inflate(Resource.Layout.AlertDialogLayout, null);

                AlertDialog.Builder builder = new AlertDialog.Builder(activ);
                builder.SetView(view);
                Android.Widget.TextView title = view.FindViewById<Android.Widget.TextView>(Resource.Id.Login);
                title.Text = Title;

                Android.Widget.TextView body = view.FindViewById<Android.Widget.TextView>(Resource.Id.pincodeText);
                body.Text = Body;

                Android.Widget.EditText pincode = view.FindViewById<Android.Widget.EditText>(Resource.Id.pincodeEditText);
                Android.Widget.Button btnPositive = view.FindViewById<Android.Widget.Button>(Resource.Id.btnLoginLL);
                Android.Widget.Button btnNegative = view.FindViewById<Android.Widget.Button>(Resource.Id.btnClearLL);
                Android.Widget.Button btnNeutral = view.FindViewById<Android.Widget.Button>(Resource.Id.btnNeutral);

                //Checks if there are no buttons, and if there aren't any, creates a neutral one
                if (Buttons == null || Buttons.Count == 0)
                {
                    btnPositive.Visibility = Android.Views.ViewStates.Gone;
                    btnNegative.Visibility = Android.Views.ViewStates.Gone;
                    btnNeutral.Visibility = Android.Views.ViewStates.Visible;
                    pincode.Visibility = Android.Views.ViewStates.Gone;

                    Buttons = new List<AlertButton> {
                    new AlertButton {
                        Text = "Oké",
                        IsPreferred = true,
                        Action = () => false
                    }
                };
                    btnNeutral.Text = Buttons.First().Text;
                    btnNeutral.Click += delegate
                    {
                        CommandsForButtons(Buttons.First());
                    };
                }

          
                //Positive button feedback
                btnPositive.Text = Buttons.Last().Text;
                btnPositive.Click += delegate
                {
                    CommandsForButtons(Buttons.Last());
                };

                //Negative button feedback
                btnNegative.Text = Buttons.First().Text;
                btnNegative.Click += delegate
                {
                    CommandsForButtons(Buttons.First());
                };

                builder.SetCancelable(false);
                return builder.Create();
            }

            public void CommandsForButtons(AlertButton button)
            {      
                var command = new Command(async () =>
                {
                    var ab = button;
                    var cont = true;
                    if (ab.Action != null)
                        cont = ab.Action();
                    if (ab.ActionAsync != null)
                    {
                        cont = cont && await ab.ActionAsync();
                    }
                    if (!cont)
                    {
                        Dismiss();
                    }
                });

                command.Execute(this);
            }

            public override Dialog OnCreateDialog(Bundle savedInstanceState)
            {
                var test = AndroidCustomAlert(Activity);
                //test.SetCanceledOnTouchOutside(false);
                return test;
            }

        }

        public async Task Show(string title, string body, View content, List<AlertButton> buttons)
        {

            var tcs = new TaskCompletionSource<object>();

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

            await tcs.Task;

            return;
        }

    }
}
