using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stochastics;
using Utilities.ExtenstionMethods;
using OptionPricing;

namespace Tests.TestStochastics
{
    [TestClass]
    public class TestBasicStochasticEngine
    {

        #region fixtures 

        private BasicStochasticEngine GetStochasticEngine(int? seed)
        {
            var randomGenerator = new BasicRandomNumberGenerator(seed);
            var sampler = new Sampler(randomGenerator);
            var stochasticEngine = new BasicStochasticEngine(sampler);
            return stochasticEngine;
        }

        private static EuropeanEquityOption GetEuropeanOption(bool isCall,
                                                              double strike = 50)
        {
            var expiryDate = new DateTime(2020, 9, 30);
            return new EuropeanEquityOption("Test index", expiryDate, strike, isCall, "Test curve");
        }

        private static DigitalOption GetDigitalOption(bool isCall,
                                                      bool isAssetSettled,
                                                      double strike = 80)
        {
            var expiryDate = new DateTime(2020, 3, 13);
            return new DigitalOption("Test index", expiryDate, strike, isCall, "Test curve", isAssetSettled);
        }

        #endregion

        #region tests

        [TestMethod]
        public void TestGetStandardBrownianMotion()
        {
            // arrange
            var stochasticEngine = GetStochasticEngine(1234);

            // act
            double timePeriod = 1.0 / 12.0;
            var gbmParams = new GbmParameters(0.1, 0.18);
            var result = stochasticEngine.GetBrownianMotionSeries(gbmParams, timePeriod, 7);

            // assert
            var expectedResult = new List<double> {-0.004955, 0.068774, 0.052689, 0.144889, 0.131710,
                                                    0.224903, 0.278469};
            Assert.IsTrue(expectedResult.IsAlmostEqual(result, 1e-6));
        }

        [TestMethod]
        public void TestGetGeometricBrownianMotion()
        {
            // arrange
            var stochasticEngine = GetStochasticEngine(1234);

            // act
            double timePeriod = 1.0 / 12.0;
            var gbpParams = new GbmParameters(0.05, 0.2, 50);
            var result = stochasticEngine.GetGeometricBrownianSeries(gbpParams, timePeriod, 7);

            // assert
            var expectedResult = new List<double> { 49.390526, 53.245843, 51.950346, 57.166804, 55.956294,
                                                    61.642971, 64.982546};
            Assert.IsTrue(expectedResult.IsAlmostEqual(result, 1e-6));
        }

        [TestMethod]
        public void TestPriceCallOption()
        {
            // arrange
            var option = GetEuropeanOption(true);
            var stochasticEngine = GetStochasticEngine(4321);
            var currentDate = new DateTime(2019, 6, 30);
            var paramsByUnderlying = new Dictionary<string, GbmParameters>
            {
                { "Test index", new GbmParameters(0.02, 0.24, 51)}
            };

            // act
            var result = stochasticEngine.GetOptionValue(option, currentDate, paramsByUnderlying, 0.939413, 100000);

            // assert
            Assert.AreEqual(6.4892, result, 1e-4);

        }

        [TestMethod]
        public void TestPricePutOption()
        {
            // arrange
            var option = GetEuropeanOption(false);
            var stochasticEngine = GetStochasticEngine(1234);
            var currentDate = new DateTime(2019, 6, 30);
            var paramsByUnderlying = new Dictionary<string, GbmParameters>
            {
                { "Test index", new GbmParameters(0.02, 0.20, 50)}
            };

            // act
            var result = stochasticEngine.GetOptionValue(option, currentDate, paramsByUnderlying, 0.939413, 100000);

            // assert
            Assert.AreEqual(3.7813, result, 1e-4);

        }

        [TestMethod]
        public void TestPriceDigitalCallAssetSettled()
        {
            // arrange
            var option = GetDigitalOption(true, true);
            var stochasticEngine = GetStochasticEngine(1423);
            var currentDate = new DateTime(2019, 7, 20);
            var paramsByUnderlying = new Dictionary<string, GbmParameters>
            {
                { "Test index", new GbmParameters(0.04, 0.18, 75) }
            };

            // act
            var result = stochasticEngine.GetOptionValue(option, currentDate, paramsByUnderlying, 0.966378, 100000);

            // assert
            Assert.AreEqual(32.0434, result, 1e-4);
        }

        [TestMethod]
        public void TestPriceDigitalPutAssetSettled()
        {
            // arrange
            var option = GetDigitalOption(false, true);
            var stochasticEngine = GetStochasticEngine(1243);
            var currentDate = new DateTime(2019, 7, 20);
            var paramsByUnderlying = new Dictionary<string, GbmParameters>
            {
                { "Test index", new GbmParameters(0.04, 0.18, 75) }
            };

            // act
            var result = stochasticEngine.GetOptionValue(option, currentDate, paramsByUnderlying, 0.966378, 100000);

            // assert
            Assert.AreEqual(42.5441, result, 1e-4);
        }

        [TestMethod]
        public void TestPriceDigitalCallCashSettled()
        {
            // arrange
            var option = GetDigitalOption(true, false);
            var stochasticEngine = GetStochasticEngine(1111);
            var currentDate = new DateTime(2019, 5, 31);
            var paramsByUnderlying = new Dictionary<string, GbmParameters>
            {
                { "Test index", new GbmParameters(0.01, 0.25, 81)}
            };

            // act
            var result = stochasticEngine.GetOptionValue(option, currentDate, paramsByUnderlying, 1, 100000);

            // assert
            Assert.AreEqual(0.49296, result, 1e-4);
        }

        [TestMethod]
        public void TestPriceDigitalPutCashSettled()
        {
            // arrange
            var option = GetDigitalOption(false, false);
            var stochasticEngine = GetStochasticEngine(1221);
            var currentDate = new DateTime(2019, 5, 31);
            var paramsByUnderlying = new Dictionary<string, GbmParameters>
            {
                { "Test index", new GbmParameters(0.01, 0.25, 84)}
            };

            // act
            var result = stochasticEngine.GetOptionValue(option, currentDate, paramsByUnderlying, 1, 100000);

            // assert
            Assert.AreEqual(0.4484, result, 1e-4);
        }

        #endregion

    }
}
