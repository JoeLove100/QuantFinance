using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stochastics;
using Utilities.ExtenstionMethods;

namespace Tests.TestStochastics
{
    [TestClass]
    public class TestBasicRandomNumberGenerator
    {

        private static Sampler GetSampler()
        {
            var randomGenerator = new BasicRandomNumberGenerator(1000);
            var sampler = new Sampler(randomGenerator);
            return sampler;
        }

        [TestMethod]
        public void TestGetStandardRandomNormal()
        {
            // arrange
            var sampler = GetSampler();

            // act
            var result = sampler.GetRandStandardNormal(5);

            // assert
            var expectedResult = new List<double> {-1.0297765, -0.719414, 0.693535, -3.064060, 0.500882};
            Assert.IsTrue(expectedResult.IsAlmostEqual(result, 1e-6));
        }

        [TestMethod]
        public void TestGetRandomNormal()
        {
            // arrange
            var sampler = GetSampler();

            // act
            var result = sampler.GetRandNormal(0.05, 0.1, 5);

            // assert
            var expectedResult = new List<double> { -0.052978, -0.021941, 0.119354, -0.256406, 0.100088 };
            Assert.IsTrue(expectedResult.IsAlmostEqual(result, 1e-6));
        }
    }
}
