using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastics
{
    public class BasicRandomNumberGenerator: IRandomNumberGenerator
    {
        public BasicRandomNumberGenerator(int? seed)
        {
            RandomGenerator = seed is null ? new Random() : new Random((int) seed);
        }

        private readonly Random RandomGenerator;

        public List<double> GetRandUniform(double lowerBound, double upperBound, int length)
        {
            List<double> randomNumbers = new List<double>();

            for (int i = 0; i < length; i++)
            {
                var number = RandomGenerator.NextDouble() * (upperBound - lowerBound) + lowerBound;
                randomNumbers.Add(number);
            }

            return randomNumbers;
        }
    }
}
