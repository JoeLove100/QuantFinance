using System;
using System.Collections.Generic;
using Utilities.Calculations.Interpolations;
using Utilities.MarketData.Curves;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.TestUtilities.TestMarketData
{
    [TestClass]
    public class TestContinuousCompoundingCurve
    {
        private static ContinuousCompoundingCurve GetCurve()
        {
            var ratesByTenor = new SortedDictionary<double, double>
            {
                { 0.5, 0.01},
                { 1.0, 0.02},
                { 2.0, 0.05},
                { 5.0, 0.06}
            };

            var interpolator = new LinearInterpolator();
            var curve = new ContinuousCompoundingCurve(ratesByTenor, interpolator);
            return curve;
        }

        private static ContinuousCompoundingCurve GetFlatCurve()
        {
            var ratesByTenor = new SortedDictionary<double, double> { { 1, 0.03 } };
            var interpolator = new LinearInterpolator();
            var curve = new ContinuousCompoundingCurve(ratesByTenor, interpolator);
            return curve;
        }

        [TestMethod]
        public void TestGetAnnualisedForwardRate()
        {
            // arrange
            var curve = GetCurve();
            var startTenor = 1;
            var endTenor = 5;

            // act
            var result = curve.GetAnnualisedForwardRate(startTenor, endTenor);

            // assert
            Assert.AreEqual(0.07, result, 1e-6);
        }

        [TestMethod]
        public void TestGetAnnualisedForwardRateFlatCurve()
        {
            // arrange
            var curve = GetFlatCurve();
            var startTenor = 2.5;
            var endTenor = 4;

            // act
            var result = curve.GetAnnualisedForwardRate(startTenor, endTenor);

            // assert
            Assert.AreEqual(0.03, result, 1e-6);
        }

        [TestMethod]
        public void TestGetDiscountFactor()
        {
            // arrange
            var curve = GetCurve();
            var tenor = 1.3;
            var rate = 0.04;

            // act
            var result = curve.GetDiscountFactor(tenor, rate);

            // assert
            Assert.AreEqual(0.949329, result, 1e-6);
        }

        [TestMethod]
        public void TestGetDiscountFactorFlat()
        {
            // arrange
            var curve = GetFlatCurve();
            var tenor = 2;

            // act
            var result = curve.GetDiscountFactor(tenor);

            // assert
            Assert.AreEqual(0.941765, result, 1e-6);
        }
    }
}
