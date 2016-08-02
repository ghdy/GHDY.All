using GHDY.Core;
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

namespace GHDY.Workflow.WpfLibrary.Control
{
    /// <summary>
    /// Interaction logic for SetDocumentTimelineUserControl.xaml
    /// </summary>
    public partial class SetDocumentTimelineUserControl : AutoFontSizeUserControl
    {
        public SetDocumentTimelineViewModel ViewModel { get; set; }

        public SetDocumentTimelineUserControl()
        {
            this.ViewModel = new SetDocumentTimelineViewModel(this,this.InitControl);
            //this.DataContext = this.ViewModel;
            InitializeComponent();
        }

        private void InitControl(DMDocument doc, ObservableCollection<LyricsPhrase> phraseCollection)
        {
            this.setDocByLrcUserControl.Document = doc;
            this.setDocByLrcUserControl.SentencePhrases = phraseCollection;
        }

        private void AutoFontSizeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
