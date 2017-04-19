using System;
using System.Linq;

using Xamarin.Forms;

namespace ACD
{
    public partial class TextStyles<T>
    {
        private Style display4, display3, display2, display, headline,
        title, subhead, body2, body, caption;

        public TextStyles(Color firstColor, Color secondColor, TargetIdiom? idiom = null)
        {
            idiom = idiom ?? Device.Idiom;

            MakeStyles(firstColor, secondColor, idiom);

            /* For other platforms:
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
            */
        }

        #region StyleProperties

        /* public Style Display4 {
            get {
                return display4;
            }
        }

        public Style Display3 {
            get {
                return display3;
            }
        }

        public Style Display2 {
            get {
                return display2;
            }
        } */

        public Style Display {
            get {
                return display;
            }
        }

        /* public Style Headline {
            get {
                return headline;
            }
        } */

        public Style Title {
            get {
                return title;
            }
        }

        public Style Subhead {
            get {
                return subhead;
            }
        }

        public Style Body2 {
            get {
                return body2;
            }
        }

        public Style Body {
            get {
                return body;
            }
        }

        public Style Caption {
            get {
                return caption;
            }
        }

        #endregion

        /* private Style CreateStyle (
            Color textColor, 
            double fontSize, 
            string fontFamily, 
            LineBreakMode breakmode = LineBreakMode.WordWrap)
        {
            return new Style(typeof(Label))
            {
                Setters = {
                    new Setter { Property = Label.TextColorProperty, Value = textColor },
                    new Setter { Property = Label.FontSizeProperty, Value = fontSize },
                    new Setter { Property = Label.FontFamilyProperty, Value = fontFamily },
                    new Setter { Property = Label.LineBreakModeProperty, Value = breakmode }
                }
            };
        } */

        private Style CreateStyle (
            Color textColor, 
            double fontSize, 
            string fontFamily, 
            LineBreakMode breakmode = LineBreakMode.WordWrap)
        {
            Type t = typeof(T);

            return new Style(t)
            {
                Setters = {
                    new Setter { Property = t.GetStatic<BindableProperty>("TextColorProperty"), Value = textColor },
                    new Setter { Property = t.GetStatic<BindableProperty>("FontSizeProperty"), Value = fontSize },
                    new Setter { Property = t.GetStatic<BindableProperty>("FontFamilyProperty"), Value = fontFamily },
                    new Setter { Property = t.GetStatic<BindableProperty>("LineBreakModeProperty"), Value = breakmode }
                }
            };
        }
    }
}

