using System;
using System.Collections.Generic;
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

namespace GHDY.Core.DocumentModel.SyncControl.Dialog
{
    /// <summary>
    /// Interaction logic for DictationTimeLineSelector.xaml
    /// </summary>
    public partial class DictationTimeLineSelector : UserControl, ITimelineSelector
    {
        public DMDocument Dictation
        {
            get { return (DMDocument)GetValue(DictationProperty); }
            set { SetValue(DictationProperty, value); }
        }

        public static readonly DependencyProperty DictationProperty =
            DependencyProperty.Register("Dictation", typeof(DMDocument), typeof(DictationTimeLineSelector), new PropertyMetadata(new PropertyChangedCallback(Dictation_Changed)));

        private static void Dictation_Changed(DependencyObject dObj, DependencyPropertyChangedEventArgs e)
        {
            var dictation = e.NewValue as DMDocument;
            var selector = dObj as DictationTimeLineSelector;
            selector._dictationLength = 0;
            selector.flowDocumentScrollViewer.Document = dictation;
        }

        int _dictationLength = 0;
        public int DictationLength
        {
            get
            {
                if (this._dictationLength < 1)
                {
                    foreach (var sentence in this.Dictation.Sentences)
                    {
                        this._dictationLength += sentence.ToString().Length;
                    }
                }

                return this._dictationLength;
            }
        }

        public DictationTimeLineSelector()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.flowDocumentScrollViewer.Selection.Changed += Selection_Changed;
        }

        void Selection_Changed(object sender, EventArgs e)
        {
            var start = this.flowDocumentScrollViewer.Selection.Start;
            var end = this.flowDocumentScrollViewer.Selection.End;

            var startWord = start.GetSelectedSyncable<SyncableWord>();
            if (startWord == null)
                return;
            //var endWord = end.GetSelectedSyncable<SyncableWord>();
            this.SyncableObjectSelected?.Invoke(this, new TimelineEventArgs(startWord));
        }

        #region ITimelineSelector
        public event EventHandler<TimelineEventArgs> SyncableObjectSelected;


        public TimeSpan BeginTime
        {
            get
            {
                if (this.flowDocumentScrollViewer.Selection != null)
                {
                    var word = this.flowDocumentScrollViewer.Selection.Start.GetSelectedSyncable<SyncableWord>();
                    if (word != null)
                        return word.BeginTime;
                }

                return TimeSpan.Zero;
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                if (this.flowDocumentScrollViewer.Selection != null)
                {
                    var word = this.flowDocumentScrollViewer.Selection.End.GetSelectedSyncable<SyncableWord>();
                    if (word != null)
                        return word.EndTime;
                }

                return TimeSpan.Zero;
            }
        }

        public bool CanSelectByTranscript
        {
            get { return false; }
        }

        public bool CanSelectByCharIndex
        {
            get { return true; }
        }

        public void SelectByTranscript(string transcript)
        {
            MessageBox.Show("DictationTimeLineSelector can't SelectByTranscript.");
        }

        public void SelectByCharIndex(int beginCharIndex, int endCharIndex, int allCharCount)
        {
            int sum = 0;
            var percent = Utility.GetPercent(beginCharIndex, allCharCount);
            foreach (var sentence in this.Dictation.Sentences)
            {
                sum += sentence.ToString().Length;
                var temp = Utility.GetPercent(sum, this.DictationLength);
                if (temp > percent)
                {
                    this.SelectElement(sentence);
                    return;
                }
            }
        }

        public void Select(TimeSpan time)
        {
            foreach (var sentence in this.Dictation.Sentences)
            {
                if (sentence.ContainsTimeSpan(time) == true)
                {
                    this.SelectElement(sentence);
                    return;
                }
            }
        }

        public void Select(TimeSpan begin, TimeSpan end)
        {
            DMSentence first = null;
            DMSentence last = null;
            foreach (var sentence in this.Dictation.Sentences)
            {
                if (TimeSpanHelper.Intersect(begin, end, sentence.BeginTime, sentence.EndTime))
                {
                    if (first == null)
                        first = sentence;
                    last = sentence;
                }
                else if (first != null)
                    break;
            }

            if (first != null && last != null)
                this.SelectElement(first, last);
        }
        #endregion

        private void SelectElement(DMSentence textElement)
        {
            this.SelectElement(textElement, textElement);
        }

        private void SelectElement(DMSentence begin, DMSentence end)
        {
            this.flowDocumentScrollViewer.Selection.Changed -= Selection_Changed;

            this.flowDocumentScrollViewer.Selection.Select(begin.ContentStart, end.ContentEnd);
            begin.BringIntoView();

            this.SyncableObjectSelected?.Invoke(this, new TimelineEventArgs(begin));

            this.flowDocumentScrollViewer.Selection.Changed += Selection_Changed;
        }
    }
}
