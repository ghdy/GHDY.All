using GHDY.Core.DocumentModel;
using GHDY.Workflow.Recognize.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using GHDY.Workflow.WpfLibrary.UxService;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using GHDY.Core;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace GHDY.Workflow.WpfLibrary.Control
{
    public class PronunciationMarkupViewModel : BaseStateControlViewModel, INotifyPronunciationMarkup
    {
        public Action<DMDocument> DocumentChangedAction { get; set; }


        DMDocument _document = null;
        public DMDocument Document
        {
            get { return this._document; }
            set
            {
                this._document = value;
                this.NotifyPropertyChanged("Document");
                //this._document.FontSize = this.View.FontSize;

                Binding binding = new Binding("FontSize");
                binding.Source = this.View;
                this._document.SetBinding(DMDocument.FontSizeProperty, binding);
            }
        }

        public PronunciationMarkupViewModel(UserControl control)
            : base(control)
        {

        }

        #region Commands

        public RoutedUICommand CmdFindSpecialPronounce { get; set; }

        private void CmdFindSpecialPronounce_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Document.Paragraphs.ToList()
                .ForEach((para) =>
                {
                    para.Sentences.ToList().ForEach((sentence) =>
                    {
                        int index = 0;
                        sentence.Syncables.ToList().ForEach((syncable) =>
                        {
                            var text = syncable.ToString();
                            //if (TextUtilities.IsMatch(text, "\\W") == false)
                            //{
                                var count = TextUtilities.Matches(text, "[A-Z]")?.Count()??0;
                                if (index == 0 && count >1)
                                {
                                    sentence.SetValue(Selector.IsSelectedProperty, true);
                                }
                            //}

                            index += 1;
                        });
                    });
                });
        }


        public RoutedUICommand CmdSetSpecialPronounce { get; set; }

        private void CmdSetSpecialPronounce_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var collection = e.Parameter as ObservableCollection<TextElement>;
            if (collection.Count == 1 && collection.First() is DMSentence)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CmdSetSpecialPronounce_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            //var collection = e.Parameter as ObservableCollection<TextElement>;

            //var sentence = collection.First() as DMSentence;
            //if ((bool)sentence.GetValue(SyncExtension.IsQuateProperty) == true)
            //{
            //    sentence.SetValue(SyncExtension.IsQuateProperty, false);
            //    return;
            //}

            //ITimelineSelector selector = null;
            //if (this.Lyrics != null)
            //{
            //    var lrcSelector = new LyricsTimelineSelector();
            //    lrcSelector.Lyrics = this.Lyrics;
            //    selector = lrcSelector;
            //}
            //else if (this.Dictation != null)
            //{
            //    var dictationSelector = new DictationTimeLineSelector();
            //    dictationSelector.Dictation = this.Dictation;
            //    selector = dictationSelector;
            //}

            //DialogSentenceTimeRangeEditor editor = new DialogSentenceTimeRangeEditor(this.AudioPlayer, selector)
            //{
            //    Sentence = sentence,
            //    WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner
            //};

            //sentence.SetValue(SyncExtension.IsQuateProperty, true);

            //sentence.SetValue(Selector.IsSelectedProperty, false);

            ////CompositionContainer container = new CompositionContainer();
            ////container.ComposeParts(editor, selector, this.AudioPlayer);

            //editor.ShowDialog();
        }
        #endregion

        #region BaseStateControlViewModel
        public override Recognize.RecognizeTransition State
        {
            get { return Recognize.RecognizeTransition.ActualPronounce; }
        }

        public override string Title
        {
            get { return "PronunciationMarkup"; }
        }

        public override string Description
        {
            get { return "Markup real Pronunciation to Word & Phrase."; }
        }

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.BusyManager.NotBusy();
        }

        protected override void Initialize()
        {
            this.CmdFindSpecialPronounce = new RoutedUICommand();
            this.ParentWindow.CommandBindings.Add(new CommandBinding(this.CmdFindSpecialPronounce, this.CmdFindSpecialPronounce_Executed));

            this.CmdSetSpecialPronounce = new RoutedUICommand();
            this.ParentWindow.CommandBindings.Add(new CommandBinding(this.CmdSetSpecialPronounce, this.CmdSetSpecialPronounce_Executed, this.CmdSetSpecialPronounce_CanExecute));


            this.NotifyPropertyChanged("CmdFindSpecialPronounce");
            this.NotifyPropertyChanged("CmdSetSpecialPronounce");
        }
        #endregion

        #region INotifyPronunciationMarkup
        public void LoadSyncDocument(string filePath)
        {

            this.ParentWindow.Dispatcher.Invoke(() =>
            {
                this.Document = DMDocument.Load(filePath);
                this.DocumentChangedAction(this.Document);

                using (CompositionContainer container = new CompositionContainer())
                {
                    KaraokeHighlightService karaokeService = new KaraokeHighlightService();

                    container.ComposeParts(this.AudioPlayer, this.Document, karaokeService);
                }
            });
        }
        #endregion
    }
}
