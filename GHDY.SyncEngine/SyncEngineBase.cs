using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;

namespace GHDY.SyncEngine
{

    public abstract class SyncEngineBase : IDisposable
    {
        public event EventHandler<RecognizeCompletedEventArgs> RecognizeCompleted = null;

        public event EventHandler<SentenceRecognizedEventArgs> SentenceRecognized = null;

        public SpeechRecognitionEngine Engine { get; private set; }

        public string WaveFilePath { get; internal set; }

        public ObservableCollection<IndexGrammar> GrammarCollection { get; private set; }

        public List<RecognizedSentence> Result { get; internal set; }

        public string CultureName { get; private set; }

        public bool IsBusy { get; private set; }

        public SyncEngineBase(string cultureName)
        {
            this.GrammarCollection = new ObservableCollection<IndexGrammar>();
            this.CultureName = cultureName;
            this.Result = new List<RecognizedSentence>();

            CultureInfo cInfo = new CultureInfo(cultureName);
            this.Engine = new SpeechRecognitionEngine(cInfo);
            
            this.Engine.SetInputToNull();

            this.Engine.RecognizeCompleted += Engine_RecognizeCompleted;
            this.Engine.SpeechRecognized += Engine_SpeechRecognized;
        }

        protected virtual void Engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Grammar is IndexGrammar indexGrammar)
            {
                indexGrammar.Enabled = false;
                indexGrammar.RecoResult = e.Result;
            }

            var srea = new SentenceRecognizedEventArgs(e);
            this.Result.Add(srea.Sentence);

            this.SentenceRecognized?.Invoke(this, srea);
        }

        void Engine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            this.IsBusy = false;

            this.RecognizeCompleted?.Invoke(this, e);
        }

        public void Process(string waveFile)
        {
            this.IsBusy = true;
            this.Result.Clear();
            this.WaveFilePath = waveFile;

            if (File.Exists(waveFile) == false)
            {
                var mp3File = waveFile.Substring(0, waveFile.Length - 3) + "mp3";
                WaveDecoder wd = new WaveDecoder();
                wd.ProcessForRecognize(mp3File, waveFile);
            }

            this.Engine.SetInputToWaveFile(this.WaveFilePath);
            OnProcess();
        }

        public abstract RecognitionResult Recognize(string waveFile);

        protected abstract void OnProcess();

        public void Dispose()
        {
            this.Engine.SpeechRecognized -= Engine_SpeechRecognized;
            this.Engine.RecognizeCompleted -= Engine_RecognizeCompleted;
            this.RecognizeCompleted = null;

            if (this.Engine != null)
            {
                this.Engine.RecognizeAsyncStop();
                this.Engine.SetInputToNull();
                this.Engine.Dispose();
                this.Engine = null;
            }
        }
    }
}
