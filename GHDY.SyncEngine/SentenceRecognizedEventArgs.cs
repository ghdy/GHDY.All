using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

namespace GHDY.SyncEngine
{
    public class RecognizedObject
    {
        public TimeSpan Begin { get; internal set; }
        public TimeSpan End { get; internal set; }
        public string Text { get; internal set; }
        public double Confidence { get; internal set; }
    }

    public class RecognizedWord : RecognizedObject
    {

        public RecognizedWord(TimeSpan begin, TimeSpan end, double confidence, string text)
        {
            this.Begin = begin;
            this.End = end;
            this.Confidence = confidence;
            this.Text = text;
        }
    }

    public class RecognizedSentence : RecognizedObject
    {
        public List<RecognizedWord> Words { get; private set; }

        public bool IsOffseted { get;private set; }

        public RecognizedSentence(TimeSpan begin, TimeSpan end, double confidence, string text)
        {
            this.Begin = begin;
            this.End = end;
            this.Confidence = confidence;
            this.Text = text;
            this.Words = new List<RecognizedWord>();
            this.IsOffseted = false;
        }

        public void SetOffset(TimeSpan offset)
        {
            if (this.IsOffseted == false)
            {
                this.Begin += offset;
                this.End += offset;

                foreach (var word in Words)
                {
                    word.Begin += offset;
                    word.End += offset;
                }

                this.IsOffseted = true;
            }
        }
    }

    public class SentenceRecognizedEventArgs : EventArgs
    {
        public SpeechRecognizedEventArgs SpeechRecognizedEventArgs { get; private set; }

        public RecognizedSentence Sentence { get; private set; }

        public IndexGrammar Grammar { get { return this.SpeechRecognizedEventArgs.Result.Grammar as IndexGrammar; } }

        public SentenceRecognizedEventArgs(SpeechRecognizedEventArgs e)
            : base()
        {
            this.SpeechRecognizedEventArgs = e;

            var begin = e.Result.Audio.AudioPosition;
            var end = begin + e.Result.Audio.Duration;
            var text = e.Result.Text;
            var confidence = e.Result.Confidence;

            this.Sentence = new RecognizedSentence(begin, end, confidence, text);

            if (e.Result.Words.Count < 1)
                throw new Exception("No Words.");
            foreach (var w in e.Result.Words)
            {
                var wordAudio = e.Result.GetAudioForWordRange(w, w);
                begin = wordAudio.AudioPosition + e.Result.Audio.AudioPosition;
                end = begin + wordAudio.Duration;
                text = w.Text;
                confidence = w.Confidence;

                RecognizedWord word = new RecognizedWord(begin, end, confidence, text);
                this.Sentence.Words.Add(word);
            }
        }
    }
}
