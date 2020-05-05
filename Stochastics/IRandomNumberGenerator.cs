using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stochastics
{
    public interface IRandomNumberGenerator
    {
        List<double> GetRandUniform(double lowerBound, double upperBound, int length);
    }
}
