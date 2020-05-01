using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.ExtenstionMethods;

namespace Tests.TestUtils.TestExtentionMethods
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetPrevAndNextIndexBeforeFirstIndex()
        {
            // arrange
            var testList = new List<double> { 1, 2, 3, 4, 5 };
            var testValue = -1.2;

            // act
            var result = testList.GetPrevAndNextIndex(testValue);

            // assert
            var expectedResult = new Tuple<int, int>(0, 0);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestGetPrevAndNextIndexAfterLastIndex()
        {
            // arrange
            var testList = new List<double> { 1, 2, 3, 4, 5 };
            var testValue = 8.2;

            // act
            var result = testList.GetPrevAndNextIndex(testValue);

            // assert
            var expectedResult = new Tuple<int, int>(4, 4);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestGetPrevAndNextIndexExactIndexInList()
        {
            // arrange
            var testList = new List<double> { 1, 2, 3, 4, 5 };
            var testValue = 2;

            // act
            var result = testList.GetPrevAndNextIndex(testValue);

            // assert
            var expectedResult = new Tuple<int, int>(1, 1);
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void TestGetPrevAndNextIndexInterpolateList()
        {
            // arrange
            var testList = new List<double> { 1, 2, 3, 4, 5 };
            var test_value = 3.3;

            // act
            var result = testList.GetPrevAndNextIndex(test_value);

            // assert
            var expectedResult = new Tuple<int, int>(2, 3);
            Assert.AreEqual(expectedResult, result);
        }

    }
}
