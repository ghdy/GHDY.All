using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Web;

namespace GHDY.Core.DocumentModel
{
    public class SyncableWord : Run, ISyncable, ICloneable
    {
        public SyncableWord(string text)
            : base(text)
        {
            
        }

        public SyncableWord()
            : base()
        {

        }

        #region ISyncable

        public TimeSpan BeginTime
        {
            get { return (TimeSpan)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(SyncableWord), new UIPropertyMetadata(TimeSpan.Zero));


        public TimeSpan EndTime
        {
            get { return (TimeSpan)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(SyncableWord), new UIPropertyMetadata(TimeSpan.Zero));


        public double Confidence
        {
            get { return (double)GetValue(ConfidenceProperty); }
            set { SetValue(ConfidenceProperty, value); }
        }

        public static readonly DependencyProperty ConfidenceProperty =
            DependencyProperty.Register("Confidence", typeof(double), typeof(SyncableWord), new PropertyMetadata(0.0));


        public string ToSpeechText()
        {
            string result = this.GetValue(SyncExtension.SpeechTextProperty).ToString();

            if (string.IsNullOrEmpty(result) == true)
                return this.Text;
            else
                return result;
        }

        #endregion

        public TimeSpan Duration
        {
            get
            {
                return this.EndTime - this.BeginTime;
            }
        }

        public bool ParentIsSentence
        {
            get
            {
                return this.Parent is DMSentence;
            }
        }

        public DMSentence Sentence
        {
            get
            {
                if (!(this.Parent is DMSentence sentence))
                    sentence = this.Phrase.Sentence;

                return sentence;
            }
        }

        public bool ParentIsPhrase
        {
            get
            {
                return this.Parent is DMPhrase;
            }
        }

        public DMPhrase Phrase
        {
            get
            {
                return this.Parent as DMPhrase;
            }
        }

        public override string ToString()
        {
            return this.Text;
        }

        public object Clone()
        {
            SyncableWord word = new SyncableWord(this.Text)
            {
                BeginTime = this.BeginTime,
                EndTime = this.EndTime,
                Confidence = this.Confidence
            };

            return word;
        }
    }
}
