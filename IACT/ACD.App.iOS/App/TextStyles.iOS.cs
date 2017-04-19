using System;
using System.Linq;

using Xamarin.Forms;

using UIKit;

namespace ACD
{
    public partial class TextStyles<T>
    {
        private void MakeStyles(Color firstColor, Color secondColor, TargetIdiom? idiom = null)
        {
            if (idiom == TargetIdiom.Phone)
            {
                display = CreateStyle(firstColor, (float)UIFontDescriptor.PreferredHeadline.PointSize * 1.8, StyleKit.RegularFont, LineBreakMode.WordWrap);
                headline = CreateStyle(secondColor, (float)UIFontDescriptor.PreferredHeadline.PointSize * 1.2, StyleKit.RegularFont, LineBreakMode.WordWrap);
                title = CreateStyle(firstColor, (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.MediumFont);
                subhead = CreateStyle(secondColor, (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.RegularFont, LineBreakMode.WordWrap);
                body2 = CreateStyle(firstColor, (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.MediumFont, LineBreakMode.WordWrap);
                body = CreateStyle(firstColor, (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.RegularFont, LineBreakMode.WordWrap);
                caption = CreateStyle(secondColor, (float)UIFontDescriptor.PreferredFootnote.PointSize, StyleKit.RegularFont, LineBreakMode.WordWrap);
            }
            else
            {
                display = CreateStyle(firstColor, 1.2 * (float)UIFontDescriptor.PreferredHeadline.PointSize * 1.8, StyleKit.RegularFont, LineBreakMode.WordWrap);
                headline = CreateStyle(secondColor, 1.2 * (float)UIFontDescriptor.PreferredHeadline.PointSize * 1.2, StyleKit.RegularFont, LineBreakMode.WordWrap);
                title = CreateStyle(firstColor, 1.2 * (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.MediumFont);
                subhead = CreateStyle(secondColor, 1.2 * (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.RegularFont, LineBreakMode.WordWrap);
                body2 = CreateStyle(firstColor, (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.MediumFont, LineBreakMode.WordWrap);
                body = CreateStyle(firstColor, (float)UIFontDescriptor.PreferredHeadline.PointSize, StyleKit.RegularFont, LineBreakMode.WordWrap);
                caption = CreateStyle(secondColor, (float)UIFontDescriptor.PreferredFootnote.PointSize, StyleKit.RegularFont, LineBreakMode.WordWrap);
            }
        }
    }
}
