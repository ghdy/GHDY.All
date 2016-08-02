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
                this.NotifyPropertyChanged("SelectedParagraphIndex");
                this.NotifyPropertyChanged("CurrentParagraph");
            }
        }

        public SplitedParagraph CurrentParagraph
        {
            get
            {
                if (this.SelectedParagraphIndex >= 0 && this.SelectedParagraphIndex < this.SplitedParagraphs.Count)
                    return this.SplitedParagraphs[this.SelectedParagraphIndex];
                else
                    return null;
            }
        }

        private int _selectedSentenceIndex = -1;
        public int SelectedSentenceIndex
        {
            get { return this._selectedSentenceIndex; }
            set
            {
                this._selectedSentenceIndex = value;
            }
        }

        public SplitTranscriptViewModel(UserControl uControl)
            : base(uControl)
        {
            this.SplitedParagraphs = new ObservableCollection<SplitedParagraph>();
            this.SplitedParagraphs.CollectionChanged += SplitedParagraphs_CollectionChanged;

            this.CanCancel = true;
        }

        void SplitedParagraphs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.NotifyPropertyChanged("SplitedParagraphs");
        }

        #region Commands
        //CmdManualSplitParagraph
        RoutedUICommand _cmdManualSplitParagraph = new RoutedUICommand();
        public RoutedUICommand CmdManualSplitParagraph { get { return this._cmdManualSplitParagraph; } }

        private void CmdManualSplitParagraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var text = e.Parameter.ToString();
            if (string.IsNullOrEmpty(text) == false)
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
        RoutedUICommand _cmdDeleteParagraph = new RoutedUICommand();
        public RoutedUICommand CmdDeleteParagraph { get { return this._cmdDeleteParagraph; } }

        private void CmdDeleteParagraph_CanExecute(object sender, CanExecuteRoutedEventArgs e)
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
            this.NotifyPropertyChanged("SplitedParagraphs");

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
            SplitedDocument result = new SplitedDocument();
            foreach (var sPara in this.SplitedParagraphs)
            {
                result.Paragraphs = this.SplitedParagraphs;
            }

            this.ResumeBookmatk(this.State.ToString(), result);
        }
    }
}
