using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionPricing;

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

        private static Dictionary<string, SortedList<DateTime, double>> GetIndexData()
        {
            var indexPrices = new SortedList<DateTime, double>
            {
                { new DateTime(2020, 8, 31), 48},
                { new DateTime(2020, 9, 30), 50.5},
                { new DateTime(2020, 10, 31), 49}
            };

            var indexData = new Dictionary<string, SortedList<DateTime, double>>
            {
                { "Test index", indexPrices }
            };

            return indexData;
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

            // act
            var result = EqOption.GetPriceBSModel(1.25, 48, 0.05, 0.02, 0.2);

            // assert
            Assert.AreEqual(4.096997, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPriceBSModelPut()
        {
            // arrange
            var EqOption = GetOption(false);

            // act
            var result = EqOption.GetPriceBSModel(0.5, 53, 0.04, 0.06, 0.15);

            // assert
            Assert.AreEqual(1.128237, result, 1e-6);
        }

        [TestMethod]
        public void TestGetPriceBSModelPutCallParity()
        {
            // arrange
            var callOption = GetOption(true);
            var putOption = GetOption(false);

            // assert
            var callPrice = callOption.GetPriceBSModel(0.9, 45, 0.02, 0.01, 0.18);
            var putPrice = putOption.GetPriceBSModel(0.9, 45, 0.02, 0.01, 0.18);
            var difference = callPrice - putPrice;

            // act
            var parityImpliedDifference = -4.5112346;
            Assert.AreEqual(parityImpliedDifference, difference, 1e-6);
        }

        [TestMethod]
        public void TestGetPayoffCallInMoney()
        {
            // arrange
            var option = GetOption(true);
            var indexData = GetIndexData();

            // act
            var result = option.GetPayoff(indexData);

            // assert
            Assert.AreEqual(0.5, result);
        }

        [TestMethod]
        public void TestGetPayoffCallOutOfMoney()
        {
            // arrange
            var option = GetOption(true, 53);
            var indexData = GetIndexData();

            // act
            var result = option.GetPayoff(indexData);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetPayoffPutInMoney()
        {
            // arrange
            var option = GetOption(false, 51.5);
            var indexData = GetIndexData();

            // act
            var result = option.GetPayoff(indexData);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOutOfMoney()
        {
            // arrange
            var option = GetOption(false, 49.5);
            var indexData = GetIndexData();

            // act
            var result = option.GetPayoff(indexData);

            // assert
            Assert.AreEqual(0, result);
        }
    }
}
