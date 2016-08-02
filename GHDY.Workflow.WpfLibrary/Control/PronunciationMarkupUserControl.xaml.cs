using GHDY.Core;
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

namespace GHDY.Workflow.WpfLibrary.Control
{
    /// <summary>
    /// Interaction logic for PronunciationMarkupUserControl.xaml
    /// </summary>
    public partial class PronunciationMarkupUserControl : AutoFontSizeUserControl
    {
        public PronunciationMarkupViewModel ViewModel { get; private set; }

        public PronunciationMarkupUserControl()
        {
            this.ViewModel = new PronunciationMarkupViewModel(this);

            this.ViewModel.DocumentChangedAction = new Action<Core.DocumentModel.DMDocument>(
                (doc) =>
                {
                    this.docScrollViewer.Document = doc;
                    this.docScrollViewer.Document.FontSize = this.FontSize;
                });

            InitializeComponent();
        }

        private void AutoFontSizeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SyncableElementPlayAdapter adapter = new SyncableElementPlayAdapter(this.docScrollViewer, this.ViewModel.AudioPlayer);

        }
    }
}
