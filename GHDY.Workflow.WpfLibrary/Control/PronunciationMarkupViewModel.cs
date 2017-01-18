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
using GHDY.Core.DocumentModel.SyncControl;
using GHDY.Core.DocumentModel.SyncControl.Dialog;
using System.Windows;

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
                        foreach (var syncable in sentence.Syncables)
                        {
                            var text = syncable.ToString();
                            var isSpecialPronunciation = text.IsSpecialPronunciation(index == 0);

                            if (isSpecialPronunciation == true)
                            {
                                sentence.SetValue(Selector.IsSelectedProperty, true);
                                break;
                            }
                            index += 1;
                        }
                    });
                });
        }


        public RoutedUICommand CmdSetSpecialPronounce { get; set; }

        private void CmdSetSpecialPronounce_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;

            var viewer = e.Parameter as DMDocumentScrollViewer;
            if (viewer == null) return;

            var collection = viewer.SelectedElements;

            if (collection.Count == 1 && collection.First() is ISyncable)
                e.CanExecute = true;
        }

        private void CmdSetSpecialPronounce_Executed(object sender, ExecutedRoutedEventArgs e)
        {

            var viewer = e.Parameter as DMDocumentScrollViewer;
            if (viewer == null) return;

            var syncable = viewer.SelectedElements.First() as ISyncable;
            var dpoSync = syncable as DependencyObject;
            var sentence = syncable.GetParent<DMSentence>();

            DialogSpeechTextEditor editorDialog = new DialogSpeechTextEditor(sentence,
                new Action<string>(
                    (speechText) =>
                    {
                        var dpo = syncable as DependencyObject;
                        if (dpo != null)
                            SyncExtension.SetSpeechText(dpo, speechText);
                    }));

            editorDialog.ShowDialog();

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
