using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;

namespace GHDY.Core.DocumentModel
{
    public class DMParagraph : Paragraph, ISyncable, ISyncableCollection
    {
        public DMParagraph()
            : base()
        {

        }

        public IEnumerable<DMSentence> Sentences
        {
            get
            {
                foreach (var inline in this.Inlines)
                {
                    if (inline is DMSentence)
                        yield return inline as DMSentence;
                }
            }
        }

        public DMParagraph PreviousParagraph
        {
            get
            {
                var prevBlock = this.PreviousBlock;
                while (prevBlock != null)
                {
                    if (prevBlock is DMParagraph)
                        return prevBlock as DMParagraph;

                    prevBlock = prevBlock.PreviousBlock;
                }

                return null;
            }
        }

        public DMParagraph NextParagraph
        {
            get
            {
                var nextBlock = this.NextBlock;
                while (nextBlock != null)
                {
                    if (nextBlock is DMParagraph)
                        return nextBlock as DMParagraph;

                    nextBlock = nextBlock.NextBlock;
                }

                return null;
            }
        }

        #region ISyncable

        [NonSerialized]
        TimeSpan _begin = TimeSpan.Zero;
        public TimeSpan BeginTime
        {
            get
            {
                if (_begin == TimeSpan.Zero && this.Sentences.Count() > 0)
                {
                    var firstS = this.Sentences.First();
                    if (firstS != null)
                        return firstS.BeginTime;
                }

                return this._begin;
            }
            set { this._begin = value; }
        }

        [NonSerialized]
        TimeSpan _end = TimeSpan.Zero;
        public TimeSpan EndTime
        {
            get
            {
                if (_end == TimeSpan.Zero && this.Sentences.Count() > 0)
                {
                    var lastS = this.Sentences.Last();
                    if (lastS != null)
                        return lastS.EndTime;
                }

                return this._end;
            }
            set { this._end = value; }
        }

        public double Confidence
        {
            get;
            set;
        }

        public string ToSpeechText()
        {
            string result = "";
            foreach (var sentence in this.Sentences)
            {
                result += sentence.ToSpeechText() + " | ";
            }

            return result;
        }

        #endregion

        #region ISyncableCollection

        public T GetSyncable<T>(TimeSpan currentTimeSpan) where T : class, ISyncable
        {
            T result = null;
            foreach (var sentence in this.Sentences)
            {
                if (sentence.BeginTime <= currentTimeSpan && currentTimeSpan <= sentence.EndTime)
                {
                    if (sentence is T)
                        result = sentence as T;
                    else
                    {
                        result = sentence.GetSyncable<T>(currentTimeSpan);
                    }

                    break;
                }
            }

            return result;
        }

        #endregion
    }
}
