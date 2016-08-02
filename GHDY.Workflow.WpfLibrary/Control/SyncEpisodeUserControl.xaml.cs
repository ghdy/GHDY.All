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
using GHDY.Core;

namespace GHDY.Workflow.WpfLibrary.Control
{
    /// <summary>
    /// SyncEpisodeUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SyncEpisodeUserControl : AutoFontSizeUserControl
    {

        public SyncEpisodeViewModel ViewModel { get; private set; }

        public SyncEpisodeUserControl()
        {
            this.ViewModel = new SyncEpisodeViewModel(this);
            InitializeComponent();
        }

        private void AutoFontSizeUserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
