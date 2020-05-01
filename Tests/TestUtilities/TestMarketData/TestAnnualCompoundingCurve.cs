using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.MarketData.InterestRates;
using Utilities.Calculations.Interpolations;

namespace Tests.TestUtils.TestMarketData
{
    /// <summary>
    /// Summary description for TestAnnualCompoundingCurve
    /// </summary>
    [TestClass]
    public class TestAnnualCompoundingCurve
    {

        #region fixtures

        private AnnualCompoundingCurve GetCurve()
        {
            var ratesByTenor = new SortedDictionary<double, double>
            {
                {0.5, 0.0025},
                {1, 0.005},
                {2, 0.005},
                {10, 0.025}

            };

            var interpolator = new LinearInterpolator();
            var curve = new AnnualCompoundingCurve(ratesByTenor, interpolator);
            return curve;
        }

        #endregion

        #region tests

        [TestMethod]
        public void TestGetSpotRateExactPoint()
        {
            // arrange
            var curve = GetCurve();

            // act
            var result = curve.GetSpotRate(0.5);

            // assert
            Assert.AreEqual(0.0025, result);
        }

        [TestMethod]
        public void TestGetSpotRateSmallerThanSmallestTenor()
        {
            // arrange
            var curve = GetCurve();

            // act
            var result = curve.GetSpotRate(0.1);

            // assert
            Assert.AreEqual(result, 0.0025);
        }

        [TestMethod]
        public void TestGetSpotRateLargerThanLargestTenor()
        {
            // arrange
            var curve = GetCurve();

            // act
            var result = curve.GetSpotRate(12);

            // assert
            Assert.AreEqual(0.025, result); 
        }

        [TestMethod]
        public void TestGetSpotRateNeedsInterpolation()
        {
            // arrange
            var curve = GetCurve();

            // act
            var result = curve.GetSpotRate(4);

            // assert
            Assert.AreEqual(0.01, result);
        }

        [TestMethod]
        public void TestGetForwardRateOverOneYear()
        {
            // arrange
            var curve = GetCurve();

            // act
            var result = curve.GetAnnualisedForwardRate(1, 5);

            // assert
            Assert.AreEqual(0.01438373, result, 1e-5);
        }

        [TestMethod]
        public void TestGetForwardRateLessThanOneYear()
        {
            // arrange
            var curve = GetCurve();

            // act
            var result = curve.GetAnnualisedForwardRate(0.5, 0.75);

            // assert
            Assert.AreEqual(0.00625468, result, 1e-5);
        }

        #endregion
    }
}
