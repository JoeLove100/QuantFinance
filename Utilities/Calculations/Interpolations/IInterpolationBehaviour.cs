using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Calculations.Interpolations
{
    public interface IInterpolationBehaviour
    {
        double Interpolate2D(IList<double> xVals, IList<double> yVals, double value);
        double Interpolate3D(IList<double> xVals, IList<double> yVals, IList<double> zVals, IList<double> value);
    }
}
