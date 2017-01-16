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
    /// <summary>
    /// Interaction logic for SetDocumentByLyricsUserControl.xaml
    /// </summary>
    public partial class SetDocumentByLyricsUserControl : UserControl
    {
        ScrollViewer sv1, sv2;

        public event EventHandler OnLyricsChanged = null;

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

            sv1.ScrollChanged += new ScrollChangedEventHandler(sv1_ScrollChanged);
            sv2.ScrollChanged += new ScrollChangedEventHandler(sv2_ScrollChanged);

            this.list_LyricsSentence.SelectionChanged += List_LyricsSentence_SelectionChanged;
            this.list_Sentences.SelectionChanged += List_Sentences_SelectionChanged;
        }

        private void List_Sentences_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.list_LyricsSentence.SelectedIndex = -1;
        }

        private void List_LyricsSentence_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.list_Sentences.SelectedIndex = -1;
        }

        void sv1_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            sv2.ScrollToVerticalOffset(sv1.VerticalOffset);
        }

        void sv2_ScrollChanged(object sender, ScrollChangedEventArgs e)
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
            ListBox target = e.Source as ListBox;
            var selectedLP = this.list_LyricsSentence.SelectedItem as LyricsPhrase;
            if (selectedLP != null)
                ProcessLyrics(cmd, selectedLP);
            else
            {
                var selectedSentence = this.list_Sentences.SelectedItem as DMSentence;

                if (selectedSentence != null)
                    ProcessSentence(cmd,selectedSentence);
            }
        }

        private void ProcessSentence(string cmd, DMSentence selectedSentence)
        {
            var newSentence = new DMSentence();
            newSentence.Initialize("(New Sentences)");

            switch (cmd)
            {
                case "Before":
                    newSentence.BeginTime = selectedSentence.BeginTime;
                    newSentence.EndTime = selectedSentence.BeginTime;
                    this.Document.Insert(this.list_LyricsSentence.SelectedIndex, newLP);
                    break;
                case "Behind":
                    newSentence.BeginTime = selectedSentence.EndTime;
                    newSentence.EndTime = selectedSentence.EndTime;
                    this.SentencePhrases.Insert(this.list_LyricsSentence.SelectedIndex + 1, newLP);
                    break;
                case "Delete":
                    this.SentencePhrases.RemoveAt(this.list_LyricsSentence.SelectedIndex);
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
            if (this.OnLyricsChanged != null)
                this.OnLyricsChanged(this, new EventArgs());
        }
    }
}
