using System.Windows;
using ChartingApp.ViewModels;

namespace ChartingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region start-up

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var MainWindow = new MainWindow();
            MainWindow.Show();
        }

        #endregion
    }
}
