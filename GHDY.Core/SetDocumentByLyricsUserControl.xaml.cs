using GHDY.Core.DocumentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GHDY.Core
{
    public class SyncableSelectedEventArgs : EventArgs
    {
        public TimeSpan BeginTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public SyncableSelectedEventArgs(TimeSpan begin, TimeSpan end)
        {
            this.BeginTime = begin;
            this.EndTime = end;
        }
    }

    /// <summary>
    /// Interaction logic for SetDocumentByLyricsUserControl.xaml
    /// </summary>
    public partial class SetDocumentByLyricsUserControl : UserControl
    {
        ScrollViewer sv1, sv2;

        public event EventHandler<SyncableSelectedEventArgs> OnLyricsChanged = null;

        #region DP:Document

        public DMDocument Document
        {
            get { return (DMDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Document.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(DMDocument), typeof(SetDocumentByLyricsUserControl), new PropertyMetadata(null));

        #endregion

        #region DP：SentencePhrases


        public ObservableCollection<LyricsPhrase> SentencePhrases
        {
            get { return (ObservableCollection<LyricsPhrase>)GetValue(SentencePhrasesProperty); }
            set { SetValue(SentencePhrasesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SentencePhrases.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SentencePhrasesProperty =
            DependencyProperty.Register("SentencePhrases", typeof(ObservableCollection<LyricsPhrase>), typeof(SetDocumentByLyricsUserControl), new PropertyMetadata(null));

        #endregion

        #region DP:Message
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Message.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(SetDocumentByLyricsUserControl), new PropertyMetadata(""));

        #endregion

        public RoutedUICommand CmdProcessLrc { get; set; }

        public SetDocumentByLyricsUserControl()
        {
            InitializeComponent();
        }

        public SetDocumentByLyricsUserControl(DMDocument doc, ObservableCollection<LyricsPhrase> phrases)
            : this()
        {
            this.Document = doc;
            this.SentencePhrases = phrases;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.CmdProcessLrc = new RoutedUICommand();
            this.CommandBindings.Add(new CommandBinding(
                this.CmdProcessLrc,
                CmdProcessCollection_Executed,
                CmdProcessCollection_CanExecute));
            this.DataContext = this;

            sv1 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.list_LyricsSentence, 0), 0) as ScrollViewer;
            sv2 = VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(this.list_Sentences, 0), 0) as ScrollViewer;

            sv1.ScrollChanged += new ScrollChangedEventHandler(Sv1_ScrollChanged);
            sv2.ScrollChanged += new ScrollChangedEventHandler(Sv2_ScrollChanged);

            this.list_LyricsSentence.SelectionChanged += List_LyricsSentence_SelectionChanged;
            this.list_Sentences.SelectionChanged += List_Sentences_SelectionChanged;
        }


        void Sv1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sv2.ScrollToVerticalOffset(sv1.VerticalOffset);
        }

        void Sv2_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sv1.ScrollToVerticalOffset(sv2.VerticalOffset);
        }

        private void CmdProcessCollection_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.list_LyricsSentence.SelectedIndex >= 0;
        }

        private void CmdProcessCollection_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string cmd = e.Parameter.ToString().Trim();
            //ListBox target = e.Source as ListBox;
            if (this.list_LyricsSentence.SelectedItem is LyricsPhrase selectedLP)
                ProcessLyrics(cmd, selectedLP);
            else
            {
                if (this.list_Sentences.SelectedItem is DMSentence selectedSentence)
                    ProcessSentence(cmd, selectedSentence);
            }
        }

        private void ProcessSentence(string cmd, DMSentence selectedSentence)
        {
            var paragraph = selectedSentence.Paragraph;
            var newSentence = new DMSentence();
            newSentence.Initialize("(New Sentences)");

            switch (cmd)
            {
                case "Before":
                    newSentence.BeginTime = selectedSentence.BeginTime;
                    newSentence.EndTime = selectedSentence.BeginTime;
                    //this.Document.Insert(this.list_LyricsSentence.SelectedIndex, newLP);
                    paragraph.Inlines.InsertBefore(selectedSentence, newSentence);
                    break;
                case "Behind":
                    newSentence.BeginTime = selectedSentence.EndTime;
                    newSentence.EndTime = selectedSentence.EndTime;
                    //this.SentencePhrases.Insert(this.list_LyricsSentence.SelectedIndex + 1, newLP);
                    paragraph.Inlines.InsertAfter(selectedSentence, newSentence);
                    break;
                case "Delete":
                    //this.SentencePhrases.RemoveAt(this.list_LyricsSentence.SelectedIndex);
                    paragraph.Inlines.Remove(selectedSentence);
                    break;
            }
        }

        private void ProcessLyrics(string cmd, LyricsPhrase selectedLP)
        {
            var newLP = new LyricsPhrase() { Text = "(Lyrics Phrase)" };

            switch (cmd)
            {
                case "Before":
                    newLP.BeginTime = selectedLP.BeginTime;
                    newLP.EndTime = selectedLP.BeginTime;
                    this.SentencePhrases.Insert(this.list_LyricsSentence.SelectedIndex, newLP);
                    break;
                case "Behind":
                    newLP.BeginTime = selectedLP.EndTime;
                    newLP.EndTime = selectedLP.EndTime;
                    this.SentencePhrases.Insert(this.list_LyricsSentence.SelectedIndex + 1, newLP);
                    break;
                case "Delete":
                    this.SentencePhrases.RemoveAt(this.list_LyricsSentence.SelectedIndex);
                    break;
            }
        }
        
        #region On 2 List of Sentence SelectedChanged
        private void List_Sentences_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            this.list_LyricsSentence.SelectionChanged -= List_LyricsSentence_SelectionChanged;
            this.list_LyricsSentence.SelectedIndex = -1;
            this.list_LyricsSentence.SelectionChanged += List_LyricsSentence_SelectionChanged;

            var sentence = this.list_Sentences.SelectedItem as DMSentence;
            this.Message = string.Format("[Transcript] index:{0}", sentence.Index);
        }

        private void List_LyricsSentence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.list_Sentences.SelectionChanged -= List_Sentences_SelectionChanged;
            this.list_Sentences.SelectedIndex = -1;
            this.list_Sentences.SelectionChanged += List_Sentences_SelectionChanged;


            if (this.list_LyricsSentence.SelectedItem is LyricsPhrase lrcPhrase)
            {
                this.Message = string.Format("[Lyrics] << {0} ={2}= {1} >>", lrcPhrase.Begin.ToString("F2"), lrcPhrase.End.ToString("F2"), lrcPhrase.Duration.ToString("F2"));

                this.OnLyricsChanged?.Invoke(this, new SyncableSelectedEventArgs(lrcPhrase.BeginTime, lrcPhrase.EndTime));
            }
        }
        #endregion
    }
}
