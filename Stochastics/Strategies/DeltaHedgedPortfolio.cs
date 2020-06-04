using System;
using System.Collections.Generic;
using OptionPricing;
using Utilities.MarketData;

namespace Stochastics.Strategies
{
    public class DeltaHedgedPortfolio: HedgedPortfolio
    {
        #region constructor

        public DeltaHedgedPortfolio(EquityOption option,
                                    int contracts): base(option, contracts)
        {

        }

        #endregion 

        #region overrides

        public override ValueTuple<double, double, double> CurrentValue(DateTime currentDate,
                                                                        SortedList<DateTime, OptionPricingData> availableHistory)
        {
            var optionsValue = Option.GetCurrentPrice(currentDate, availableHistory) * _numberOfContracts;
            var hedgeValue = -Option.GetCurrentDelta(currentDate, availableHistory) * _numberOfContracts * availableHistory[currentDate].CurrentPrice;
            var bankAccountValue = -(optionsValue + hedgeValue);

            return new ValueTuple<double, double, double>(optionsValue, hedgeValue, bankAccountValue);
        }

        #endregion 

    }
}
