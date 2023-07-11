using GHDY.Core.AudioPlayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    /// <summary>
    /// Interaction logic for DialogSentenceTimeRangeEditor.xaml
    /// </summary>
    public partial class DialogSentenceTimeRangeEditor : Window
    {
        #region BeginTime

        public TimeSpan BeginTime
        {
            get { return (TimeSpan)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(DialogSentenceTimeRangeEditor), new PropertyMetadata(new PropertyChangedCallback(BeginTime_Changed)));

        private static void BeginTime_Changed(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            DialogSentenceTimeRangeEditor sentenceEditor = dpObj as DialogSentenceTimeRangeEditor;
            sentenceEditor.timeRangeEditor.ViewModel.Begin = ((TimeSpan)e.NewValue).TotalSeconds;
            sentenceEditor.PlayBegin();
        }

        #endregion

        #region EndTime

        public TimeSpan EndTime
        {
            get { return (TimeSpan)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DialogSentenceTimeRangeEditor), new PropertyMetadata(new PropertyChangedCallback(EndTime_Changed)));

        private static void EndTime_Changed(DependencyObject dpObj, DependencyPropertyChangedEventArgs e)
        {
            DialogSentenceTimeRangeEditor sentenceEditor = dpObj as DialogSentenceTimeRangeEditor;
            sentenceEditor.timeRangeEditor.ViewModel.End = ((TimeSpan)e.NewValue).TotalSeconds;
            sentenceEditor.PlayEnd();
        }

        #endregion

        #region DP : Sentence
        public DMSentence Sentence
        {
            get { return (DMSentence)GetValue(SentenceProperty); }
            set { SetValue(SentenceProperty, value); }
        }

        public static readonly DependencyProperty SentenceProperty =
            DependencyProperty.Register("Sentence", typeof(DMSentence), typeof(DialogSentenceTimeRangeEditor), new PropertyMetadata(new PropertyChangedCallback(Sentence_Changed)));

        private static void Sentence_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DialogSentenceTimeRangeEditor editor = d as DialogSentenceTimeRangeEditor;
            if (e.NewValue is DMSentence sentence)
            {
                editor.BeginTime = sentence.BeginTime;
                editor.EndTime = sentence.EndTime;
            }
        }
        #endregion

        [Import(typeof(AudioPlayerBase))]
        public AudioPlayerBase AudioPlayer { get; set; }

        [Import(typeof(ITimelineSelector))]
        public ITimelineSelector TimeLineSelector { get; set; }

        #region Commands

        public RoutedUICommand CmdPlayBegin { get; set; }
        public RoutedUICommand CmdPlayEnd { get; set; }

        #region CmdSetBegin & CmdSetEnd
        public RoutedUICommand CmdSetBegin { get; set; }

        private void CmdSetBegin_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.TimeLineSelector.BeginTime > TimeSpan.Zero)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CmdSetBegin_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newValue = this.TimeLineSelector.BeginTime;
            if (this.EndTime <= newValue)
            {
                this.EndTime = newValue.Add(TimeSpan.FromSeconds(0.1));
            }
            this.BeginTime = newValue;

        }

        public RoutedUICommand CmdSetEnd { get; set; }

        private void CmdSetEnd_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.TimeLineSelector.EndTime > TimeSpan.Zero)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CmdSetEnd_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var newValue = this.TimeLineSelector.EndTime;
            if (this.BeginTime >= newValue)
            {
                this.BeginTime = newValue.Add(TimeSpan.FromSeconds(-0.1));
            }
            this.EndTime = newValue;
        }
        #endregion

        #region CmdConfirm & CmdCancel


        public RoutedUICommand CmdConfirm { get; set; }

        private void CmdConfirm_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.TimeLineSelector.EndTime > this.TimeLineSelector.BeginTime &&
                this.TimeLineSelector.BeginTime > TimeSpan.Zero &&
                this.TimeLineSelector.EndTime > TimeSpan.Zero)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void CmdConfirm_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Sentence.BeginTime = this.BeginTime;
            this.Sentence.EndTime = this.EndTime;
            this.Close();
        }


        public RoutedUICommand CmdCancel { get; set; }

        private void CmdCancel_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion
        #endregion

        public DialogSentenceTimeRangeEditor()
        {
            this.CmdPlayBegin = new RoutedUICommand();
            this.CmdPlayEnd = new RoutedUICommand();
            this.CmdSetBegin = new RoutedUICommand();
            this.CmdSetEnd = new RoutedUICommand();
            this.CmdConfirm = new RoutedUICommand();
            this.CmdCancel = new RoutedUICommand();
            InitializeComponent();
        }

        public DialogSentenceTimeRangeEditor(AudioPlayerBase audioPlayer, ITimelineSelector timelineSelector)
            : this()
        {
            this.AudioPlayer = audioPlayer;
            this.TimeLineSelector = timelineSelector;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

            if (this.AudioPlayer != null)
                this.AudioPlayer.PositionChanged += AudioPlayer_PositionChanged;

            this.CommandBindings.Add(new CommandBinding(this.CmdPlayBegin, new ExecutedRoutedEventHandler((obj, args) =>
            {
                var time = TimeSpan.FromSeconds(this.timeRangeEditor.ViewModel.Begin);
                this.PlayBegin(time);
            })));

            this.CommandBindings.Add(new CommandBinding(this.CmdPlayEnd, new ExecutedRoutedEventHandler((obj, args) =>
            {
                var time = TimeSpan.FromSeconds(this.timeRangeEditor.ViewModel.End);
                this.PlayEnd(time);
            })));

            this.CommandBindings.Add(new CommandBinding(this.CmdSetBegin, CmdSetBegin_Executed, CmdSetBegin_CanExecute));
            this.CommandBindings.Add(new CommandBinding(this.CmdSetEnd, CmdSetEnd_Executed, CmdSetEnd_CanExecute));

            this.CommandBindings.Add(new CommandBinding(this.CmdConfirm, CmdConfirm_Executed, CmdConfirm_CanExecute));
            this.CommandBindings.Add(new CommandBinding(this.CmdCancel, CmdCancel_Executed));

            if (this.TimeLineSelector != null)
            {
                this.TimeLineSelector.SyncableObjectSelected += TimeLineSelector_SyncableObjectSelected;
            }

            if (this.TimeLineSelector is Control)
            {
                this.selectorContainer.Content = this.TimeLineSelector as Control;
            }

            this.timeRangeEditor.ViewModel.Minimum = 0;
            this.timeRangeEditor.ViewModel.Maximum = this.AudioPlayer.Length.TotalSeconds;
            this.timeRangeEditor.ViewModel.TimeRangeChanged += TimeRangeEditor_TimeRangeChanged;

        }

        void TimeRangeEditor_TimeRangeChanged(object sender, TimeRangeChangedEventArgs e)
        {
            if (e.IsBeginChanged == true)
                this.PlayBegin(e.Begin);
            else
                this.PlayEnd(e.End);
        }

        void TimeLineSelector_SyncableObjectSelected(object sender, TimelineEventArgs e)
        {
            this.AudioPlayer.PlayRange(e.SyncableObjects.First().BeginTime, e.SyncableObjects.Last().EndTime);
        }

        #region Play Method
        private void PlayBegin()
        {
            this.PlayBegin(this.BeginTime);
        }

        private void PlayEnd()
        {
            this.PlayEnd(this.EndTime);
        }

        private void PlayBegin(TimeSpan time)
        {
            if (this.AudioPlayer != null && time > TimeSpan.Zero)
            {
                TimeSpan end = time.Add(TimeSpan.FromSeconds(3));

                this.AudioPlayer.PlayRange(time, end);
            }
        }

        private void PlayEnd(TimeSpan time)
        {
            TimeSpan begin = time.Add(TimeSpan.FromSeconds(-3));
            if (this.AudioPlayer != null && time > TimeSpan.Zero)
            {
                if (time - this.BeginTime < TimeSpan.FromSeconds(3))
                {
                    this.AudioPlayer.PlayRange(this.BeginTime, time);
                }
                else
                    this.AudioPlayer.PlayRange(begin, time);
            }
        }
        #endregion

        void AudioPlayer_PositionChanged(object sender, PositionChangedEventArgs e)
        {

        }

        private void Btn_AutoFind_Click(object sender, RoutedEventArgs e)
        {
            if (this.TimeLineSelector.CanSelectByTranscript == true)
                this.TimeLineSelector.SelectByTranscript(this.Sentence.ToString());
            else
            {
                DMDocument doc = this.Sentence.Paragraph.Parent as DMDocument;
                var tmp = 0;
                var all = 0;
                foreach (var sentence in doc.Sentences)
                {
                    var length = sentence.ToString().Length;
                    if (sentence != this.Sentence)
                        tmp += length;
                    all += length;
                }
                this.TimeLineSelector.SelectByCharIndex(tmp + 1, tmp + 1 + this.Sentence.ToString().Length, all);
            }
        }
    }
}