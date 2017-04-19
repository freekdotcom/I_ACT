using System;

using Xamarin.Forms;

namespace ACD
{
    public partial class TextStyles<T>
    {
        private void MakeStyles(Color firstColor, Color secondColor, TargetIdiom? idiom = null)
        {
            if (idiom == TargetIdiom.Phone)
            {
                display4 = CreateStyle(firstColor, 76, StyleKit.LightFont);
                display3 = CreateStyle(firstColor, 50, StyleKit.RegularFont);
                display2 = CreateStyle(firstColor, 42, StyleKit.RegularFont);
                display = CreateStyle(firstColor, 30, StyleKit.RegularFont, LineBreakMode.WordWrap);
                headline = CreateStyle(secondColor, 24, StyleKit.RegularFont, LineBreakMode.WordWrap);
                title = CreateStyle(firstColor, 16, StyleKit.MediumFont);
                subhead = CreateStyle(secondColor, 16, StyleKit.RegularFont, LineBreakMode.WordWrap);
                body2 = CreateStyle(firstColor, 14, StyleKit.MediumFont, LineBreakMode.WordWrap);
                body = CreateStyle(firstColor, 14, StyleKit.RegularFont, LineBreakMode.WordWrap);
                caption = CreateStyle(secondColor, 12, StyleKit.RegularFont, LineBreakMode.WordWrap);
            }
            else
            {
                display4 = CreateStyle(firstColor, 90, StyleKit.LightFont);
                display3 = CreateStyle(firstColor, 66, StyleKit.RegularFont);
                display2 = CreateStyle(firstColor, 52, StyleKit.RegularFont);
                display = CreateStyle(firstColor, 44, StyleKit.RegularFont, LineBreakMode.WordWrap);
                headline = CreateStyle(secondColor, 36, StyleKit.RegularFont, LineBreakMode.WordWrap);
                title = CreateStyle(firstColor, 22, StyleKit.MediumFont);
                subhead = CreateStyle(secondColor, 22, StyleKit.RegularFont, LineBreakMode.WordWrap);
                body2 = CreateStyle(firstColor, 20, StyleKit.MediumFont, LineBreakMode.WordWrap);
                body = CreateStyle(firstColor, 20, StyleKit.RegularFont, LineBreakMode.WordWrap);
                caption = CreateStyle(secondColor, 16, StyleKit.RegularFont, LineBreakMode.WordWrap);
            }
        }
    }
}
