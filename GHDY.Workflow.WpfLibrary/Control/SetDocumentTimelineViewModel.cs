using GHDY.Core;
using GHDY.Core.DocumentModel;
using GHDY.Workflow.Recognize.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class SetDocumentTimelineViewModel : BaseStateControlViewModel, INotifySetSentenceTimeline
    {
        DMDocument _doc = null;
        public DMDocument Document
        {
            get { return this._doc; }
            set
            {
                this._doc = value;
                //this.NotifyPropertyChanged("Document");
            }
        }

        ObservableCollection<LyricsPhrase> _sentencePhrases = null;
        public ObservableCollection<LyricsPhrase> SentencePhrases
        {
            get { return this._sentencePhrases; }
            set
            {
                this._sentencePhrases = value;
                this.NotifyPropertyChanged("SentencePhrases");
                this.CheckSentence42();
                this.SentencePhrases.CollectionChanged += SentencePhrases_CollectionChanged;
            }
        }

        void SentencePhrases_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.CheckSentence42();
        }

        Lyrics _lrc = null;
        public Lyrics Lyrics
        {
            get { return this._lrc; }
            set
            {
                this._lrc = value;

                this.NotifyPropertyChanged("Lyrics");
            }
        }

        public Action<DMDocument, ObservableCollection<LyricsPhrase>> ActionSetControl { get; set; }

        public SetDocumentTimelineViewModel(UserControl uControl, Action<DMDocument, ObservableCollection<LyricsPhrase>> action)
            : base(uControl)
        {
            this.ActionSetControl = action;
        }

        #region BaseStateControlViewModel
        public override Recognize.RecognizeTransition State
        {
            get { return Recognize.RecognizeTransition.SetTimeline; }
        }

        public override string Title
        {
            get { return "Set Document Timeline"; }
        }

        public override string Description
        {
            get { return "Set each sentence's Timeline by Lyrics."; }
        }

        public override void StateLoaded()
        {
            //binding Commands
            //throw new NotImplementedException();
            this.BusyManager.NotBusy();
        }

        protected override void Initialize()
        {
            //base.StateComplete();
        }

        
        #endregion

        #region INotifySetSentenceTimeline
        public void NotifyDictation(string dictationFilePath)
        {
            //this.Dictation = DMDocument.Load(dictationFilePath);
        }

        public void NotifyLyrics(string lyricsFilePath)
        {
            this.Lyrics = new Lyrics(lyricsFilePath,true);
            //var first = this.Lyrics.Phrases.First();
            //if (first.Begin <= 0)
            //    this.Lyrics.Phrases.Remove(first);

            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                this.SentencePhrases = new ObservableCollection<LyricsPhrase>(this._lrc.Phrases);

                this.ActionSetControl(this.Document, this.SentencePhrases);

                this.CheckSentence42();
            });
        }

        private void CheckSentence42()
        {
            if (this.SentencePhrases != null &&
                this.Document.Sentences.Count() == this.SentencePhrases.Count)
            {
                this.CanSelectNextPage = true;
            }
            else
                this.CanSelectNextPage = false;
        }

        public void NotifySyncDocument(string documentstring)
        {

            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                this.Document = DMDocument.Load(documentstring);
            });
        }
        #endregion

        public override void StateCompleted()
        {
            //base.StateCompleted();
            var lyrics = new Lyrics();
            lyrics.Phrases.AddRange(this.SentencePhrases);
            this.ResumeBookmatk(this.State.ToString(), lyrics);
        }
    }
}
