using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows;
using GHDY.Core.DocumentModel;
using System.Xml;
using GHDY.NLP;
using System.ComponentModel.Composition;

[assembly: XmlnsDefinition("schema:GHDY.Core.DocumentModel", "GHDY.Core.DocumentModel")]
namespace GHDY.Core.DocumentModel
{
    [Export(typeof(DMDocument))]
    public class DMDocument : FlowDocument, ISyncableCollection
    {
        public IEnumerable<DMParagraph> Paragraphs
        {
            get
            {
                foreach (var block in this.Blocks)
                {
                    if (block is DMParagraph)
                        yield return block as DMParagraph;
                }
            }
        }

        public IEnumerable<DMSentence> Sentences
        {
            get
            {
                foreach (var paragraph in this.Paragraphs)
                {
                    foreach (var sentence in paragraph.Sentences)
                    {
                        yield return sentence;
                    }
                }
            }
        }

        public DMDocument()
            : base()
        {

        }

        public void Save(string filePath)
        {
            using (XmlWriter writer = XmlWriter.Create(filePath))
            {
                XamlWriter.Save(this, writer);
            }
        }

        public static DMDocument Load(string filePath)
        {
            using (XmlReader reader = XmlReader.Create(filePath))
            {
                var obj = XamlReader.Load(reader);
                return obj as DMDocument;
            }
        }

        #region ISyncableCollection

        public T GetSyncable<T>(TimeSpan currentTimeSpan) where T : class, ISyncable
        {
            T result = null;

            foreach (var paragraph in this.Paragraphs)
            {
                if (paragraph.BeginTime <= currentTimeSpan && currentTimeSpan <= paragraph.EndTime)
                {
                    if (paragraph is T)
                    {
                        result = paragraph as T;
                    }
                    else
                    {
                        result = paragraph.GetSyncable<T>(currentTimeSpan);
                    }

                    break;
                }
            }

            return result;
        }

        #endregion

        public bool AutoSetTimeLine(Lyrics lrc)
        {
            bool result = false;

            var lrcTranscript = lrc.Transcript;

            var textArray = TextUtility.DetectSentences(lrcTranscript);
            var sentenceArray = this.Sentences.ToArray();
            if (textArray.Length == sentenceArray.Length)
            {
                result = true;
                var sentenceLyrics = lrc.ToSentenceLyrics(textArray);

                for (int i = 0; i < sentenceArray.Length; i++)
                {
                    sentenceArray[i].BeginTime = sentenceLyrics.Phrases[i].BeginTime;
                    sentenceArray[i].EndTime = sentenceLyrics.Phrases[i].EndTime;
                }
            }

            return result;
        }

        public List<string> ToSpeechSentenceList()
        {
            var result = new List<string>();
            foreach (var paragraph in this.Paragraphs)
            {
                foreach (var sentence in paragraph.Sentences)
                {
                    result.Add(sentence.ToSpeechText());
                }
            }

            return result;
        }
    }
}
