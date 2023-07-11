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
    /// Interaction logic for LyricsTimelineSelector.xaml
    /// </summary>
    public partial class LyricsTimelineSelector : UserControl, ITimelineSelector
    {
        public LyricsTimelineSelectorViewModel ViewModel { get; set; }



        public Lyrics Lyrics
        {
            get { return (Lyrics)GetValue(LyricsProperty); }
            set { SetValue(LyricsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Lyrics.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LyricsProperty =
            DependencyProperty.Register("Lyrics", typeof(Lyrics), typeof(LyricsTimelineSelector), new PropertyMetadata(new PropertyChangedCallback(Lyrics_Changed)));

        private static void Lyrics_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as LyricsTimelineSelector;
            selector.ViewModel = new LyricsTimelineSelectorViewModel(e.NewValue as Lyrics);
            selector.ViewModel.SelectedCollection.CollectionChanged += selector.SelectedCollection_CollectionChanged;
            if (selector.ViewModel != null)
                selector.DataContext = selector.ViewModel;
        }



        public LyricsTimelineSelector()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void SelectedCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.ViewModel.SelectedCollection.CollectionChanged -= SelectedCollection_CollectionChanged;
            this.lb_LrcList.SelectionChanged -= Lb_LrcList_SelectionChanged;

            if (this.ViewModel.SelectedCollection.Count < 1)
            {
                this.lb_LrcList.SelectedIndex = -1;
            }
            else
            {
                foreach (var item in this.ViewModel.SelectedCollection)
                {
                    this.lb_LrcList.SelectedItems.Add(item);
                }
            }

            this.lb_LrcList.ScrollIntoView(this.lb_LrcList.SelectedItem);


            this.ViewModel.SelectedCollection.CollectionChanged += SelectedCollection_CollectionChanged;
            this.lb_LrcList.SelectionChanged += Lb_LrcList_SelectionChanged;

        }

        private void Lb_LrcList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ViewModel.SelectedCollection.CollectionChanged -= SelectedCollection_CollectionChanged;
            this.ViewModel.SelectedCollection.Clear();

            foreach (var item in this.lb_LrcList.SelectedItems)
            {
                this.ViewModel.SelectedCollection.Add(item as ISyncable);
            }
            this.ViewModel.SelectedCollection.CollectionChanged += SelectedCollection_CollectionChanged;

            //notify
            if (this.SyncableObjectSelected != null && this.ViewModel.SelectedCollection.Count > 0)
            {
                this.SyncableObjectSelected(this, new TimelineEventArgs(this.ViewModel.SelectedCollection.ToList()));
            }
        }

        #region ITimelineSelector
        public event EventHandler<TimelineEventArgs> SyncableObjectSelected;

        public TimeSpan BeginTime
        {
            get
            {
                if (this.ViewModel.SelectedCollection.Count > 0)
                {
                    return this.ViewModel.SelectedCollection.First().BeginTime;
                }
                else
                    return TimeSpan.Zero;
            }
        }

        public TimeSpan EndTime
        {
            get
            {
                if (this.ViewModel.SelectedCollection.Count > 0)
                {
                    return this.ViewModel.SelectedCollection.Last().EndTime;
                }
                else
                    return TimeSpan.Zero;
            }
        }

        public bool CanSelectByTranscript
        {
            get { return true; }
        }

        public bool CanSelectByCharIndex
        {
            get { return false; }
        }

        public void SelectByTranscript(string transcript)
        {
            this.ViewModel.SelectedCollection.Clear();

            this.ViewModel.Lyrics.Phrases.ForEach((phrase) =>
            {
                int length = transcript.Length;
                if (phrase.Text.Length < length)
                    length = phrase.Text.Length;

                var result = DiffHelper.MatchString(transcript, phrase.Text);
                var percent = Utility.GetPercent(result.Same - result.Replace, length);
                if (percent > 0.8)
                {
                    this.ViewModel.SelectedCollection.Add(phrase);
                }
            });
        }

        public void SelectByCharIndex(int beginCharIndex, int endCharIndex, int allCharCount)
        {
            this.ViewModel.SelectedCollection.Clear();

            int sum = 0;
            var percent = Utility.GetPercent(beginCharIndex, allCharCount);
            this.ViewModel.Lyrics.Phrases.ForEach((phrase) =>
            {
                sum += phrase.Text.Length;
                var temp = Utility.GetPercent(sum, this.ViewModel.LyricsLength);
                if (temp > percent)
                {
                    this.ViewModel.SelectedCollection.Add(phrase);
                    return;
                }
            });
        }

        public void Select(TimeSpan time)
        {
            this.ViewModel.SelectedCollection.Clear();
            this.ViewModel.Lyrics.Phrases.ForEach((phrase) =>
            {
                if (phrase.BeginTime < time && time < phrase.EndTime)
                {
                    this.ViewModel.SelectedCollection.Add(phrase);
                    return;
                }
            });
        }

        public void Select(TimeSpan begin, TimeSpan end)
        {
            this.ViewModel.SelectedCollection.Clear();
            this.ViewModel.Lyrics.Phrases.ForEach((phrase) =>
            {
                if (TimeSpanHelper.Intersect(begin, end, phrase.BeginTime, phrase.EndTime))
                {
                    this.ViewModel.SelectedCollection.Add(phrase);
                    return;
                }
            });
        }
        #endregion
    }
}
