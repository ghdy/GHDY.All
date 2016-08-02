using GHDY.Core.DocumentModel;
using GHDY.Core.LearningContentProviderCore;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Workflow.Recognize;
using GHDY.Workflow.WpfLibrary.UxService;
using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class DictationViewModel : BaseStateControlViewModel, INotifyDictationProgress
    {
        DMDocument _document = null;
        public DMDocument Document
        {
            get
            {
                if (this._document == null) this.Document = new DMDocument();
                return this._document;
            }
            private set
            {
                this._document = value;
                this.HighlightService.Document = this._document;
                this.NotifyPropertyChanged("Document");
            }
        }

        public Action Action_Complete { get; set; }

        public KaraokeHighlightService HighlightService { get; set; }

        public DictationViewModel(UserControl uControl)
            : base(uControl)
        {
            this.HighlightService = new KaraokeHighlightService();

        }

        public void Play(DMSentence sentence)
        {
            if (this.HighlightService.AudioPlayer == null)
                this.HighlightService.AudioPlayer = this.AudioPlayer;

            if (this.HighlightService.IsActivated == false)
                this.HighlightService.Activate();

            if (this.AudioPlayer != null)
            {
                this.AudioPlayer.PlayRange(sentence.BeginTime, sentence.EndTime);
            }
        }

        #region INotifyDictationProgress

        public void Recognized(Core.DocumentModel.DMSentence sentence, int percentage)
        {
            this.ParentWindow.Dispatcher.Invoke(new Action(() =>
            {
                var para = new DMParagraph();
                para.Inlines.Add(sentence);
                this.Document.Blocks.Add(para);
            }));
        }

        public void RecognizeCompleted()
        {
            this.ParentWindow.Dispatcher.Invoke(new Action(() =>
            {
                if (this.Action_Complete != null)
                    this.Action_Complete();
            }));
        }

        public void Exists(string filePath)
        {
            this.ParentWindow.Dispatcher.Invoke(new Action(() =>
            {
                string text = File.ReadAllText(filePath);

                var document = XamlReader.Parse(text) as DMDocument;

                this.Document = document;

                if (this.Action_Complete != null)
                    this.Action_Complete();
            }));
        }
        #endregion

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.BusyManager.NotBusy();
        }

        public override RecognizeTransition State
        {
            get { return RecognizeTransition.DictationAudio; }
        }

        public override string Title
        {
            get { return "Dictation Audio"; }
        }

        public override string Description
        {
            get { return "Recognize Audio by DictationGrammar."; }
        }

        protected override void Initialize()
        {
            this.NotifyPropertyChanged("Document");
        }

    }
}
