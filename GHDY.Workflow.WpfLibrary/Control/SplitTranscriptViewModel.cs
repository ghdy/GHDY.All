using GHDY.Core;
using GHDY.Core.DocumentModel;
using GHDY.NLP;
using GHDY.Workflow.Recognize.Interface;
using GHDY.Workflow.Recognize;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GHDY.Workflow.WpfLibrary.Control
{

    public class SplitTranscriptViewModel : BaseStateControlViewModel, INotifyParagraphSplited
    {
        public ObservableCollection<SplitedParagraph> SplitedParagraphs { get; private set; }

        private int _selectedParagraphIndex = -1;
        public int SelectedParagraphIndex
        {
            get { return this._selectedParagraphIndex; }
            set
            {
                this._selectedParagraphIndex = value;
                this.NotifyPropertyChanged(nameof(SelectedParagraphIndex));
                this.NotifyPropertyChanged(nameof(CurrentParagraph));
            }
        }

        public SplitedParagraph CurrentParagraph
        {
            get
            {
                return this.SelectedParagraphIndex >= 0 && this.SelectedParagraphIndex < this.SplitedParagraphs.Count ? this.SplitedParagraphs[this.SelectedParagraphIndex] : null;
            }
        }
        public int SelectedSentenceIndex { get; set; } = -1;

        public SplitTranscriptViewModel(UserControl uControl)
            : base(uControl)
        {
            this.SplitedParagraphs = new ObservableCollection<SplitedParagraph>();
            this.SplitedParagraphs.CollectionChanged += SplitedParagraphs_CollectionChanged;

            this.CanCancel = true;
        }

        void SplitedParagraphs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.NotifyPropertyChanged(nameof(SplitedParagraphs));
        }

        #region Commands
        //CmdManualSplitParagraph
        readonly RoutedUICommand _cmdManualSplitParagraph = new RoutedUICommand();
        public RoutedUICommand CmdManualSplitParagraph { get { return this._cmdManualSplitParagraph; } }

        private static void CmdManualSplitParagraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = e.Parameter.ToString();
            if (string.IsNullOrEmpty(text))
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CmdManualSplitParagraph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var text = e.Parameter.ToString();
            var array = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var para = this.CurrentParagraph;
            para.Sentences.Clear();
            foreach (var sentence in array)
            {
                para.Sentences.Add(sentence);
            }
        }

        //CmdDeleteParagraph
        readonly RoutedUICommand _cmdDeleteParagraph = new RoutedUICommand();
        public RoutedUICommand CmdDeleteParagraph { get { return this._cmdDeleteParagraph; } }

        private static void CmdDeleteParagraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var paragraphList = e.Parameter as ListBox;
            if (paragraphList?.SelectedIndex >= 0)
            {
                e.CanExecute = true;
            }
            else
                e.CanExecute = false;
        }

        private void CmdDeleteParagraph_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var paragraphList = e.Parameter as ListBox;
            var selectedIndex = paragraphList.SelectedIndex;
            this.SplitedParagraphs.RemoveAt(selectedIndex);
        }

        #endregion

        #region INotifyParagraphSplited

        public void NotifyParagraphSplited(SplitedParagraph paragraph)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                this.SplitedParagraphs.Add(paragraph);
            }));
        }

        #endregion

        #region BaseStateControlViewModel

        public override string Title
        {
            get { return "Split Paragraphs"; }
        }

        public override string Description
        {
            get { return "Select each paragraph in List-Box, and check the splited sentences. If has error, manual split it in Text-Box."; }
        }

        public override RecognizeTransition State
        {
            get { return RecognizeTransition.Split2Sentences; }
        }

        protected override void Initialize()
        {
            //Refresh UI to display SplitedParagraphs.
            this.NotifyPropertyChanged(nameof(SplitedParagraphs));

            //AddCommandBindings
            this.ParentWindow.CommandBindings.Add(new CommandBinding(
                this.CmdManualSplitParagraph, CmdManualSplitParagraph_Executed, CmdManualSplitParagraph_CanExecute));

            this.ParentWindow.CommandBindings.Add(new CommandBinding(
                this.CmdDeleteParagraph, CmdDeleteParagraph_Executed, CmdDeleteParagraph_CanExecute));
            //this.NotifyPropertyChanged("CmdManualSplitParagraph");

        }

        public override void StateLoaded()
        {
            this.CanSelectNextPage = true;
            this.BusyManager.NotBusy();
            this.SelectedParagraphIndex = 0;
        }
        #endregion

        public override void StateCompleted()
        {
            var result = new SplitedDocument() {
                Paragraphs = this.SplitedParagraphs
            };
            //for (int i = 0; i < SplitedParagraphs.Count; i++)
            //{
            //    SplitedParagraph sPara = this.SplitedParagraphs[i];
            //    result.Paragraphs = this.SplitedParagraphs;
            //}

            this.ResumeBookmatk(this.State.ToString(), result);
        }
    }
}
