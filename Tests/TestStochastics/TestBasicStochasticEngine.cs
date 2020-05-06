using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stochastics;
using Utilities.ExtenstionMethods;

namespace Tests.TestStochastics
{
    [TestClass]
    public class TestBasicStochasticEngine
    {
        private BasicStochasticEngine GetStochasticEngine(int seed)
        {
            var randomGenerator = new BasicRandomNumberGenerator(seed);
            var sampler = new Sampler(randomGenerator);
            var stochasticEngine = new BasicStochasticEngine(sampler);
            return stochasticEngine;
        }

        [TestMethod]
        public void TestGetStandardBrownianMotion()
        {
            // arrange
            var stochasticEngine = GetStochasticEngine(1234);

            // act
            double timePeriod = 1.0 / 12.0;
            var result = stochasticEngine.GetBrownianMotionSeries(0.1, 0.18, timePeriod, 7);

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
            var result = stochasticEngine.GetGeometricBrownianSeries(0.05, 0.2, 50, timePeriod, 7);

            // assert
            var expectedResult = new List<double> { 49.390526, 53.245843, 51.950346, 57.166804, 55.956294,
                                                    61.642971, 64.982546};
            Assert.IsTrue(expectedResult.IsAlmostEqual(result, 1e-6));
        }
    }
}
