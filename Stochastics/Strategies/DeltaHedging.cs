using System;
using System.Collections.Generic;
using OptionPricing;
using Utilities.MarketData;
using Utilities.ExtenstionMethods;

namespace Stochastics.Strategies
{
    public class DeltaHedging : OptionHedgingStrategy
    {
        #region constructor

        public DeltaHedging(EquityOption option, 
                            int contractNumbers) : base(option, contractNumbers)
        {

        }

        #endregion

        #region overrides

        public override SortedList<DateTime, ValueTuple<double, double, double>> 
            GetDailyPnl(Queue<KeyValuePair<DateTime, OptionPricingData>> marketData)
        {
            var currentData = marketData.Dequeue();
            var dailyPnl = new SortedList<DateTime, ValueTuple<double, double, double>>
            {
                { currentData.Key, new ValueTuple<double, double, double>(0, 0, 0)}
            };
            var availableHistory = new SortedList<DateTime, OptionPricingData>
            {
                { currentData.Key, currentData.Value}
            };


            while (marketData.Count > 0)
            {
                var (optionsValue, hedgeValue, bankAccountValue) = GetInitialPortfolioValue(currentData.Key, availableHistory);
                var nextData = marketData.Dequeue();
                var (newOptionsValue, newHedgeValue, newBankAccountValue) = RevaluePortfolio(currentData.Key, nextData.Key, availableHistory,
                                                                                             hedgeValue, bankAccountValue);

                var optionChange = newOptionsValue - optionsValue;
                var hedgeChange = newHedgeValue - hedgeValue;
                var cashChange = newBankAccountValue - bankAccountValue;
                dailyPnl.Add(nextData.Key, new ValueTuple<double, double, double>(optionChange, hedgeChange, cashChange));
            }
            

            return dailyPnl;
        }

        #endregion

        #region private methods

        private ValueTuple<double, double, double> GetInitialPortfolioValue(DateTime currentDate, 
                                                                            SortedList<DateTime, OptionPricingData> availableHistory)
        {
            var optionsValue = _option.GetCurrentPrice(currentDate, availableHistory);
            var hedgeValue = - _option.GetCurrentDelta(currentDate, availableHistory) * _numberOfContracts;
            var bankAccountValue = -(optionsValue - hedgeValue);

            return new ValueTuple<double, double, double>(optionsValue, hedgeValue, bankAccountValue);
        }

        private ValueTuple<double, double, double> RevaluePortfolio(DateTime currentDate,
                                                                    DateTime nextDate,
                                                                    SortedList<DateTime, OptionPricingData> availableHistory,
                                                                    double hedgeValue,
                                                                    double bankAccountValue)
        {
            var timePeriod = currentDate.GetWorkingDaysTo(nextDate) / TimePeriods.BusinessDaysInYear;

            var newOptionsValue = _option.GetCurrentPrice(nextDate, availableHistory);
            var newHedgeValue = hedgeValue * (availableHistory[nextDate].CurrentPrice / availableHistory[currentDate].CurrentPrice);
            newHedgeValue *= Math.Exp(availableHistory[currentDate].DivYield * timePeriod);  // incremental div yield
            var newBankAccountValue = bankAccountValue * (Math.Exp(availableHistory[currentDate].InterestRate * timePeriod) - 1);

            return new ValueTuple<double, double, double>(newOptionsValue, newHedgeValue, newBankAccountValue);
        }

        #endregion
    }
}
