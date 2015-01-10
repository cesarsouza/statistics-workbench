using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Workbench.ViewModels;

namespace Workbench
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Create the main view model for the application
            MainViewModel mainViewModel = new MainViewModel();

            MainWindow wnd = new MainWindow()
            {
                DataContext = mainViewModel
            };

            wnd.Show();
        }
    }
}
