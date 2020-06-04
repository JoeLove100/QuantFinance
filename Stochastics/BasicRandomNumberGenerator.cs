using System;
using System.Collections.Generic;

namespace Stochastics
{
    public class BasicRandomNumberGenerator: IRandomNumberGenerator
    {

        public List<double> GetRandUniform(double lowerBound, double upperBound, int length, int? seed)
        {
            List<double> randomNumbers = new List<double>();
            Random randomGenerator = seed != null ? new Random((int) seed) : new Random();

            for (int i = 0; i < length; i++)
            {
                var number = randomGenerator.NextDouble() * (upperBound - lowerBound) + lowerBound;
                randomNumbers.Add(number);
            }

            return randomNumbers;
        }
    }
}
