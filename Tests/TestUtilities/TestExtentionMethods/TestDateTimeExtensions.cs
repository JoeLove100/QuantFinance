using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.ExtenstionMethods;

namespace Tests.TestUtilities.TestExtentionMethods
{
    /// <summary>
    /// Summary description for TestDateTimeExtensions
    /// </summary>
    [TestClass]
    public class TestDateTimeExtensions
    {
        [TestMethod]
        public void TestGetYearFractionToCurrentAfterFuture()
        {
            // arrange
            var currentDate = new DateTime(2020, 3, 31);
            var futureDate = new DateTime(2020, 2, 15);

            // act
            var result = currentDate.GetYearFractionTo(futureDate);

            // assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestGetYearFractionToLessThatOneYear()
        {
            // arrange
            var currentDate = new DateTime(2019, 4, 28);
            var futureDate = new DateTime(2019, 11, 28);

            // act
            var result = currentDate.GetYearFractionTo(futureDate);

            //
            Assert.AreEqual(0.5863014, result, 1e-6);
        }

        [TestMethod]
        public void TestGetYearFractionToMoreThanOneYear()
        {
            // arrange
            var currentDate = new DateTime(2015, 6, 4);
            var futureDate = new DateTime(2020, 2, 15);

            // act
            var result = currentDate.GetYearFractionTo(futureDate);

            // assert
            Assert.AreEqual(4.7041096, result, 1e-6);
        }

        [TestMethod]
        public void TestAddWorkingDaysFriday()
        {
            // arrange
            var date = new DateTime(2020, 5, 1);

            // act
            var result = date.AddWorkingDay();

            // asset
            Assert.AreEqual(new DateTime(2020, 5, 4), result);
        }

        [TestMethod]
        public void TestAddWorkingDaysSaturday()
        {
            // arrange
            var date = new DateTime(2020, 5, 2);

            // act
            var result = date.AddWorkingDay();

            // asset
            Assert.AreEqual(new DateTime(2020, 5, 4), result);
        }

        [TestMethod]
        public void TestAddWorkingDaysOtherDay()
        {
            // arrange
            var date = new DateTime(2020, 5, 4);

            // act
            var result = date.AddWorkingDay();

            // asset
            Assert.AreEqual(new DateTime(2020, 5, 5), result);
        }

    }
}
