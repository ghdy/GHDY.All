using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;

namespace GHDY.Core.DocumentModel.SyncControl
{
    public partial class DMDocumentScrollViewer : UserControl
    {
        const string keyWordSpeechTextEditor = "WordSpeechTextEditor";

        public TimeSpan SelectionBegin
        {
            get
            {
                //if (this.SelectedElements != null)
                //{
                //    foreach (var element in this.SelectedElements)
                //    {
                //        var syncObj = element as ISyncable;
                //        if (syncObj != null)
                //            return syncObj.BeginTime;
                //    }
                //}

                //return TimeSpan.Zero;
                return this.SelectedElements?.Cast<ISyncable>()?.First()?.BeginTime ?? TimeSpan.Zero;
            }
        }

        public TimeSpan SelectionEnd
        {
            get
            {
                var last = this.SelectedElements?.Cast<ISyncable>()?.Last();
                return last == null ? TimeSpan.Zero : last.EndTime;
            }
        }

        #region DP:Document

        //public DMDocument Document
        //{
        //    get
        //    {
        //        return (DMDocument)GetValue(DocumentProperty);
        //    }
        //    set
        //    {
        //        SetValue(DocumentProperty, value);
        //    }
        //}

        //public static readonly DependencyProperty DocumentProperty =
        //    DependencyProperty.Register("Document", typeof(DMDocument), typeof(DMDocumentScrollViewer), new UIPropertyMetadata(new PropertyChangedCallback(Document_Changed)));

        //static void Document_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        //{
        //    var viewer = sender as DMDocumentScrollViewer;
        //    if (viewer == null)
        //        return;
        //    if (viewer.flowDocumentViewer.Selection != null)
        //    {
        //        viewer.flowDocumentViewer.Selection.Changed -= viewer.Selection_Changed;
        //    }

        //    viewer.flowDocumentViewer.Document = e.NewValue as DMDocument;

        //    viewer.flowDocumentViewer.Selection.Changed += viewer.Selection_Changed;
        //}

        #endregion

        #region DP : Selected Begin & End


