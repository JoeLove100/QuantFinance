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
            Assert.AreEqual(103.641378, optionValue, 1e-6);
            Assert.AreEqual(1195.805281, hedgeValue, 1e-6);
            Assert.AreEqual(-1299.446659, bankAccountValue, 1e-6);
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
                                                                                            1195.805281, -1299.446659);

            // assert
            Assert.AreEqual(36.651980, newOptionsValue, 1e-6);
            Assert.AreEqual(1289.697102, newHedgeValue, 1e-6);
            Assert.AreEqual(-1299.706574, newBankAccountValue, 1e-6);
        }
    }
}
