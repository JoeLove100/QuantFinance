using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OptionPricing;

namespace Tests.TestOptionPricing
{
    /// <summary>
    /// Summary description for TestEqutiyOption
    /// </summary>
    [TestClass]
    public class TestEqutiyOption
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
    }
}
