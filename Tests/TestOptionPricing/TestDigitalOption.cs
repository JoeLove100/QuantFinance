using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionPricing;

namespace Tests.TestOptionPricing
{
    [TestClass]
    public class TestDigitalOption
    {

        private static DigitalOption GetOption(bool isCall, 
                                               bool isAssetSettled,
                                               double strike = 105)
        {
            var expiryDate = new DateTime(2018, 5, 31);
            var option = new DigitalOption("testAsset", expiryDate, strike, isCall, "testCurve", isAssetSettled);
            return option;
        }

        private static Dictionary<string, SortedList<DateTime, double>> GetIndexData()
        {
            var indexPrices = new SortedList<DateTime, double>
            {
                { new DateTime(2018, 4, 30), 103},
                { new DateTime(2018, 5, 31), 106},
                { new DateTime(2018, 6, 30), 110}
            };

            var indexData = new Dictionary<string, SortedList<DateTime, double>>
            {
                { "testAsset", indexPrices }
            };

            return indexData;
        }

        [TestMethod]
        public void TestDigitalOptionCallAssetSettled()
        {
            // arrange
            var option = GetOption(true, true);

            // act
            var result = option.GetPriceBSModel(2.1, 100, 0.04, 0, 0.15);

            // assert
            Assert.AreEqual(55.779689, result, 1e-6);
        }

        [TestMethod]
        public void TestDigitalOptionPutAssetSettled()
        {
            // arrange
            var option = GetOption(false, true);

            // act
            var result = option.GetPriceBSModel(2.1, 100, 0.04, 0, 0.15);

            // assert
            Assert.AreEqual(36.1634361, result, 1e-6);
        }

        [TestMethod]
        public void TestDigitalOptionCallCashSettled()
        {
            // arrange
            var option = GetOption(true, false);

            // act
            var result = option.GetPriceBSModel(0.75, 107, 0.03, 0.04, 0.24);

            // assert
            Assert.AreEqual(0.469682, result, 1e-6);
        }

        [TestMethod]
        public void TestDigitalOptionPutCashSettled()
        {
            // arrange
            var option = GetOption(false, false);

            // act
            var result = option.GetPriceBSModel(0.75, 107, 0.03, 0.04, 0.24);

            // assert
            Assert.AreEqual(0.5080694, result, 1e-6);
        }

        [TestMethod]
        public void TestDigitalOptionPutCallParityCashSettled()
        {
            // arrange
            var callOption = GetOption(true, false);
            var putOption = GetOption(false, false);

            // act
            var callPrice = callOption.GetPriceBSModel(1, 106, 0.07, 0.01, 0.12);
            var putPrice = putOption.GetPriceBSModel(1, 106, 0.07, 0.01, 0.12);
            var sum = callPrice + putPrice;

            // assert
            var paritySum = 0.9323938;
            Assert.AreEqual(paritySum, sum, 1e-6);
        }

        [TestMethod]
        public void TestDigitalOptionPutCallParityAssetSettled()
        {
            // arrange
            var callOption = GetOption(true, true);
            var putOption = GetOption(false, true);

            // act
            var callPrice = callOption.GetPriceBSModel(1.5, 101, 0.04, 0.04, 0.29);
            var putPrice = putOption.GetPriceBSModel(1.5, 101, 0.04, 0.04, 0.29);
            var sum = callPrice + putPrice;

            // assert
            var paritySum = 95.118218;
            Assert.AreEqual(paritySum, sum, 1e-6);
        }

        [TestMethod]
        public void TestGetPayoffCallOptionInTheMoneyAssetSettled()
        {
            // arrange
            var option = GetOption(true, true);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(106, result);
        }

        [TestMethod]
        public void TestGetPayoffCallOptionInTheMoneyCashSettled()
        {
            // arrange
            var option = GetOption(true, false);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestGetPayoffCallOptionOutTheMoneyAssetSettled()
        {
            // arrange
            var option = GetOption(true, true, 108);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetPayoffCallOptionOutTheMoneyCashSettled()
        {
            // arrange
            var option = GetOption(true, false, 108);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOptionInTheMoneyAssetSettled()
        {
            // arrange
            var option = GetOption(false, true, 108);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(106, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOptionInTheMoneyCashSettled()
        {
            // arrange
            var option = GetOption(false, false, 108);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOptionOutTheMoneyAssetSettled()
        {
            // arrange
            var option = GetOption(false, true, 100);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetPayoffPutOptionOutTheMoneyCashSettled()
        {
            // arrange
            var option = GetOption(false, false, 100);
            var indexPrices = GetIndexData();

            // act
            var result = option.GetPayoff(indexPrices);

            // assert
            Assert.AreEqual(0, result);
        }

    }
}
