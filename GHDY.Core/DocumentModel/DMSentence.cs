using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Text.RegularExpressions;
using System.Windows;
using System.Web;

namespace GHDY.Core.DocumentModel
{
    public class DMSentence : Span, ISyncable, ISyncableCollection, ICloneable
    {
        public DMSentence()
            : base()
        {

        }

        public DMSentence(int index)
        {
            this.Index = index;

        }

        public IEnumerable<ISyncable> Syncables
        {
            get
            {
                foreach (var inline in this.Inlines)
                {
                    if (inline is ISyncable)
                        yield return inline as ISyncable;
                }
            }
        }

        public int Index { get; set; }

        public string Text { get { return this.ToString(); } }

        #region ISyncable

        public TimeSpan BeginTime
        {
            get
            {
                var result = (TimeSpan)GetValue(BeginTimeProperty);
                if (result <= TimeSpan.Zero && this.Syncables.Count() > 0)
                {
                    var first = this.Syncables.First();
                    result = first.BeginTime;
                }
                return result;
            }
            set { SetValue(BeginTimeProperty, value); }
        }

        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(DMSentence));


        public TimeSpan EndTime
        {
            get
            {
                var result = (TimeSpan)GetValue(EndTimeProperty);
                if (result <= TimeSpan.Zero && this.Syncables.Count() > 0)
                {
                    var last = this.Syncables.Last();
                    result = last.EndTime;
                }
                return result;
            }
            set
            {
                SetValue(EndTimeProperty, value);
            }
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DMSentence));


        public double Confidence
        {
            get { return (double)GetValue(ConfidenceProperty); }
            set { SetValue(ConfidenceProperty, value); }
        }

        public static readonly DependencyProperty ConfidenceProperty =
            DependencyProperty.Register("Confidence", typeof(double), typeof(DMSentence), new PropertyMetadata(0.0));

        public string ToSpeechText()
        {
            if (this.Inlines.Count == 1)
            {
                if (this.Inlines.First() is Run run)
                    return run.Text;
                else
                    return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var syncObj in this.Syncables)
                {
                    sb.Append(syncObj.ToSpeechText());
                    sb.Append(" ");
                }

                return sb.ToString().Trim();
            }
        }

        #endregion

        public DMParagraph Paragraph
        {
            get
            {
                return this.Parent as DMParagraph;
            }
        }

        public override string ToString()
        {
            if (this.Inlines.Count == 1)
            {
                if (this.Inlines.First() is Run run)
                    return run.Text;
                else
                    return string.Empty;
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                foreach (var inline in this.Inlines)
                {
                    if (inline is Run run)
                    {
                        sb.Append(run.Text);
                    }
                }

                return sb.ToString();
            }
        }

        public void Initialize(string sentenceText)
        {
            this.Inlines.Clear();
            char[] chars = new char[] { ' ' };

            var formated = HttpUtility.HtmlDecode(sentenceText.Trim());
            var array = formated.Split(chars, StringSplitOptions.RemoveEmptyEntries);

            foreach (var text in array)
            {
                if (this.Inlines.Count > 0)
                    this.Inlines.Add(new Run(" "));

                WordAndP wap = ProcessWordText(text);

                var wordLength = text.Length - wap.Left.Length - wap.Right.Length;

                if (wordLength > 0 && wap.IsTranscript == true)
                {
                    if (wap.Left.Length > 0)
                    {
                        foreach (var item in wap.Left.ToCharArray())
                        {
                            this.Inlines.Add(new Run(item.ToString()));
                        }
                    }

                    this.Inlines.Add(new SyncableWord(wap.Word));

                    if (wap.Right.Length > 0)
                    {
                        foreach (var item in wap.Right.ToCharArray())
                        {
                            this.Inlines.Add(new Run(item.ToString()));
                        }
                    }
                }
                else
                {
                    this.Inlines.Add(new Run(text));
                }
            }
        }

        class WordAndP
        {
            public string Left { get; set; }
            public string Right { get; set; }
            public string Word { get; set; }
            public bool IsTranscript { get; set; }
        }

        public static char[] PunctuationArray = { ',', ':', ';', '\"', '?', '!', '.' };
        private WordAndP ProcessWordText(string text)
        {
            WordAndP result = new WordAndP();

            Regex regex = new Regex("[A-Z, a-z, 0-9,']");
            if (regex.Match(text).Success == false)
            {
                result.IsTranscript = false;

                result.Left = "";
                result.Right = "";
                result.Word = text;
            }
            else
            {
                result.IsTranscript = true;

                string before = "";
                string after = "";
                string word = "";
                foreach (var c in text.ToCharArray())
                {
                    var contains = PunctuationArray.Contains(c);

                    if (word.Length < 1)
                    {
                        if (contains)
                            before += c.ToString();
                        else
                            word += c.ToString();
                    }
                    else
                    {
                        if (contains == true)
                            after += c.ToString();
                        else
                        {
                            word += after + c.ToString();
                            after = "";
                        }
                    }
                }

                result.Left = before;
                result.Right = after;
                result.Word = word;
            }

            return result;
        }

        public DMSentence PreviousSentence
        {
            get
            {
                var prevInlinle = this.PreviousInline;
                while (prevInlinle != null)
                {
                    if (prevInlinle is DMSentence)
                        return prevInlinle as DMSentence;

                    prevInlinle = prevInlinle.PreviousInline;
                }

                if (this.Paragraph.PreviousParagraph != null)
                    return this.Paragraph.PreviousParagraph.Sentences.Last();

                return null;
            }
        }

        public DMSentence NextSentence
        {
            get
            {
                var nextInlinle = this.NextInline;
                while (nextInlinle != null)
                {
                    if (nextInlinle is DMSentence)
                        return nextInlinle as DMSentence;

                    nextInlinle = nextInlinle.NextInline;
                }

                if (this.Paragraph.NextParagraph != null)
                    return this.Paragraph.NextParagraph.Sentences.First();

                return null;
            }
        }

        #region ISyncableCollection

        public T GetSyncable<T>(TimeSpan currentTimeSpan) where T : class, ISyncable
        {
            T result = null;
            foreach (var syncObj in this.Syncables)
            {
                if (syncObj.BeginTime <= currentTimeSpan && currentTimeSpan <= syncObj.EndTime)
                {
                    if (syncObj is T)
                    {
                        result = syncObj as T;
                    }
                    else if (syncObj is ISyncableCollection)
                    {
                        result = (syncObj as ISyncableCollection).GetSyncable<T>(currentTimeSpan);
                    }

                    break;
                }
            }

            return result;
        }

        #endregion

        public object Clone()
        {
            DMSentence newSentence = new DMSentence(this.Index);

            foreach (var inline in this.Inlines)
            {
                if (inline is SyncableWord)
                {
                    var word = inline as SyncableWord;
                    newSentence.Inlines.Add(word.Clone() as SyncableWord);
                }
                else if (inline is Run run)
                    newSentence.Inlines.Add(new Run(run.Text));
            }
            newSentence.BeginTime = this.BeginTime;
            newSentence.EndTime = this.EndTime;
            newSentence.Confidence = this.Confidence;

            return newSentence;
        }
    }
}
