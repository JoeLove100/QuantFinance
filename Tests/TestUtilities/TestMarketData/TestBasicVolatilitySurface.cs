using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.MarketData.Volatility;
using Utilities.Calculations.Interpolations;

namespace Tests.TestUtils.TestMarketData
{
    [TestClass]
    public class TestBasicVolatilitySurface
    {

        private BasicVolatilitySurface GetVolSurface()
        {
            var tenors = new List<double> { 0.5, 1, 2 , 5};
            var moneyness = new List<double> { 0.8, 1, 1.2 };
            var vol = new double[4, 3] { { 0.35, 0.30, 0.28 }, { 0.4, 0.4, 0.4 }, { 0.5, 0.45, 0.45 }, { 0.5, 0.5, 0.45 } };

            var volSurface = new BasicVolatilitySurface(vol, tenors, moneyness, new LinearInterpolator());
            return volSurface;
        }

        [TestMethod]
        public void TestBasicVolatilitySurfaceGetExactPoint()
        {
            // arrange
            var volSurface = GetVolSurface();
            var tenor = 2;
            var moneyness = 1.2;

            // act
            var result = volSurface.GetVolatility(tenor, moneyness);

            // assert
            Assert.AreEqual(0.45, result);
        }

        [TestMethod]
        public void TestBasicVolatilitySurfaceTenorOutOfRange()
        {
            // arrange
            var volSurface = GetVolSurface();
            var tenor = 6;
            var moneyness = 0.8;

            // act
            var result = volSurface.GetVolatility(tenor, moneyness);

            // assert
            Assert.AreEqual(0.5, result);
        }

        [TestMethod]
        public void TestBasicVolatilitySurfaceMoneynessOutOfRange()
        {
            // arrange
            var volSurface = GetVolSurface();
            var tenor = 0.5;
            var moneyness = 0.75;

            // act
            var result = volSurface.GetVolatility(tenor, moneyness);

            // assert
            Assert.AreEqual(0.35, result);
        }

        [TestMethod]
        public void TestBasicVolatilitySurfaceInterpolateVol()
        {
            // arrange
            var volSurface = GetVolSurface();
            var tenor = 4;
            var moneyness = 1.1;

            // act
            var result = volSurface.GetVolatility(tenor, moneyness);

            // assert
            Assert.AreEqual(0.4666667, result, 1e-6);
        }
    }
}
