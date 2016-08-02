using GHDY.Core;
using GHDY.Core.LearningContentProviderCore;
using GHDY.Workflow.Recognize;
using GHDY.Workflow.WpfLibrary.Control;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Core;

namespace GHDY.Workflow.WpfLibrary
{
    /// <summary>
    /// Interaction logic for RecognizeEpisodeWizardWindow.xaml
    /// </summary>
    public partial class RecognizeEpisodeWizardWindow : Window
    {
        public RecognizeEpisodeViewModel ViewModel { get; private set; }

        public RecognizeEpisodeWizardWindow(BaseTarget target, XEpisode episode)
        {
            this.ViewModel = new RecognizeEpisodeViewModel(target, episode, this)
            {
                RecognizeStateChanged = (st) => { SetCurrentPage(st); },
            };


            InitializeComponent();
        }

        private void SetCurrentPage(RecognizeTransition st)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                var pages = this.recognizeWizard.Items.Cast<WizardPage>();

                foreach (var page in pages)
                {
                    var uc = page.Content as UserControl;
                    if (uc != null)
                    {
                        var vm = uc.DataContext as BaseStateControlViewModel;
                        if (vm != null && vm.State == st)
                        {
                            this.recognizeWizard.CurrentPage = page;
                            this.ViewModel.SetBusy(st.ToString());
                            return;
                        }
                    }
                }
            }));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitPages();

            this.recognizeWizard.CurrentPage = this.recognizeWizard.Items[0] as WizardPage;

            this.ViewModel.RunWorkflow();
        }

        bool AllPagesLoaded()
        {
            bool result = true;
            foreach (var svm in this.ViewModel.StateControlViewModels)
            {
                if (svm.View.IsLoaded == false)
                {
                    result = false;
                    break;
                }
            }

            return result;
        }

        private void InitPages()
        {
            foreach (var viewModel in this.ViewModel.StateControlViewModels)
            {
                var wizardPage = new WizardPage()
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    PageType = WizardPageType.Interior,
                    Content = viewModel.View,
                };

                wizardPage.SetBinding(WizardPage.CanFinishProperty, new Binding("CanFinish") { Source = viewModel });
                wizardPage.SetBinding(WizardPage.CanCancelProperty, new Binding("CanCancel") { Source = viewModel });
                wizardPage.SetBinding(WizardPage.CanSelectNextPageProperty, new Binding("CanSelectNextPage") { Source = viewModel });
                wizardPage.SetBinding(WizardPage.CanSelectPreviousPageProperty, new Binding("CanSelectPreviousPage") { Source = viewModel });

                this.recognizeWizard.Items.Add(wizardPage);
            }
        }

        BaseStateControlViewModel GetStateControlViewModel(WizardPage page)
        {
            BaseStateControlViewModel result = null;
            var uControl = page.Content as UserControl;
            if (uControl != null)
                if (uControl.DataContext is BaseStateControlViewModel)
                {
                    result = uControl.DataContext as BaseStateControlViewModel;
                }
            return result;
        }

        #region Wizard Events

        private void Wizard_Finish(object sender, RoutedEventArgs e)
        {

        }

        private void Wizard_Cancel(object sender, RoutedEventArgs e)
        {

        }

        private void Wizard_PageChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = this.GetStateControlViewModel(recognizeWizard.CurrentPage);

            if (viewModel != null)
            {
                Console.WriteLine("PageChanged:" + viewModel.Title);
            }
            //this.ViewModel.SetBusy("");
        }

        private void Wizard_Next(object sender, CancelRoutedEventArgs e)
        {
            var viewModel = this.GetStateControlViewModel(recognizeWizard.CurrentPage);

            if (viewModel != null)
            {
                Console.WriteLine("Wizard_Next From:" + viewModel.Title);
                this.ViewModel.SetBusy("Processing:" + viewModel.Title);
                viewModel.StateCompleted();
            }
        }

        private void Wizard_Previous(object sender, CancelRoutedEventArgs e)
        {
            var viewModel = this.GetStateControlViewModel(recognizeWizard.CurrentPage);

            if (viewModel != null)
                Console.WriteLine("Wizard_Previous:" + viewModel.Title);
        }

        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.ViewModel != null)
                this.ViewModel.Dispose();
        }
    }
}
