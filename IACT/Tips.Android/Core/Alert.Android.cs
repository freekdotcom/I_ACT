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


            public Dialog AndroidCustomAlert(Activity activ)
            {
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

                if (Title.Contains("Tijd"))
                {
                    Android.Views.View secondView = inflater.Inflate(Resource.Layout.TimePickerLayout, null);
                    builder.SetView(secondView);

                    btnPositive = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnLoginLL);
                    btnNegative = secondView.FindViewById<Android.Widget.Button>(Resource.Id.btnClearLL);
                    var tp = secondView.FindViewById<Android.Widget.TimePicker>(Resource.Id.timePicker1);
                    tp.SetIs24HourView((Java.Lang.Boolean)true);
                    //Positive button feedback
                    btnPositive.Text = Buttons.Last().Text;
                    btnPositive.Click += delegate
                    {
                        var car = (Xamarin.Forms.TimePicker)Content;
                        var ts = new TimeSpan(tp.Hour, tp.Minute, 0);
                        car.Time = ts;

                        CommandsForButtons(Buttons.Last());
                    };

                    //Negative button feedback
                    btnNegative.Text = Buttons.First().Text;
                    btnNegative.Click += delegate
                    {

                        CommandsForButtons(Buttons.First());
                    };
                }
                else
                {
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

                    if (Content == null)
                    {
                        pincode.Visibility = Android.Views.ViewStates.Gone;
                    }
                    else
                    {

                    }

                    //Positive button feedback
                    btnPositive.Text = Buttons.Last().Text;
                    btnPositive.Click += delegate
                    {
  
                            var test = (StackLayout)Content;
                            var car = (Entry)test.Children[0];
                            car.Text = pincode.Text;
                  

                        CommandsForButtons(Buttons.Last());
                    };

                    //Negative button feedback
                    btnNegative.Text = Buttons.First().Text;
                    btnNegative.Click += delegate
                    {

                        CommandsForButtons(Buttons.First());
                    };
                }

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
                test.SetCanceledOnTouchOutside(false);
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

            if (title.Contains("welkom"))
                tcs.SetResult(null);

            ft.AddToBackStack(null);

            adf.Show(ft, "alert");

            await tcs.Task;
                    }

    }
}
