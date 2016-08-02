using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

namespace GHDY.SyncEngine
{
    public class DictationSyncEngine : SyncEngineBase
    {
        string _cultureName = "";
        string _possibleText = "";
        public DictationSyncEngine(string cultureName)
            : base(cultureName)
        {
            _cultureName = cultureName;
            ReSetGrammar();
        }

        public DictationSyncEngine(string cultureName,string possibleText)
            : base(cultureName)
        {
            _cultureName = cultureName;
            _possibleText = possibleText;
            ReSetGrammar();
        }

        //int _sIndex = 0;
        //protected override void Engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        //{
        //    base.Engine_SpeechRecognized(sender, e);

        //    IndexGrammar grammar = new IndexGrammar(this._sIndex++, e.Result.Text, _cultureName);
        //    grammar.RecoResult = e.Result;
        //    this.GrammarCollection.Add(grammar);
        //}

        public void ReSetGrammar()
        {
            //this._sIndex = 0;
            this.GrammarCollection.Clear();

            this.Engine.RecognizeAsyncCancel();
            this.Engine.SetInputToNull();
            this.Engine.UnloadAllGrammars();
            var grammar = new DictationGrammar("grammar:dictation");

            this.Engine.LoadGrammar(grammar);

            if (string.IsNullOrEmpty(this._possibleText) == false)
                grammar.SetDictationContext(this._possibleText, null);
        }

        protected override void OnProcess()
        {
            this.Engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public override RecognitionResult Recognize(string waveFile)
        {
            this.WaveFilePath = waveFile;

            this.Engine.SetInputToWaveFile(waveFile);
            var result = this.Engine.Recognize();
            
            this.Engine.SetInputToNull();

            return result;
        }
    }
}
