using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionPricing;
using Utilities.MarketData;

namespace Tests.TestOptionPricing
{
    /// <summary>
    /// Summary description for TestEqutiyOption
    /// </summary>
    [TestClass]
    public class TestEuropeanEqutiyOption
    {

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
            Assert.AreEqual(4.241538, result, 1e-6);
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
            Assert.AreEqual(2.968755, result, 1e-6);
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
            var parityImpliedDifference = -4.949466;
            Assert.AreEqual(parityImpliedDifference, difference, 1e-6);
        }

        [TestMethod]
        public void TestGetPayoffCallInMoney()
        {
            // arrange
            var option = GetOption(true);
            var pricingData = GetPricingData();

            // act
            var result = option.GetPayoff(pricingData);

            // assert
            Assert.AreEqual(0.5, result);
        }

        [TestMethod]
        public void TestGetPayoffCallOutOfMoney()
        {
            // arrange
            var option = GetOption(true, 53);
            var pricingData = GetPricingData();

            // act
            var result = option.GetPayoff(pricingData);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetPayoffPutInMoney()
        {
            // arrange
            var option = GetOption(false, 51.5);
            var pricingData = GetPricingData();

            // act
            var result = option.GetPayoff(pricingData);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOutOfMoney()
        {
            // arrange
            var option = GetOption(false, 49.5);
            var pricingData = GetPricingData();

            // act
            var result = option.GetPayoff(pricingData);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetCallDelta()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetDelta(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(0.455130, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPutDelta()
        {
            // arrange
            var option = GetOption(false);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetDelta(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(-0.521624, result, 1e-6);
        }

        [TestMethod]
        public void TestGetGamma()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetGamma(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(0.041960, result, 1e-6);
        }

        [TestMethod]
        public void TestGetVega()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetVega(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(0.163291, result, 1e-6);
        }

        [TestMethod]
        public void TestGetCallRho()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetRho(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(0.146873, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPutRho()
        {
            // arrange
            var option = GetOption(false);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetRho(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(-0.230058, result, 1e-6);
        }

        [TestMethod]
        public void TestGetCallTheta()
        {
            // arrange
            var option = GetOption(true);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetTheta(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(-0.010317, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPutTheta()
        {
            // arrange
            var option = GetOption(false);
            var currentDate = new DateTime(2020, 1, 1);

            // act
            var result = option.GetTheta(currentDate, 47.5, 0.05, 0.03, 0.22);

            // asset
            Assert.AreEqual(-0.006269, result, 1e-6);
        }
    }
}
