using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Calculations.Interpolations;

namespace Tests.TestUtils.TestCalculations
{
    [TestClass]
    public class TestLinearInterpolation
    {
        [TestMethod]
        public void TestLinearInterpolation2DNoInterpolation()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { 0, 5 };
            var yVals = new List<double> { 2, 7 };
            var interpolate = 5;

            // act
            var result = interpolator.Interpolate2D(xVals, yVals, interpolate);

            // assert
            Assert.AreEqual(7, result);
        }

        [TestMethod]
        public void TestLinear2DInterpolationSamePoint()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { -2, -2 };
            var yVals = new List<double> { -3, -3 };
            var interpolate = -2;

            // act
            var result = interpolator.Interpolate2D(xVals, yVals, interpolate);

            // assert
            Assert.AreEqual(-3, result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLinear2DInterpolationErrorForWrongNumberOfInputs()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { -2, -2 };
            var yVals = new List<double> { -3, -3, 4};
            var interpolate = -2;

            // act/assert
            var result = interpolator.Interpolate2D(xVals, yVals, interpolate);
        }

        [TestMethod]
        public void TestLinear2dInterpolationSmallToBig()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { -2,  1.5 };
            var yVals = new List<double> { 3.2, 12.5 };
            var interpolate = 0.5;

            // act
            var result = interpolator.Interpolate2D(xVals, yVals, interpolate);

            // assert
            Assert.AreEqual(9.842857, result, 1e-6);
        }

        [TestMethod]
        public void TestLinear2dInterpolationBigToSmall()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { 1, 2 };
            var yVals = new List<double> { 2.1, -0.8 };
            var interpolate = 1.2;

            // act
            var result = interpolator.Interpolate2D(xVals, yVals, interpolate);

            // assert
            Assert.AreEqual(1.52, result, 1e-6);
        }

        [TestMethod]
        public void TestLinear3dInterpolation()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { 1, 3.5, 1, 3.5 };
            var yVals = new List<double> { -0.8, -0.8, 1.2, 1.2 };
            var zVals = new List<double> { 1.2, 2.2, -3.4, 5 };
            var interpolate = new List<double> { 2.1, 0.1};

            // act
            var result = interpolator.Interpolate3D(xVals, yVals, zVals, interpolate);

            // assert
            Assert.AreEqual(1.0352, result, 1e-6);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestLinear3dInterpolationExceptionWrongNumberOfArguments()
        {
            // arrange
            var interpolator = new LinearInterpolator();
            var xVals = new List<double> { 1, 3.5, 1, 3.5, 10 };
            var yVals = new List<double> { -0.8, -0.8, 1.2, 1.2 };
            var zVals = new List<double> { 1.2, 2.2, -3.4 };
            var interpolate = new List<double> { 2.1, 0.1 };

            // act/assert
            var result = interpolator.Interpolate3D(xVals, yVals, zVals, interpolate);
        }
    }
}
