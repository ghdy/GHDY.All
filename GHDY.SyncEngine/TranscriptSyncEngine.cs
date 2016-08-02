using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Recognition;

namespace GHDY.SyncEngine
{
    public class TranscriptSyncEngine : SyncEngineBase
    {
        public TranscriptSyncEngine()
            : this("en-US")
        {
        }

        public TranscriptSyncEngine(string cultureName)
            : base(cultureName)
        {
        }

        public TimeSpan Offset { get;private set; }

        public void SetTranscript(List<string> transcripts)
        {
            this.SetTranscript(transcripts,0,TimeSpan.Zero);
        }

        public void SetTranscript(List<string> transcripts, int beginIndex, TimeSpan offSet)
        {
            this.Offset = offSet;

            List<IndexGrammar> grammars = new List<IndexGrammar>();

            int index = beginIndex;
            transcripts.ForEach(sentenceText =>
            {
                var grammar = new IndexGrammar(index++, sentenceText, this.CultureName);

                grammars.Add(grammar);
            });

            this.SetTranscript(grammars);
        }

        private void SetTranscript(List<IndexGrammar> grammars)
        {
            this.Engine.RecognizeAsyncStop();
            this.Engine.RecognizeAsyncCancel();
            this.Engine.UnloadAllGrammars();
            Thread.Sleep(TimeSpan.FromSeconds(1.0));

            this.GrammarCollection.Clear();

            grammars.ForEach(grammar =>
            {
                this.GrammarCollection.Add(grammar);
                this.Engine.LoadGrammar(grammar);
            });
        }

        protected override void OnProcess()
        {
            this.Engine.RecognizeAsync(System.Speech.Recognition.RecognizeMode.Multiple);
        }

        public override RecognitionResult Recognize(string waveFile)
        {
            this.Engine.SetInputToWaveFile(waveFile);
            return this.Engine.Recognize();
        }
    }
}
