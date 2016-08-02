using GHDY.Core;
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
    /// Interaction logic for InitUserControl.xaml
    /// </summary>
    public partial class InitUserControl : AutoFontSizeUserControl
    {
        public InitViewModel ViewModel { get; private set; }

        public InitUserControl()
        {
            this.ViewModel = new InitViewModel(this);
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
