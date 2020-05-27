using System;
using System.Collections.Generic;
using Utilities.MarketData;
using OptionPricing;

namespace Stochastics.Strategies
{
    public class StopLossPortfolio : HedgedPortfolio
    {
        #region constructor

        public StopLossPortfolio(EquityOption option, 
                                 int numberOfContracts): base(option, numberOfContracts)
        {

        }

        #endregion

        #region 

        public override (double, double, double) CurrentValue(DateTime currentDate, 
                                                              SortedList<DateTime, OptionPricingData> availableHistory)
        {
            var optionValue = _option.GetCurrentPrice(currentDate, availableHistory) * _numberOfContracts;
            double hedgeValue = 0;
            if (_option.IsInTheMoney(currentDate, availableHistory))
            {
                var sharePrice = availableHistory[currentDate].CurrentPrice;
                hedgeValue = _option.IsCall ? -sharePrice : sharePrice;
                hedgeValue *= _numberOfContracts;
            }
            var bankAccountValue = -(optionValue + optionValue);
            return new ValueTuple<double, double, double>(optionValue, hedgeValue, bankAccountValue);
        }

        #endregion 
    }
}
