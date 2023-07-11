using GHDY.Core;
using GHDY.Core.DocumentModel;
using GHDY.Core.DocumentModel.SyncControl;
using System;
using System.Activities;
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

namespace GHDY.Workflow.WpfLibrary.Control
{
    /// <summary>
    /// Interaction logic for DictationUserControl.xaml
    /// </summary>
    public partial class DictationUserControl : AutoFontSizeUserControl
    {
        public DictationViewModel ViewModel { get; private set; }

        public DictationUserControl()
        {
            this.ViewModel = new DictationViewModel(this)
            {
                Action_Complete = new Action(AddSelectionEvent)
            };

            this.SizeChanged += DictationUserControl_SizeChanged;

            //this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/GHDY.Core.DocumentModel.SyncControl;component/DMDocumentSelectedStyle.xaml") });

            InitializeComponent();
        }

        void DictationUserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var temp = e.NewSize.Width / 1280;
            if (e.NewSize.Width > 0)
            {
                this.FontSize = Convert.ToInt32(temp * 20);
            }
            //this.flowDocumentViewer.FontSize = this.FontSize;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = ViewModel;

            AddSelectionEvent();
        }

        private void AddSelectionEvent()
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    if (this.flowDocumentViewer.Selection != null)
                        this.flowDocumentViewer.Selection.Changed += Selection_Changed;

                }));
            }
            catch (Exception ex)
            {
                throw new Exception("Error:[DictationUserControl.AddSelectionEvent]", ex);
            }
        }

        void Selection_Changed(object sender, EventArgs e)
        {
            var sentence = GetSelectedSentence(sender as TextSelection);

            if (sentence != null)
                this.ViewModel.Play(sentence);

        }

        private DMSentence GetSelectedSentence(TextSelection selection)
        {
            if (selection == null)
                return null;

            var sender = selection.Start.Parent;

            DMSentence sentence = null;
            switch (sender.GetType().Name)
            {
                case nameof(DMSentence):
                    sentence = sender as DMSentence;
                    break;
                case nameof(SyncableWord):
                    sentence = (sender as SyncableWord).Sentence;
                    break;
                case nameof(DMParagraph):
                    sentence = (sender as DMParagraph).Sentences.First();
                    break;
                default:
                    if (sender is TextElement)
                    {
                        sentence = GetParent<DMSentence>(sender as TextElement);
                    }
                    break;

            }
            return sentence;
        }

        T GetParent<T>(TextElement element) where T : class
        {
            if (element is T)
                return element as T;
            if (element.Parent is TextElement parentElement)
            {
                return GetParent<T>(parentElement);
            }

            return null;
        }
    }
}
