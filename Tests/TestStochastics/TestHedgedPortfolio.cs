using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stochastics.Strategies;
using OptionPricing;
using Utilities.MarketData;

namespace Tests.TestStochastics
{
    [TestClass]
    public class TestHedgedPortfolio
    {

        #region fixtures

        public HedgedPortfolio GetPortfolio()
        {
            var expiryDate = new DateTime(2020, 6, 30);
            var option = new EuropeanEquityOption(@"Test asset", expiryDate, 1000, false, "Test curve");
            var portfolio = new HedgedPortfolio(option, 3);
            return portfolio;
        }

        #endregion 

        [TestMethod]
        public void TestGetCurrentValue()
        {
            // arrange
            var portfolio = GetPortfolio();
            var currentDate = new DateTime(2020, 5, 1);
            var availableHistory = new SortedList<DateTime, OptionPricingData>
            {
                { currentDate, new OptionPricingData(1020, 0.28, 0.05, 0.02)}
            };

            // act
            var (optionValue, hedgeValue, bankAccountValue) = portfolio.CurrentValue(currentDate, availableHistory);

            // assert
            Assert.AreEqual(105.068766, optionValue, 1e-6);
            Assert.AreEqual(41.089092, hedgeValue, 1e-6);
            Assert.AreEqual(-146.157858, bankAccountValue, 1e-6);
        }

        [TestMethod]
        public void TestGetNextValue()
        {
            // arrange
            var portfolio = GetPortfolio();
            var currentDate = new DateTime(2020, 5, 1);
            var nextDate = new DateTime(2020, 5, 4);
            var availableHistory = new SortedList<DateTime, OptionPricingData>
            {
                { currentDate, new OptionPricingData(1020, 0.28, 0.05, 0.02)},
                { nextDate, new OptionPricingData(1100, 0.28, 0.05, 0.02)}
            };

            // act
            var (newOptionsValue, newHedgeValue, newBankAccountValue) = portfolio.NextValue(currentDate, nextDate, availableHistory,
                                                                                            41.08909216, -146.1578583);

            // assert
            Assert.AreEqual(37.739245, newOptionsValue, 1e-6);
            Assert.AreEqual(44.311766, newHedgeValue, 1e-6);
            Assert.AreEqual(-146.157858, newBankAccountValue, 1e-6);
        }
    }
}
