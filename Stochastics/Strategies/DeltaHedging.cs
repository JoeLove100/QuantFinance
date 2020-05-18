using System;
using System.Collections.Generic;
using OptionPricing;
using Utilities.MarketData;

namespace Stochastics.Strategies
{
    public class DeltaHedging : OptionHedgingStrategy
    {
        #region constructor

        public DeltaHedging(EquityOption options, 
                            int contractNumbers) : base(options, contractNumbers)
        {

        }

        #endregion

        #region overrides

        public override SortedList<DateTime, double> GetDailyPnl(SortedList<DateTime, OptionPricingData> marketData)
        {
            //var dailyPnl = new SortedList<DateTime, double>();
            //var firstDate = marketData.Keys[0];

            //var optionValue = _option.GetCurrentPrice();

            return new SortedList<DateTime, double>();
            
        }

        #endregion
    }
}