        public TimeSpan BeginTime
        {
            get { return (TimeSpan)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BeginTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register("BeginTime", typeof(TimeSpan), typeof(DMDocumentScrollViewer), new PropertyMetadata(TimeSpan.Zero));




        public TimeSpan EndTime
        {
            get { return (TimeSpan)GetValue(EndTimeProperty); }
            set { SetValue(EndTimeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndTime.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register("EndTime", typeof(TimeSpan), typeof(DMDocumentScrollViewer), new PropertyMetadata(TimeSpan.Zero));


        #endregion

        public event EventHandler SelectionChanged;

        public FlowDocumentScrollViewer FlowDocumentScrollViewer { get { return this.flowDocumentViewer; } }

        public DMDocument Document
        {
            get
            {
                return this.flowDocumentViewer.Document as DMDocument;
            }

            set
            {
                if (this.flowDocumentViewer.Selection != null)
                {
                    this.flowDocumentViewer.Selection.Changed -= Selection_Changed;
                }

                this.flowDocumentViewer.Document = value;

                this.flowDocumentViewer.Selection.Changed += Selection_Changed;
            }
        }

        public void Selection_Changed(object sender, EventArgs e)
        {
            //this.Dely(200);
            this.SelectedElements.Cast<DependencyObject>().ToList().ForEach(
                (dpo) => {
                    SyncExtension.SetIsCurrent(dpo, false);
                });
            this.SelectedElements.Clear();

            var selection = this.flowDocumentViewer.Selection;

            TextElement first = null;
            switch (this.SelectMode)
            {
                case SyncControl.SelectMode.Word:
                    first = this.GetSelectedSyncable<SyncableWord>(selection.Start);

                    break;
                case SyncControl.SelectMode.Phrase:
                    first = this.GetSelectedSyncable<DMPhrase>(selection.Start);
                    break;
                case SyncControl.SelectMode.Sentence:
                    first = this.GetSelectedSyncable<DMSentence>(selection.Start);
                    break;
                case SyncControl.SelectMode.Paragraph:
                    first = this.GetSelectedSyncable<DMParagraph>(selection.Start);
                    break;
            }


            ISyncable begin = null, end = null;
            if (first != null)
            {
                begin = first as ISyncable;
                var next = first;

                while (next != null && next.ElementStart.CompareTo(selection.End) == -1)
                {
                    this.SelectedElements.Add(next);

                    if (next is ISyncable)
                    {
                        end = next as ISyncable;
                        this.BeginTime = begin.BeginTime;
                        this.EndTime = end.EndTime;
                    }
                    if (next is Inline)
                    {
                        next = (next as Inline).NextInline;
                    }
                    else if (next is Block)
                    {
                        next = (next as Block).NextBlock;
                    }
                    else
                        break;
                }



                this.flowDocumentViewer.Selection.Changed -= Selection_Changed;
                this.flowDocumentViewer.Selection.Select(first.ElementStart, this.SelectedElements.Last().ContentEnd);
                this.flowDocumentViewer.Selection.Changed += Selection_Changed;
            }

            if (this.SelectionChanged != null)
                this.SelectionChanged(this, new EventArgs());

            this.SelectedElements.Cast<DependencyObject>().ToList().ForEach(
                (dpo) => {
                    SyncExtension.SetIsCurrent(dpo, true);
                });
        }

        public ObservableCollection<TextElement> SelectedElements { get; private set; }

        public void Dely(int milliSeconds)
        {
            var window = Window.GetWindow(this);
            var t = DateTime.Now.AddMilliseconds(milliSeconds);
            while (DateTime.Now < t)
                DispatcherHelper.DoEvents();
        }

        private T GetSelectedSyncable<T>(TextPointer selectionStart) where T : TextElement, ISyncable
        {
            //var parentElement = selectionStart.Parent as TextElement;
            var parent = selectionStart.Parent;
            while (parent != null)
            {
                if (parent is T)
                    return (T)parent;
                else if (parent is TextElement)
                {
                    var textElement = (parent as TextElement);
                    if (textElement != null)
                        parent = textElement.Parent;
                    else
                        break;
                }
                else
                    break;
            }

            return parent as T;
        }

        #region PROPDP SelectionMode

        public SelectMode SelectMode
        {
            get { return (SelectMode)GetValue(SelectModeProperty); }
            set { SetValue(SelectModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectModeProperty =
            DependencyProperty.Register("SelectMode", typeof(SelectMode), typeof(DMDocumentScrollViewer), new UIPropertyMetadata(SelectMode.Sentence, new PropertyChangedCallback(SelectMode_Changed)));

        private static void SelectMode_Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //var syncDocViewer = sender as SyncDocumentScrollViewer;
            //if (syncDocViewer == null)
            //{
            //    MessageBox.Show("SyncDocumentScrollViewer is null.");
            //}
            //else
            //{
            //    var mode = syncDocViewer.SelectMode;
            //    var resources = syncDocViewer.Resources;

            //    if (resources.Contains(typeof(SyncableWord)) == true)
            //    {
            //        RefreshStyle<SyncableWord>(syncDocViewer);
            //    }
            //    else if (resources.Contains(typeof(DMPhrase)) == true)
            //    {
            //        RefreshStyle<DMPhrase>(syncDocViewer);
            //    }
            //    else if (resources.Contains(typeof(DMSentence)) == true)
            //    {
            //        RefreshStyle<DMSentence>(syncDocViewer);
            //    }
            //    else if (resources.Contains(typeof(DMParagraph)) == true)
            //    {
            //        RefreshStyle<DMParagraph>(syncDocViewer);
            //    }
            //    else
            //    {
            //        RefreshStyle<SyncableWord>(syncDocViewer);
            //    }
            //}
        }

        private static void RefreshStyle<T>(DMDocumentScrollViewer syncDocViewer) where T : TextElement, ISyncable
        {
            var resources = syncDocViewer.Resources;

            Type addKeyType = GetAddStyleType(syncDocViewer.SelectMode);

            Style addStyle = resources[addKeyType] as Style;

            Type delKeyType = typeof(T);
        }

        private static Type GetAddStyleType(SelectMode mode)
        {
            Type addKeyType = null;
            switch (mode)
            {
                case SelectMode.Word:
                    addKeyType = typeof(SyncableWord);
                    break;
                case SelectMode.Phrase:
                    addKeyType = typeof(DMPhrase);
                    break;
                case SelectMode.Sentence:
                    addKeyType = typeof(DMSentence);
                    break;
                case SelectMode.Paragraph:
                    addKeyType = typeof(DMParagraph);
                    break;

            }
            return addKeyType;
        }

        #endregion

        #region ctor & Loaded

        public DMDocumentScrollViewer()
        {
            this.SelectedElements = new ObservableCollection<TextElement>();

            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //binding ContextMenu
            Binding binding = new Binding("ContextMenu");
            binding.Source = this;
            this.flowDocumentViewer.SetBinding(FlowDocumentScrollViewer.ContextMenuProperty, binding);

            ////binding FontSize
            //binding = new Binding("FontSize");
            //binding.Source = this;
            //this.flowDocumentViewer.SetBinding(FlowDocumentScrollViewer.FontSizeProperty,binding);

            //init
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.KeyUp += SyncDocumentScrollViewer_KeyUp;
                window.KeyDown += SyncDocumentScrollViewer_KeyDown;
            }

            this.DataContext = this;

            //RefreshStyle<DMSentence>(this);

            this.CommandBindings.Add(new CommandBinding(ProcessCommands.MergeWords, ProcessCommands.CommandBinding_MergeWords_Executed, ProcessCommands.CommandBinding_MergeWords_CanExecuted));
            this.CommandBindings.Add(new CommandBinding(ProcessCommands.SplitPhrase, ProcessCommands.CommandBinding_SplitPhrase_Executed, ProcessCommands.CommandBinding_SplitPhrase_CanExecuted));
            this.CommandBindings.Add(new CommandBinding(ProcessCommands.EditText, ProcessCommands.CommandBinding_EditText_Executed, ProcessCommands.CommandBinding_EditText_CanExecuted));

            this.flowDocumentViewer.MouseDoubleClick += new MouseButtonEventHandler(flowDocumentViewer_MouseDoubleClick);
        }

        void flowDocumentViewer_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.SelectedElements != null && this.SelectedElements.Count == 1)
            {
                var selectedItme = this.SelectedElements.First();

                if (selectedItme is SyncableWord)
                {
                    ProcessCommands.EditText.Execute("Word", this.btnEditWordText);
                }
                else if (selectedItme is DMSentence)
                {
                    ProcessCommands.EditText.Execute("Sentence", this.btnEditSentenceText);
                }
            }
        }

        void SyncDocumentScrollViewer_KeyDown(object sender, KeyEventArgs e)
        {
            RefreshSelectMode(e);
        }

        void SyncDocumentScrollViewer_KeyUp(object sender, KeyEventArgs e)
        {
            RefreshSelectMode(e);
        }

        private void RefreshSelectMode(KeyEventArgs e)
        {
            var modifierKey = Keyboard.Modifiers;
            var key = e.Key;

            //e.Handled = true;

            var mode = modifierKey.GetSelectMode();
            this.SelectMode = mode;
        }

        #endregion
    }
}
