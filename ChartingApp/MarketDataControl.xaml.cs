using System;
using System.Windows.Controls;
using Utilities.MarketData;

namespace ChartingApp
{
    /// <summary>
    /// Interaction logic for MarketDataControl.xaml
    /// </summary>
    public partial class MarketDataControl : UserControl
    {
        #region constructor

        public MarketDataControl()
        {
            InitializeComponent();
        }

        #endregion

        #region methods

        public void SetMarketData(OptionPricingData pricingData,
                                  bool fixedData)
        {
            StartPriceDoubleBox.Value = pricingData.CurrentPrice;
            VolDoubleBox.Value = pricingData.Vol;
            InterestDoubleBox.Value = pricingData.InterestRate;
            DivYieldDoubleBox.Value = pricingData.DivYield;
            FixedDataCheckBox.IsChecked = fixedData;
        }

        public ValueTuple<OptionPricingData, bool> GetMarketData()
        {
            var startPrice = (double) StartPriceDoubleBox.Value;
            var vol = (double) VolDoubleBox.Value;
            var interestRate = (double) InterestDoubleBox.Value;
            var divYield = (double) DivYieldDoubleBox.Value;
            var fixedData = (bool) FixedDataCheckBox.IsChecked;

            var pricingData = new OptionPricingData(startPrice, vol, interestRate, divYield);
            return (pricingData, fixedData);
        }
        #endregion 
    }
}
