using GHDY.Core;
using GHDY.Core.AudioPlayer;
using GHDY.Core.DocumentModel;
using GHDY.Core.DocumentModel.SyncControl;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace CreateLyricsWPF
{
    public class MyAppCommands
    {
        public static RoutedUICommand CmdSetBegin = new RoutedUICommand();
        public static RoutedUICommand CmdSetEnd = new RoutedUICommand();

        public static RoutedUICommand CmdPlayBegin = new RoutedUICommand();
        public static RoutedUICommand CmdPlayEnd = new RoutedUICommand();

        public static RoutedUICommand CmdTimeUp = new RoutedUICommand();
        public static RoutedUICommand CmdTimeDown = new RoutedUICommand();
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool IsEdited { get; set; }

        private readonly MediaAudioPlayer _audioPlayer = new MediaAudioPlayer();
        public MediaAudioPlayer AudioPlayer
        {
            get
            {
                return this._audioPlayer;
            }
        }

        ObservableCollection<XSentence> _sentences = null;
        public ObservableCollection<XSentence> Sentences
        {
            get
            {
                if (this._sentences == null)
                    this._sentences = new ObservableCollection<XSentence>();
                return this._sentences;
            }
        }

        #region All FilePaths
        private string GetFilePath(string ext)
        {
            if (String.IsNullOrEmpty(this.AudioFilePath) == true)
                return "";
            else
            {
                var folder = Path.GetDirectoryName(this.AudioFilePath);
                var fileName = Path.GetFileNameWithoutExtension(this.AudioFilePath);
                return Path.Combine(folder, fileName + ext);
            }
        }

        public string DocumentFilePath
        {
            get
            {
                var ext = ".xml";
                return GetFilePath(ext);
            }
        }

        public string LyricsFilePath
        {
            get
            {

                var ext = ".lrc";
                return GetFilePath(ext);
            }
        }

        public string SrtFilePath
        {
            get
            {

                var ext = ".srt";
                return GetFilePath(ext);
            }
        }

        public string SubtitleFilePath
        {
            get
            {

                var ext = ".st";
                return GetFilePath(ext);
            }
        }

        public string TxtFilePath
        {
            get
            {

                var ext = ".txt";
                return GetFilePath(ext);
            }
        }
        #endregion

        #region AudioFilePath
        public string AudioFilePath
        {
            get { return (string)GetValue(AudioFilePathProperty); }
            set { SetValue(AudioFilePathProperty, value); }
        }

        public static readonly DependencyProperty AudioFilePathProperty =
            DependencyProperty.Register("AudioFilePath", typeof(string), typeof(MainWindow), new PropertyMetadata(new PropertyChangedCallback(AudioFilePath_Changed)));

        private static void AudioFilePath_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var mw = sender as MainWindow;

            mw.IsEdited = false;
            mw.AudioPlayer.Load(mw.AudioFilePath);

            DMDocument doc = DMDocument.Load(mw.DocumentFilePath);
            mw.Document = doc;
            mw.documentScrollViewer_dictation.Document = doc;

            XContent content;
            TextSentencesLoader loader = new TextSentencesLoader(mw.TxtFilePath);
            if (File.Exists(mw.SubtitleFilePath) == false)
            {

                content = new XContent();
                foreach (var kvp in loader.Sentences)
                {
                    content.Root.Add(new XSentence(kvp.Key, kvp.Value));
                }
            }
            else
            {
                content = new XContent(mw.SubtitleFilePath);

                //int i = 0;
                //foreach (var sentence in content.Sentences)
                //{
                //    var kv = loader.Sentences[i];

                //    sentence.Translation = kv.Value;

                //    i += 1;
                //}

                //content.Save(mw.SubtitleFilePath);
            }
            mw.ContentDocument = content;
        }
        #endregion

        #region Content


        public XContent ContentDocument
        {
            get { return (XContent)GetValue(ContentDocumentProperty); }
            set { SetValue(ContentDocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentDocument.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentDocumentProperty =
            DependencyProperty.Register("ContentDocument", typeof(XContent), typeof(MainWindow), new PropertyMetadata(null));


        #endregion

        #region Document


        public DMDocument Document
        {
            get { return (DMDocument)GetValue(DocumentProperty); }
            set { SetValue(DocumentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Document.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register("Document", typeof(DMDocument), typeof(MainWindow), new PropertyMetadata(null));


        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            this.AudioPlayer.PositionChanged += AudioPlayer_PositionChanged;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, CmdOpen_Executed));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, CmdSave_Executed, CmdSave_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs, CmdSaveAs_Executed));

            this.CommandBindings.Add(new CommandBinding(MyAppCommands.CmdSetBegin, CmdSetBegin_Executed, CmdSetBegin_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MyAppCommands.CmdSetEnd, CmdSetEnd_Executed, CmdSetEnd_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MyAppCommands.CmdPlayBegin, CmdPlayBegin_Executed));
            this.CommandBindings.Add(new CommandBinding(MyAppCommands.CmdPlayEnd, CmdPlayEnd_Executed));
            this.CommandBindings.Add(new CommandBinding(MyAppCommands.CmdTimeUp, CmdTimeUp_Executed, CmdTimeUp_CanExecute));
            this.CommandBindings.Add(new CommandBinding(MyAppCommands.CmdTimeDown, CmdTimeDown_Executed, CmdTimeDown_CanExecute));

            this.InputBindings.Add(new KeyBinding(MyAppCommands.CmdSetBegin, new KeyGesture(Key.F1)));
            this.InputBindings.Add(new KeyBinding(MyAppCommands.CmdSetEnd, new KeyGesture(Key.F2)));

            this.InputBindings.Add(new KeyBinding(MyAppCommands.CmdPlayBegin, new KeyGesture(Key.F3)));
            this.InputBindings.Add(new KeyBinding(MyAppCommands.CmdPlayEnd, new KeyGesture(Key.F4)));

            this.InputBindings.Add(new KeyBinding(MyAppCommands.CmdTimeUp, new KeyGesture(Key.F6)));
            this.InputBindings.Add(new KeyBinding(MyAppCommands.CmdTimeDown, new KeyGesture(Key.F5)));

            this.Title = "F5 is Prev, F6 is Next. Seleted textbox first.";
        }

        void AudioPlayer_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            var word = this.Document.GetSyncable<SyncableWord>(e.CurrentPosition);

            if (word != null)
            {
                word.SetIsCurrent(true);

                if (word.Sentence != null)
                    word.Sentence.SetIsCurrent(true);
            }
        }

        private void DocumentScrollViewer_dictation_SelectionChanged(object sender, EventArgs e)
        {
            ISyncable first = null, last = null;
            foreach (var element in this.documentScrollViewer_dictation.SelectedElements)
            {
                if (element is ISyncable syncObj)
                {
                    if (first == null)
                        first = syncObj;
                    last = syncObj;
                }
            }

            if (first != null && last != null)
                this.AudioPlayer.PlayRange(first.BeginTime, last.EndTime);
        }

        private void List_Sentences_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sentence = list_Sentences.SelectedItem as XSentence;
            if (sentence.EndTime > sentence.BeginTime)
            {
                this.AudioPlayer.PlayRange(sentence.BeginTime, sentence.EndTime);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.IsEdited == true)
            {
                this.CmdSave_Executed(this, null);
            }
        }

        #region Commands

        #region Play Audio


        private void CmdPlayBegin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sentence = list_Sentences.SelectedItem as XSentence;
            PlayBegin3s(sentence);
        }

        private void CmdPlayEnd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sentence = list_Sentences.SelectedItem as XSentence;
            PlayEnd3s(sentence);
        }

        #endregion

        #region Set Timeline
        private void CmdSetBegin_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            //var docViewer = e.Parameter as DMDocumentScrollViewer;
            if (list_Sentences.SelectedIndex >= 0 &&
                documentScrollViewer_dictation.SelectedElements.Count > 0)
            {
                e.CanExecute = true;
            }
        }

        private void CmdSetBegin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sentence = list_Sentences.SelectedItem as XSentence;
            sentence.BeginTime = documentScrollViewer_dictation.BeginTime;
            this.IsEdited = true;
        }

        private void CmdSetEnd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            //var docViewer = e.Parameter as DMDocumentScrollViewer;
            if (list_Sentences.SelectedIndex >= 0 &&
                documentScrollViewer_dictation.SelectedElements.Count > 0)
            {
                e.CanExecute = true;
            }
        }

        private void CmdSetEnd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sentence = list_Sentences.SelectedItem as XSentence;
            sentence.EndTime = documentScrollViewer_dictation.EndTime;
            this.IsEdited = true;
        }
        #endregion

        #region Open & Save
        private void CmdOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".wav",
                Filter = "Audio files (*.wav, .mp3)|*.wav;*.mp3"
                //图像文件(*.bmp, *.jpg)|*.bmp;*.jpg|所有文件(*.*)|*.*
            };
            if (dlg.ShowDialog() == true)
            {
                string text = dlg.FileName;

                if (text != "" || text != null)
                {
                    this.AudioFilePath = text;
                }

            }
        }

        private void CmdSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var para = e.Parameter.ToString();

            if (para == "srt")
            {
                StringBuilder sb = new StringBuilder();

                int index = 1;
                foreach (var sentence in this.ContentDocument.Sentences)
                {
                    if (sentence.BeginTime >= sentence.EndTime)
                        continue;

                    sb.AppendLine(index.ToString());

                    var timeString = string.Format("{0} --> {1}", 
                        sentence.BeginTime.ToString().Replace('.',','),
                        sentence.EndTime.ToString().Replace('.', ','));
                    sb.AppendLine(timeString);

                    sb.AppendLine(sentence.Translation);
                    sb.AppendLine();

                    index += 1;
                }

                File.WriteAllText(this.SrtFilePath,sb.ToString(),Encoding.Unicode);
            }
            else
            {
                Lyrics lrc = new Lyrics() { NeedSort = true };
                foreach (var sentence in this.ContentDocument.Sentences)
                {
                    if (sentence.BeginTime >= sentence.EndTime)
                        continue;
                    var phrase = new LyricsPhrase()
                    {
                        Text = sentence.Translation,
                        BeginTime = sentence.BeginTime,
                        EndTime = sentence.EndTime
                    };
                    lrc.Phrases.Add(phrase);
                }

                lrc.Save(this.LyricsFilePath);
            }
        }

        private void CmdSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.IsEdited;
        }

        private void CmdSave_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.ContentDocument.Save(this.SubtitleFilePath);
            this.IsEdited = false;
        }
        #endregion

        #region Set Timeline Up or Down
        private void CmdTimeUp_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (list_Sentences.SelectedItem is XSentence)
                if (this.txtBegin.IsFocused == true || this.txtEndin.IsFocused == true)
                    e.CanExecute = true;
        }

        private void CmdTimeUp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var sentence = list_Sentences.SelectedItem as XSentence;

            if (this.txtBegin.IsFocused == true)
            {
                sentence.BeginTime += TimeSpan.FromSeconds(0.1);
                this.PlayBegin3s(sentence);
            }
            else if (this.txtEndin.IsFocused == true)
            {
                sentence.EndTime += TimeSpan.FromSeconds(0.1);
                PlayEnd3s(sentence);
            }
        }

        private void CmdTimeDown_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = false;
            if (list_Sentences.SelectedItem is XSentence)
                if (this.txtBegin.IsFocused == true || this.txtEndin.IsFocused == true)
                    e.CanExecute = true;
        }

        private void CmdTimeDown_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.txtBegin.IsFocused == true)
            {
                var sentence = list_Sentences.SelectedItem as XSentence;
                sentence.BeginTime += TimeSpan.FromSeconds(-0.1);
                this.PlayBegin3s(sentence);
            }
            else if (this.txtEndin.IsFocused == true)
            {
                var sentence = list_Sentences.SelectedItem as XSentence;
                sentence.EndTime += TimeSpan.FromSeconds(-0.1);
                PlayEnd3s(sentence);
            }
        }
        #endregion

        #endregion

        private void PlayBegin3s(XSentence sentence)
        {
            var time = sentence.BeginTime.Add(TimeSpan.FromSeconds(3));
            if (sentence.EndTime < time)
                time = sentence.EndTime;

            this.AudioPlayer.PlayRange(sentence.BeginTime, time);
        }

        private void PlayEnd3s(XSentence sentence)
        {
            var time = sentence.EndTime.Add(TimeSpan.FromSeconds(-1.5));
            if (sentence.BeginTime > time)
                time = sentence.BeginTime;

            this.AudioPlayer.PlayRange(time, sentence.EndTime);
        }

        private void BtnFindDictation_Click(object sender, RoutedEventArgs e)
        {
            if (list_Sentences.SelectedItem is XSentence sentence)
            {
                var result = this.Document.GetSyncable<DMSentence>(sentence.BeginTime.Add(TimeSpan.FromSeconds(1)));
                if (result != null)
                {
                    result.BringIntoView();
                    this.AudioPlayer.PlayRange(sentence.BeginTime, sentence.EndTime);
                }
            }
            else
            {
                MessageBox.Show("Plz Selected Sentence First.");
            }
        }
    }
}
