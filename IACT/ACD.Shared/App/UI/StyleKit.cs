using System;

using Xamarin.Forms;

namespace ACD
{
    public static class StyleKit
    {
        public static string LightFont = Device.OnPlatform("HelveticaNeue-Light", "sans-serif-light", "");
        public static string RegularFont = Device.OnPlatform("HelveticaNeue", "sans-serif", "");
        public static string MediumFont = Device.OnPlatform("HelveticaNeue-Medium", "sans-serif-medium", "");

        public static TextStyles<Label> AutoDarkLabelStyles = new TextStyles<Label>(Color.Black, Color.Gray);
        public static TextStyles<Label> PhoneDarkLabelStyles = new TextStyles<Label>(Color.Black, Color.Gray, TargetIdiom.Phone);
        public static TextStyles<Label> TabletDarkLabelStyles = new TextStyles<Label>(Color.Black, Color.Gray, TargetIdiom.Tablet);

        public static TextStyles<Label> AutoLightLabelStyles = new TextStyles<Label>(Color.White, Color.Gray);

        public static TextStyles<Button> AutoLightButtonStyles = new TextStyles<Button>(Color.White, Color.Gray);
        public static TextStyles<Button> PhoneLightButtonStyles = new TextStyles<Button>(Color.White, Color.Gray, TargetIdiom.Phone);
        public static TextStyles<Button> TabletLightButtonStyles = new TextStyles<Button>(Color.White, Color.Gray, TargetIdiom.Tablet);

        public static TextStyles<T> AutoDarkStyles<T>() { return new TextStyles<T>(Color.Black, Color.Gray); }
        public static TextStyles<T> PhoneDarkStyles<T>() { return new TextStyles<T>(Color.Black, Color.Gray, TargetIdiom.Phone); }
        public static TextStyles<T> TabletDarkStyles<T>() { return new TextStyles<T>(Color.Black, Color.Gray, TargetIdiom.Tablet); }

        private static T Auto<T>(T phone, T tablet)
        {
            return Device.Idiom == TargetIdiom.Phone ? phone : tablet;
        }

        public static Thickness AutoPadding { get { return Auto(PhonePadding, TabletPadding); } }
        public static Thickness AutoPaddingLight { get { return Auto(PhonePaddingLight, TabletPaddingLight); } }
        public static readonly Thickness PhonePadding = new Thickness(25, 30);
        public static readonly Thickness PhonePaddingLight = new Thickness(25, 15);
        public static readonly Thickness TabletPadding = new Thickness(70, 55);
        public static readonly Thickness TabletPaddingLight = new Thickness(70, 25);

        public class Spacing
        {
            public readonly int Small;
            public readonly int Medium;
            public readonly int Large;

            public Spacing(int s, int m, int l)
            {
                Small = s; Medium = m; Large = l;
            }
        }

        public static Spacing AutoSpacing { get { return Auto(PhoneSpacing, TabletSpacing); } }
        public static readonly Spacing PhoneSpacing = new Spacing(10, 15, 20);
        public static readonly Spacing TabletSpacing = new Spacing(15, 25, 35);
    }
}

