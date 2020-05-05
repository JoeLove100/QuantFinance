using System;
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

        private static EuropeanEquityOption GetOption(bool isCall)
        {
            var expiryDate = new DateTime(2020, 9, 30);
            return new EuropeanEquityOption("Test index", expiryDate, 50, isCall, "Test curve");
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
    }
}
