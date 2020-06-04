using System.Collections.Generic;

namespace Stochastics
{
    public interface IRandomNumberGenerator
    {
        List<double> GetRandUniform(double lowerBound, double upperBound, int length, int? seed);
    }
}
