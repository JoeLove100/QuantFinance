using System;
using System.Collections.Generic;
using Utilities.MarketData;
using OptionPricing;
using Utilities.ExtenstionMethods;

namespace Stochastics.Strategies
{
    public abstract class HedgedPortfolio
    {
        #region constructor

        public HedgedPortfolio(EquityOption option,
                               int numberOfContracts)
        {
            Option = option;
            _numberOfContracts = numberOfContracts;
        }

        #endregion

        #region protected fields

        public DateTime ExpiryDate => Option.ExpiryDate;
        public readonly EquityOption Option;
        protected int _numberOfContracts;

        #endregion

        #region virtual methods

        public virtual ValueTuple<double, double, double> NextValue(DateTime currentDate,
                                                                    DateTime nextDate,
                                                                    SortedList<DateTime, OptionPricingData> availableHistory,
                                                                    double hedgeValue,
                                                                    double bankAccountValue)
        {
            var timePeriod = (double)currentDate.GetWorkingDaysTo(nextDate) / TimePeriods.BusinessDaysInYear;

            var newOptionsValue = Option.GetCurrentPrice(nextDate, availableHistory) * _numberOfContracts;
            var newHedgeValue = hedgeValue * (availableHistory[nextDate].CurrentPrice / availableHistory[currentDate].CurrentPrice);
            newHedgeValue *= Math.Exp(availableHistory[currentDate].DivYield * timePeriod);  // incremental div yield
            var newBankAccountValue = bankAccountValue * Math.Exp(availableHistory[currentDate].InterestRate * timePeriod);

            return new ValueTuple<double, double, double>(newOptionsValue, newHedgeValue, newBankAccountValue);
        }

        #endregion 

        #region abstract methods

        public abstract ValueTuple<double, double, double> CurrentValue(DateTime currentDate,
                                                               SortedList<DateTime, OptionPricingData> availableHistory);

        #endregion 
    }
}
