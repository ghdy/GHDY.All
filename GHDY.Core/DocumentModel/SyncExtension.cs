using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Documents;

namespace GHDY.Core.DocumentModel
{
    public static class SyncExtension
    {
        #region AP IsCurrent
        public static bool GetIsCurrent(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsCurrentProperty);
        }

        public static void SetIsCurrent(DependencyObject obj, bool value)
        {
            obj.SetValue(IsCurrentProperty, value);
        }

        public static readonly DependencyProperty IsCurrentProperty =
            DependencyProperty.RegisterAttached("IsCurrent", typeof(bool), typeof(SyncExtension), new UIPropertyMetadata(false));
        #endregion

        #region AP IsQuate
        public static bool GetIsQuate(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsQuateProperty);
        }

        public static void SetIsQuate(DependencyObject obj, bool value)
        {
            obj.SetValue(IsQuateProperty, value);
        }

        public static readonly DependencyProperty IsQuateProperty =
            DependencyProperty.RegisterAttached("IsQuate", typeof(bool), typeof(SyncExtension), new UIPropertyMetadata(false));

        private static void IsQuateProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var sentence = sender as DMSentence;

            if (sentence != null)
                CheckSentenceTimeRange(sentence);
        }

        public static void CheckSentenceTimeRange(DMSentence sentence)
        {
            var isQuate = (bool)sentence.GetValue(SyncExtension.IsQuateProperty);
            if (isQuate == true)
            {
                if (sentence.BeginTime > TimeSpan.Zero &&
                    sentence.EndTime > sentence.BeginTime)
                {
                    sentence.Foreground = Brushes.Blue;
                }
                else
                    sentence.Foreground = Brushes.Gray;
            }
            else
            {
                sentence.Foreground = Brushes.Black;
            }
        }
        #endregion

        #region AP SpeechText
        public static string GetSpeechText(DependencyObject obj)
        {
            return (string)obj.GetValue(SpeechTextProperty);
        }

        public static void SetSpeechText(DependencyObject obj, string value)
        {
            obj.SetValue(SpeechTextProperty, value);
        }

        public static readonly DependencyProperty SpeechTextProperty =
            DependencyProperty.RegisterAttached("SpeechText", typeof(string), typeof(SyncExtension), new UIPropertyMetadata(""));

        private static void SpeechTextProperty_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var run = sender as Run;
            var strikethrough = TextDecorations.Strikethrough;

            if (run != null)
            {
                var newText = e.NewValue.ToString();

                if (string.IsNullOrEmpty(newText) == true)
                    run.TextDecorations = new TextDecorationCollection();
                else
                {
                    run.ToolTip = newText;
                    run.TextDecorations = strikethrough;
                }
            }
        }

        #endregion

        #region AP Speaker


        public static Speaker GetSpeaker(DependencyObject obj)
        {
            return (Speaker)obj.GetValue(SpeakerProperty);
        }

        public static void SetSpeaker(DependencyObject obj, Speaker value)
        {
            obj.SetValue(SpeakerProperty, value);
        }

        // Using a DependencyProperty as the backing store for Speaker.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeakerProperty =
            DependencyProperty.RegisterAttached("Speaker", typeof(Speaker), typeof(SyncExtension), new PropertyMetadata(null));


        #endregion
    }
}
