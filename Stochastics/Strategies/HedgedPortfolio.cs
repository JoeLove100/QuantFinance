using System;
using System.Collections.Generic;
using Utilities.MarketData;
using Utilities.ExtenstionMethods;
using OptionPricing;

namespace Stochastics.Strategies
{
    public class HedgedPortfolio
    {
        #region constructor

        public HedgedPortfolio(EquityOption option,
                               int numberOfContracts)
        {
            _option = option;
            _numberOfContracts = numberOfContracts;
        }

        #endregion

        #region protected fields

        protected EquityOption _option;
        protected int _numberOfContracts;

        #endregion

        #region public methods

        public ValueTuple<double, double, double> CurrentValue(DateTime currentDate,
                                                               SortedList<DateTime, OptionPricingData> availableHistory)
        {
            var optionsValue = _option.GetCurrentPrice(currentDate, availableHistory) * _numberOfContracts;
            var hedgeValue = -_option.GetCurrentDelta(currentDate, availableHistory) * _numberOfContracts * availableHistory[currentDate].CurrentPrice;
            var bankAccountValue = -(optionsValue + hedgeValue);

            return new ValueTuple<double, double, double>(optionsValue, hedgeValue, bankAccountValue);
        }

        public ValueTuple<double, double, double> NextValue(DateTime currentDate,
                                                            DateTime nextDate,
                                                            SortedList<DateTime, OptionPricingData> availableHistory,
                                                            double hedgeValue,
                                                            double bankAccountValue)
        {
            var timePeriod = (double) currentDate.GetWorkingDaysTo(nextDate) / TimePeriods.BusinessDaysInYear;

            var newOptionsValue = _option.GetCurrentPrice(nextDate, availableHistory) * _numberOfContracts;
            var newHedgeValue = hedgeValue * (availableHistory[nextDate].CurrentPrice / availableHistory[currentDate].CurrentPrice);
            newHedgeValue *= Math.Exp(availableHistory[currentDate].DivYield * timePeriod);  // incremental div yield
            var newBankAccountValue = bankAccountValue * Math.Exp(availableHistory[currentDate].InterestRate * timePeriod);

            return new ValueTuple<double, double, double>(newOptionsValue, newHedgeValue, newBankAccountValue);
        }

        #endregion 
    }
}
