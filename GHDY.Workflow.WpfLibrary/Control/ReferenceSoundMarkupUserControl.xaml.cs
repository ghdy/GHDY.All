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
    /// Interaction logic for ReferenceSoundMarkupUserControl.xaml
    /// </summary>
    public partial class ReferenceSoundMarkupUserControl : AutoFontSizeUserControl
    {
        public ReferenceSoundMarkupViewModel ViewModel { get; set; }

        public ReferenceSoundMarkupUserControl()
        {
            this.ViewModel = new ReferenceSoundMarkupViewModel(this)
            {
                DocumentChangedAction = new Action<Core.DocumentModel.DMDocument>(
                (doc) =>
                {
                    this.dmDocumentScrollViewer.Document = doc;
                }
                )
            };

            InitializeComponent();
        }

        private void AutoFontSizeUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            SyncableElementPlayAdapter adapter = new SyncableElementPlayAdapter(this.dmDocumentScrollViewer, this.ViewModel.AudioPlayer);

            
        }
    }
}
