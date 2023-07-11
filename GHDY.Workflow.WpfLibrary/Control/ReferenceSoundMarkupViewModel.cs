using GHDY.Core;
using GHDY.Core.DocumentModel;
using GHDY.Core.DocumentModel.SyncControl.Dialog;
using GHDY.Workflow.Recognize;
using GHDY.Workflow.Recognize.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Data;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class ReferenceSoundMarkupViewModel : BaseStateControlViewModel, INotifyReferenceSoundMarkup
    {
        DMDocument _document = null;
        public DMDocument Document
        {
            get { return this._document; }
            set
            {
                this._document = value;
                this.NotifyPropertyChanged("Document");
                //this._document.FontSize = this.View.FontSize;

                Binding binding = new Binding("FontSize")
                {
                    Source = this.View
                };
                this._document.SetBinding(DMDocument.FontSizeProperty, binding);
            }
        }

        DMDocument _dictation = null;
        public DMDocument Dictation
        {
            get { return this._dictation; }
            set
            {
                this._dictation = value;
                this.NotifyPropertyChanged("Dictation");
            }
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

        public Action<DMDocument> DocumentChangedAction { get; set; }

        public ReferenceSoundMarkupViewModel(UserControl userControl)
            : base(userControl)
        {

        }

        #region Commands

        public RoutedUICommand CmdFindRefSentence { get; set; }

        private void CmdFindRefSentence_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var para in this.Document.Paragraphs)
            {
                foreach (var sen in para.Sentences)
                {
                    var text = sen.ToString();
                    if (text.Contains('"') == true || text.Contains(':') == true)
                    {
                        sen.SetValue(Selector.IsSelectedProperty, true);
                    }
                }
            }
        }


        public RoutedUICommand CmdSetRefSentence { get; set; }

        private void CmdSetRefSentence_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var collection = e.Parameter as ObservableCollection<TextElement>;
            if (collection.Count == 1 && collection.First() is DMSentence)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CmdSetRefSentence_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            var collection = e.Parameter as ObservableCollection<TextElement>;

            var sentence = collection.First() as DMSentence;
            if ((bool)sentence.GetValue(SyncExtension.IsQuateProperty) == true)
            {
                sentence.SetValue(SyncExtension.IsQuateProperty, false);
                return;
            }

            ITimelineSelector selector = null;
            if (this.Lyrics != null)
            {
                var lrcSelector = new LyricsTimelineSelector
                {
                    Lyrics = this.Lyrics
                };
                selector = lrcSelector;
            }
            else if (this.Dictation != null)
            {
                var dictationSelector = new DictationTimeLineSelector
                {
                    Dictation = this.Dictation
                };
                selector = dictationSelector;
            }

            DialogSentenceTimeRangeEditor editor = new DialogSentenceTimeRangeEditor(this.AudioPlayer, selector)
            {
                Sentence = sentence,
                WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
            };

            sentence.SetValue(SyncExtension.IsQuateProperty, true);

            sentence.SetValue(Selector.IsSelectedProperty, false);

            //CompositionContainer container = new CompositionContainer();
            //container.ComposeParts(editor, selector, this.AudioPlayer);

            editor.ShowDialog();
        }
        #endregion

        #region BaseStateControlViewModel
        public override Recognize.RecognizeTransition State
        {
            get { return RecognizeTransition.MarkupSound; }
        }

        public override string Title
        {
            get { return "Markup Reference Sounds"; }
        }

        public override string Description
        {
            get { return "Markup a part of audio which is not need Recognize by SpeechEngine."; }
        }

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.BusyManager.NotBusy();
        }

        protected override void Initialize()
        {
            this.CmdFindRefSentence = new RoutedUICommand();
            this.ParentWindow.CommandBindings.Add(new CommandBinding(this.CmdFindRefSentence, this.CmdFindRefSentence_Executed));

            this.CmdSetRefSentence = new RoutedUICommand();
            this.ParentWindow.CommandBindings.Add(new CommandBinding(this.CmdSetRefSentence, this.CmdSetRefSentence_Executed, this.CmdSetRefSentence_CanExecute));


            this.NotifyPropertyChanged("CmdFindRefSentence");
            this.NotifyPropertyChanged("CmdSetRefSentence");
        }

        public override void StateCompleted()
        {
            base.StateCompleted();
        }
        #endregion

        #region INotifyReferenceSoundMarkup
        public void NotifySyncDocument(string documentFilePath)
        {
            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                if (File.Exists(documentFilePath) == true)
                {
                    this.Document = DMDocument.Load(documentFilePath);

                    this.DocumentChangedAction?.Invoke(this.Document);
                }
            });
        }

        public void NotifyLyrics(string lrcFilePath)
        {
            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                if (File.Exists(lrcFilePath) == true)
                    this.Lyrics = new Lyrics(lrcFilePath, false);

                //var success = this.Document.AutoSetTimeLine(this.Lyrics);
            });
        }

        public void NotifyDictation(string dictationFilePath)
        {
            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                if (File.Exists(dictationFilePath) == true)
                    this.Dictation = DMDocument.Load(dictationFilePath);

            });
        }
        #endregion
    }
}
