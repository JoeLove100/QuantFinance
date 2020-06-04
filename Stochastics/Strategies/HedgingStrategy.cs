using System;
using System.Collections.Generic;
using Utilities.MarketData;

namespace Stochastics.Strategies
{
    public class HedgingStrategy
    {
        #region constructor

        public HedgingStrategy(HedgedPortfolio portfolio, 
                               string name)
        {
            Portfolio = portfolio;
            Name = name;
        }

        #endregion

        #region properties

        public string Name;
        public DateTime ExpiryDate => Portfolio.ExpiryDate;
        public readonly HedgedPortfolio Portfolio;

        #endregion

        #region overrides

        public SortedList<DateTime, ValueTuple<double, double, double>> 
            GetDailyPnl(SortedList<DateTime, OptionPricingData> marketData)
        {

            var marketDataQueue = new Queue<KeyValuePair<DateTime, OptionPricingData>>(marketData);
            var currentData = marketDataQueue.Dequeue();
            var dailyPnl = new SortedList<DateTime, ValueTuple<double, double, double>>
            {
                { currentData.Key, new ValueTuple<double, double, double>(0, 0, 0)}
            };
            var availableHistory = new SortedList<DateTime, OptionPricingData>
            {
                { currentData.Key, currentData.Value}
            };


            while (marketDataQueue.Count > 0)
            {
                var (optionsValue, hedgeValue, bankAccountValue) = Portfolio.CurrentValue(currentData.Key, availableHistory);
                var nextData = marketDataQueue.Dequeue();
                availableHistory.Add(nextData.Key, nextData.Value);

                var (newOptionsValue, newHedgeValue, newBankAccountValue) = Portfolio.NextValue(currentData.Key, nextData.Key, availableHistory,
                                                                                             hedgeValue, bankAccountValue);

                var optionChange = newOptionsValue - optionsValue;
                var hedgeChange = newHedgeValue - hedgeValue;
                var cashChange = newBankAccountValue - bankAccountValue;
                dailyPnl.Add(nextData.Key, new ValueTuple<double, double, double>(optionChange, hedgeChange, cashChange));
                currentData = nextData;
            }

            return dailyPnl;
        }

        #endregion

    }
}
