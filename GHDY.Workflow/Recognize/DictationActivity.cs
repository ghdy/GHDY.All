using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Xml;
using System.Windows.Markup;
using System.Windows.Documents;
using GHDY.Core;
using GHDY.Core.DocumentModel;
using GHDY.SyncEngine;
using GHDY.Workflow.Recognize.Interface;

namespace GHDY.Workflow.Recognize
{
    /// <summary>
    /// Activity based on CodeActivity<TResult>
    /// </summary>
    public sealed class DictationActivity : CodeActivity<DMDocument>, IDisposable
    {
        readonly AutoResetEvent myResetEvent = new AutoResetEvent(false);
        DMDocument myDocument = new DMDocument();

        // Define an activity input argument of type string
        [RequiredArgument]
        [DefaultValue("")]
        public InArgument<String> AudioFilePath { get; set; }

        [DefaultValue("en-US")]
        public InArgument<String> CultureName { get; set; }

        public static DictationSyncEngine SyncEngine { get; set; }

        private INotifyDictationProgress _notifyDictationProgress;
        private int _sentenceIndex;

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="context">WF context</param>
        /// <returns></returns>
        protected override DMDocument Execute(CodeActivityContext context)
        {
            this._notifyDictationProgress = context.GetExtension<INotifyDictationProgress>();

            // Obtain the runtime value of the Text input argument
            var audioFilePath = context.GetValue(this.AudioFilePath);
            var cultureName = context.GetValue(this.CultureName);

            var dictationDocumentPath = EpisodeFileTypes.DictationFile.ToFileName(audioFilePath.Substring(0, audioFilePath.Length - 4));

            if (File.Exists(dictationDocumentPath))
            {
                Console.WriteLine("Dictation Doc Exists!");
                using (Stream stream = new FileStream(dictationDocumentPath, FileMode.Open))
                {
                    myDocument = XamlReader.Load(stream) as DMDocument;
                }
                Thread.Sleep(500);
                if (this._notifyDictationProgress != null)
                    this._notifyDictationProgress.Exists(dictationDocumentPath);
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("<Dictation Start>");

                if (SyncEngine == null)
                {
                    SyncEngine = new DictationSyncEngine(cultureName);
                    SyncEngine.SentenceRecognized += SyncEngine_SentenceRecognized;
                    SyncEngine.RecognizeCompleted += SyncEngine_RecognizeCompleted;
                }
                else
                {
                    while (SyncEngine.IsBusy)
                    {
                        Thread.Sleep(500);
                    }
                }

                if (audioFilePath.EndsWith(EpisodeFileTypes.WaveFile.ToExt()))
                    audioFilePath = EpisodeFileTypes.WaveFile.ToFileName(audioFilePath.Substring(0, audioFilePath.Length - 4));

                SyncEngine.Process(audioFilePath);
                myResetEvent.WaitOne();

                myDocument.Dispatcher.Invoke(new Action(
                    delegate ()
                    {
                        var xamlString = XamlWriter.Save(myDocument);
                        File.WriteAllText(dictationDocumentPath, xamlString);
                    }
                ));

                Console.WriteLine("</Dictation Completed>");
            }
            return this.myDocument;
        }

        void SyncEngine_SentenceRecognized(object sender, SentenceRecognizedEventArgs e)
        {
            var message = $"Recognized: [{e.Sentence.Begin.TotalSeconds.ToString("F2")}, {e.Sentence.End.TotalSeconds.ToString("F2")} [{e.Sentence.Text}]]";

            Console.WriteLine(message);
            DMSentence sentence = null;

            myDocument.Dispatcher.Invoke(new Action(() =>
                {

                    sentence = new DMSentence(this._sentenceIndex);
                    this._sentenceIndex += 1;

                    foreach (var word in e.Sentence.Words)
                    {
                        sentence.Inlines.Add(new SyncableWord(word.Text) { BeginTime = word.Begin, EndTime = word.End });
                        sentence.Inlines.Add(new Run(" "));
                    }
                    sentence.Inlines.Add(new Run(". "));

                    var para = new DMParagraph();
                    para.Inlines.Add(sentence);
                    this.myDocument.Blocks.Add(para);

                    if (this._notifyDictationProgress != null)
                        this._notifyDictationProgress.Recognized(sentence.Clone() as DMSentence, this._sentenceIndex);
                }
                ));
        }

        void SyncEngine_RecognizeCompleted(object sender, System.Speech.Recognition.RecognizeCompletedEventArgs e)
        {
            if (this._notifyDictationProgress != null)
            {
                this._notifyDictationProgress.RecognizeCompleted();
            }

            if (e != null)
                myResetEvent.Set();
        }

        /// <summary>
        /// Register activity's metadata
        /// </summary>
        /// <param name="metadata"></param>
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            // Register In arguments
            var audioFilePathArg = new RuntimeArgument(nameof(AudioFilePath), typeof(String), ArgumentDirection.In);
            metadata.AddArgument(audioFilePathArg);
            metadata.Bind(this.AudioFilePath, audioFilePathArg);

            // [Text] Argument must be set
            if (this.AudioFilePath == null)
            {
                metadata.AddValidationError(
                    new System.Activities.Validation.ValidationError(
                        "[AudioFilePath] argument must be set!",
                        false,
                        nameof(AudioFilePath)));
            }

            var cultureNameArg = new RuntimeArgument(nameof(CultureName), typeof(String), ArgumentDirection.In);
            metadata.AddArgument(cultureNameArg);
            metadata.Bind(this.CultureName, cultureNameArg);

            // TODO : Add arguments ... etc ...
        }

        public void Dispose()
        {
            myResetEvent.Dispose();
        }
    }
}
