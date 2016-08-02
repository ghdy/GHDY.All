using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace GHDY.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException("UnhandledException", e.Exception);
        }
    }
}
