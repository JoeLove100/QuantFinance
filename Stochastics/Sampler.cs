using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

/// <summary>
/// naive random number generator using the
/// standard Radom generator class - generates
/// pseudo random numbers only
/// </summary>


namespace Stochastics
{
    public class Sampler
    {
        #region constructor

        public Sampler(IRandomNumberGenerator randomGenerator)
        {
            RandomGenerator = randomGenerator;
        }

        public Sampler(int? seed)
        {
            RandomGenerator = new BasicRandomNumberGenerator(seed);
        }

        #endregion

        #region fields

        private readonly IRandomNumberGenerator RandomGenerator;

        #endregion

        #region implementations

        public List<double> GetRandUniform(double lowerBound, double upperBound, int length)
        {
            return RandomGenerator.GetRandUniform(lowerBound, upperBound, length);
        }

        public List<double> GetRandNormal(double mean, double std, int length)
        {
            List<double> allRandNumbers = new List<double>();
            List<double> uniformNumbers = RandomGenerator.GetRandUniform(0, 1, length);

            for(int i = 0; i < length; i++)
            {
                var uniform = uniformNumbers[i];
                var randNumber = Normal.InvCDF(mean, std, uniform);
                allRandNumbers.Add(randNumber);
            }

            return allRandNumbers;
        }


        public List<double> GetRandStandardNormal(int length)
        {
            return GetRandNormal(0, 1, length);
        }

        #endregion
    }
}
