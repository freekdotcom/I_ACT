using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace ACD.App
{
    public class SelectTipsList : ListView
    {
        public SelectTipsList()
        {
            HasUnevenRows = true;
            ItemTemplate = new DataTemplate(() => {
                var cell = new SwitchCell {
                    //StyleId = "detail-button"
                };
                var edit = new MenuItem {
                    Text = "Wijzig"
                };
                edit.Clicked += async (sender, e) => {
                    await Navigation.PushAsync(new TipEntryPage((Tip)cell.BindingContext));
                };
                /* TODO: Finish tip editor, then restore menu option */
                // cell.ContextActions.Add(edit);
                return cell;
            });
            ItemTemplate.SetBinding(SwitchCell.TextProperty, "Title");
            ItemTemplate.SetBinding(SwitchCell.OnProperty, "Enabled");
            ItemTapped += async (sender, args) =>
            {
                var tip = ((Tip)args.Item);
                //tip.Enabled = !tip.Enabled;
                await Navigation.PushAsync(new TipInfoPage(tip));
            };
        }
    }
}
