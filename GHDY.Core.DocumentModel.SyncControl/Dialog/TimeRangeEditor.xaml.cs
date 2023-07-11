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
    /// Interaction logic for TimeRangeEditor.xaml
    /// </summary>
    public partial class TimeRangeEditor : UserControl
    {
        public TimeRangeEditorViewModel ViewModel { get; private set; }
        public Window ParentWindow { get; private set; }

        readonly Key _stepKey = Key.LeftShift;

        public TimeRangeEditor()
        {
            this.ViewModel = new TimeRangeEditorViewModel();
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this.ViewModel;

            this.ParentWindow = Window.GetWindow(this);// this.GetParent<Window>();

            if (this.ParentWindow != null)
            {
                this.ParentWindow.KeyDown += ParentWindow_KeyDown;
                this.ParentWindow.KeyUp += ParentWindow_KeyUp;

                this.ViewModel.Init(this.ParentWindow);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ViewModel.Dispose();
        }

        void ParentWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyUp(this._stepKey) == true)
            {
                if (this.ViewModel.Step != 0.1)
                    this.ViewModel.Step = 0.1;
            }
            else
            {
                if (this.ViewModel.Step != 0.2)
                    this.ViewModel.Step = 0.2;
            }
        }

        void ParentWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(this._stepKey) == true)
            {
                if (this.ViewModel.Step != 0.2)
                    this.ViewModel.Step = 0.2;
            }
            else
            {
                if (this.ViewModel.Step != 0.1)
                    this.ViewModel.Step = 0.1;
            }
        }

        private void TimelineSlider_HigherValueChanged(object sender, RoutedEventArgs e)
        {
            //if (this.ViewModel.Begin != timelineSlider.LowerValue)
            //    this.ViewModel.Begin = timelineSlider.LowerValue;
        }

        private void TimelineSlider_LowerValueChanged(object sender, RoutedEventArgs e)
        {
            //if (this.ViewModel.End != timelineSlider.HigherValue)
            //    this.ViewModel.End= timelineSlider.HigherValue;
        }
    }
}
