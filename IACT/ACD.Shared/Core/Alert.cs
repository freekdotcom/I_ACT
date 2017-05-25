using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

using ACD.App;
using Android.Widget;

namespace ACD
{
    public interface IAlert
    {
        Task Show(string title, string body, View content, List<AlertButton> buttons);
    }

    public static class Alert
    {
        public static async Task Show(string title, string body, View content = null, params AlertButton[] buttons)
        {
            var enabled = MainApp.BackEnabled;
            MainApp.BackEnabled = false;
            await DependencyService.Get<IAlert>().Show(title, body, content, buttons.ToList());
            MainApp.BackEnabled = enabled;
        }
    }

    public class AlertButton 
    {
        public string Text { get; set; }
        public bool IsDestructive { get; set; }
        public bool IsPreferred { get; set; }
        public Func<bool> Action { get; set; }
        public Func<Task<bool>> ActionAsync { get; set; }

    }
}

