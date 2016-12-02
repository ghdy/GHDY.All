using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;

namespace GHDY.Core.DocumentModel
{
    public class DMPhrase : Span, ISyncable,ISyncableCollection
    {
        public DMPhrase()
            : base()
        {
            this.TextDecorations = System.Windows.TextDecorations.Underline;
            this.FontWeight = FontWeights.Bold;
        }

        public IEnumerable<SyncableWord> Words
        {
            get
            {
                foreach (var inline in this.Inlines)
                {
                    if (inline is SyncableWord)
                        yield return inline as SyncableWord;
                }

            }
        }

        public DMSentence Sentence
        {
            get
            {
                return this.Parent as DMSentence;
            }
        }

        #region ISyncable

        public TimeSpan BeginTime
        {
            get
            {
                return this.Words.First().BeginTime;
            }
            set
            {
                throw new Exception("Can not set.");
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                return this.Words.Last().EndTime;
            }
            set
            {
                throw new Exception("Can not set.");
            }
        }

        public double Confidence
        {
            get
            {
                int count = this.Words.Count();
                double allConfidence = 0;

                foreach (var inline in this.Inlines)
                {
                    if (inline is ISyncable)
                    {
                        allConfidence += (inline as ISyncable).Confidence;
                    }
                }
                return allConfidence / count;
            }
            set
            {
                //throw new Exception("Can not set.");
            }
        }


        public string ToSpeechText()
        {
            string result = "";
            foreach (var word in this.Words)
            {
                result += word.ToSpeechText() + " ";
            }

            return result.Trim();
        }

        public override string ToString()
        {
            
        }

        #endregion

        #region ISyncableCollection

        public T GetSyncable<T>(TimeSpan currentTimeSpan) where T : class, ISyncable
        {
            T result = null;
            foreach (var syncObj in this.Words)
            {
                if (syncObj.BeginTime <= currentTimeSpan && currentTimeSpan <= syncObj.EndTime)
                {
                    if (syncObj is T)
                    {
                        result = syncObj as T;
                        break;
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
