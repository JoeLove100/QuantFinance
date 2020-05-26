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
using ChartingApp.ViewModels;
using OptionPricing;
using Stochastics;
using Stochastics.Strategies;

namespace ChartingApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region constructor

        public MainWindow()
        {
            SetViewModel();
            DataContext = ViewModel;

            InitializeComponent();

        }

        #endregion

        #region properties

        public EquityOptionHedgingVM ViewModel;

        #endregion 

        #region methods

        private void SetViewModel()
        {
            var expiryDate = new DateTime(2020, 3, 31);
            EuropeanEquityOption option = new EuropeanEquityOption("Test underlying", expiryDate, 50, true, "Test curve");
            BasicStochasticEngine stochasticEngine = new BasicStochasticEngine(new Sampler(1234));
            DeltaHedging hedgingStrategy = new DeltaHedging(option, 1);
            ViewModel = new EquityOptionHedgingVM(option, stochasticEngine, hedgingStrategy);

            ViewModel.PlotDailyPnL(new DateTime(2020, 1, 1), 45, 0.2, 0.04, 0.02);
        }

        #endregion 
    }
}
