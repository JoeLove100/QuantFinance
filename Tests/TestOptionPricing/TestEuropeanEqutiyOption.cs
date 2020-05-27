using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionPricing;
using Utilities.MarketData;
using Utilities.ExtenstionMethods;

namespace Tests.TestOptionPricing
{
    /// <summary>
    /// Summary description for TestEqutiyOption
    /// </summary>
    [TestClass]
    public class TestEuropeanEqutiyOption
    {
        #region fixtures

        private static EuropeanEquityOption GetOption(bool isCall,
                                                      double strike = 50)
        {
            var expiryDate = new DateTime(2020, 9, 30);
            return new EuropeanEquityOption("Test index", expiryDate, strike, isCall, "Test curve");
        }

        private static SortedList<DateTime, OptionPricingData> GetPricingData()
        {
            var pricingData = new SortedList<DateTime, OptionPricingData>
            {
                { new DateTime(2020, 8, 31), new OptionPricingData(48, 0.2, 0.03, 0.01)},
                { new DateTime(2020, 9, 30), new OptionPricingData(50.5, 0.2, 0.03, 0.01)},
                { new DateTime(2020, 10, 31), new OptionPricingData(49, 0.2, 0.03, 0.01)}
            };

            return pricingData;
        }

        private static SortedList<DateTime, double> GetPriceDataOnly()
        {
            var allPricingData = GetPricingData();
            return allPricingData.GetPriceSeries();
        }

        #endregion

        #region tests

        [TestMethod]
        public void TestGetUnderlyingForwardPriceFromRawVariables()
        {
            // arrange
            var EqOption = GetOption(true);

            // act
            var result = EqOption.GetUnderlyingForwardPrice(48, 0.05, 0.03, 1.5);

            // assert
            Assert.AreEqual(46.5813856, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPriceBSModelCall()
        {
            // arrange
            var EqOption = GetOption(true);
            var currentDate = new DateTime(2019, 6, 30);
            var pricingData = new OptionPricingData(48, 0.2, 0.05, 0.02);

            // act
            var result = EqOption.GetPriceBSModel(currentDate, pricingData);

            // assert
            Assert.AreEqual(4.224185, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPriceBSModelPut()
        {
            // arrange
            var EqOption = GetOption(false);
            var currentDate = new DateTime(2019, 03, 31);
            var pricingData = new OptionPricingData(53, 0.15, 0.04, 0.06);

            // act
            var result = EqOption.GetPriceBSModel(currentDate, pricingData);

            // assert
            Assert.AreEqual(2.957613, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPriceBSModelPutCallParity()
        {
            // arrange
            var callOption = GetOption(true);
            var putOption = GetOption(false);
            var currentDate = new DateTime(2020, 8, 31);
            var pricingData = new OptionPricingData(45, 0.18, 0.02, 0.01);

            // assert
            var callPrice = callOption.GetPriceBSModel(currentDate, pricingData);
            var putPrice = putOption.GetPriceBSModel(currentDate, pricingData);
            var difference = callPrice - putPrice;

            // act
            var parityImpliedDifference = -4.951660;
            Assert.AreEqual(parityImpliedDifference, difference, 1e-6);
        }

        [TestMethod]
        public void TestGetPayoffCallInMoney()
        {
            // arrange
            var option = GetOption(true);
            var prices = GetPriceDataOnly();

            // act
            var result = option.GetPayoff(prices);

            // assert
            Assert.AreEqual(0.5, result);
        }

        [TestMethod]
        public void TestGetPayoffCallOutOfMoney()
        {
            // arrange
            var option = GetOption(true, 53);
            var prices = GetPriceDataOnly();

            // act
            var result = option.GetPayoff(prices);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetPayoffPutInMoney()
        {
            // arrange
            var option = GetOption(false, 51.5);
            var prices = GetPriceDataOnly();

            // act
            var result = option.GetPayoff(prices);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOutOfMoney()
        {
            // arrange
            var option = GetOption(false, 49.5);
            var prices = GetPriceDataOnly();

            // act
            var result = option.GetPayoff(prices);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetCallDelta()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetDelta(currentDate, pricingData);

            // asset
            Assert.AreEqual(0.454747, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPutDelta()
        {
            // arrange
            var option = GetOption(false);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetDelta(currentDate, pricingData);

            // asset
            Assert.AreEqual(-0.522125, result, 1e-6);
        }

        [TestMethod]
        public void TestGetGamma()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetGamma(currentDate, pricingData);

            // asset
            Assert.AreEqual(0.042069, result, 1e-6);
        }

        [TestMethod]
        public void TestGetVega()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetVega(currentDate, pricingData);

            // asset
            Assert.AreEqual(0.162878, result, 1e-6);
        }

        [TestMethod]
        public void TestGetCallRho()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetRho(currentDate, pricingData);

            // asset
            Assert.AreEqual(0.146061, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPutRho()
        {
            // arrange
            var option = GetOption(false);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetRho(currentDate, pricingData);

            // asset
            Assert.AreEqual(-0.229021, result, 1e-6);
        }

        [TestMethod]
        public void TestGetCallTheta()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetTheta(currentDate, pricingData);

            // asset
            Assert.AreEqual(-0.010341, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPutTheta()
        {
            // arrange
            var option = GetOption(false);
            var currentDate = new DateTime(2020, 1, 1);
            var pricingData = new OptionPricingData(47.5, 0.22, 0.05, 0.03);

            // act
            var result = option.GetTheta(currentDate, pricingData);

            // asset
            Assert.AreEqual(-0.006292, result, 1e-6);
        }

        #endregion
    }
}
