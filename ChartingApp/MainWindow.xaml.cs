using System;
using System.Collections.Generic;
using System.Windows;
using ChartingApp.ViewModels;
using OptionPricing;
using Stochastics;
using Stochastics.Strategies;
using Utilities.MarketData;

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
            InitializeComponent();
            InitialiseControls();
            InitialiseViewModel();
            AddEventHandlers();

            SetViewModel();

        }

        #endregion

        #region properties

        public EquityOptionHedgingVM ViewModel;

        #endregion 

        #region methods

        private void SetViewModel()
        {

            EquityOption option = OptionDefinition.GetOption();
            (OptionPricingData pricingData, bool fixedData) = MarketDataDefinition.GetMarketData();

            var strategies = new List<HedgingStrategy>()
            {
                new HedgingStrategy(new DeltaHedgedPortfolio(option, 1), "Delta hedging"),
                new HedgingStrategy(new StopLossPortfolio(option, 1), "Stop-loss hedging")
            };


            ViewModel.PlotDailyPnL(new DateTime(2020, 1, 1), pricingData, strategies);
        }

        private void InitialiseControls()
        {
            OptionDefinition.SetOption(OptionType.EuropeanCall, 60, DateTime.Now);
            MarketDataDefinition.SetMarketData(new OptionPricingData(60, 0.2, 0.04, 0.02), false);
        }

        private void InitialiseViewModel()
        {
            Sampler sampler = new Sampler();
            BasicStochasticEngine stochasticEngine = new BasicStochasticEngine(sampler);
            ViewModel = new EquityOptionHedgingVM(stochasticEngine);
            DataContext = ViewModel;
        }

        #endregion

        #region Event Handling

        private void AddEventHandlers()
        {
            RecalcButton.Click += OnRefreshClicked;
        }

        private void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            SetViewModel();
        }

        #endregion
    }
}
