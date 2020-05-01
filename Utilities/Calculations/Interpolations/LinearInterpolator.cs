using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Calculations.Interpolations
{
    public class LinearInterpolator : IInterpolationBehaviour
    {

        public double Interpolate2D(IList<double> xVals, IList<double> yVals, double interpolate)
        {
            if (xVals.Count() != 2 || yVals.Count() != 2)
            {
                throw new ArgumentException($"Passed values shoudld have lenght 2 - lengths are " +
                    $"{xVals.Count()} and ${yVals.Count()}");
            }

            if (xVals[0] == xVals[1])
            {
                return yVals[0];
            }
            else
            {
                var interpolatedValue = ((interpolate - xVals[0]) * yVals[1] + (xVals[1] - interpolate) * yVals[0]) / (xVals[1] - xVals[0]);
                return interpolatedValue;
            }

        }

        public double Interpolate3D(IList<double> xVals, 
                                    IList<double> yVals, 
                                    IList<double> zVals, 
                                    IList<double> interpolate)
        {
            if (xVals.Count != 4|| yVals.Count != 4  || zVals.Count != 4)
            {
                throw new ArgumentException($"Passed values shoudld have lenght 4 - lengths are " +
                    $"{xVals.Count},  ${yVals.Count} and {zVals.Count}");
            }

            // interpolate in the x direction first
            var interpXOne = Interpolate2D(xVals.Take(2).ToList(), zVals.Take(2).ToList(), interpolate[0]);
            var interpXTwo = Interpolate2D(xVals.Skip(2).Take(2).ToList(), zVals.Skip(2).Take(2).ToList(), interpolate[0]);

            // then interpolate these points in the y direction
            var yValues = new List<double> { yVals[0], yVals[2] };
            var interpXValues = new List<double> { interpXOne, interpXTwo };
            var interpolatedValue = Interpolate2D(yValues, interpXValues, interpolate[1]);

            return interpolatedValue;
        }
    }
}
