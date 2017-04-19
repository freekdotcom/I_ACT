using System;
using Foundation;
using UIKit;

/* Source: http://stackoverflow.com/questions/24353159/scroll-editor-in-xamarin-forms-into-view */

namespace ACD.App.iOS
{
    public class KeyboardHelper
    {
        public KeyboardHelper()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, OnKeyboardNotification);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, OnKeyboardNotification);
        }

        public event EventHandler<KeyboardHelperEventArgs> KeyboardChanged;

        private void OnKeyboardNotification(NSNotification notification)
        {
            var visible = notification.Name == UIKeyboard.WillShowNotification;
            var keyboardFrame = visible
                ? UIKeyboard.FrameEndFromNotification(notification)
                : UIKeyboard.FrameBeginFromNotification(notification);
            if (KeyboardChanged != null)
            {
                KeyboardChanged (this, new KeyboardHelperEventArgs(visible, (float)keyboardFrame.Height));
            }
        }

        public class KeyboardHelperEventArgs : EventArgs 
        {
            public readonly bool Visible;
            public readonly float Height;

            public KeyboardHelperEventArgs(bool visible, float height)
            {
                Visible = visible;
                Height = height;
            }
        }
    }
}

