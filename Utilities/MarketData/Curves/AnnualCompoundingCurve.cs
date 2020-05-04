using System;
using System.Collections.Generic;
using Utilities.Calculations.Interpolations;
using Utilities.ExtenstionMethods;

/// <summary>
/// Basic interest rate curve implemenation with simple 
/// annual compounding
/// </summary>

namespace Utilities.MarketData.Curves
{
    public class AnnualCompoundingCurve : YieldCurve
    {
        #region constructor

        public AnnualCompoundingCurve(SortedDictionary<double, double> ratesByTenor,
                                      IInterpolationBehaviour interpolator): base(ratesByTenor, interpolator)
        {

        }

        #endregion

        #region overrides

        public override double GetAnnualisedForwardRate(double startTenor, double endTenor)
        {
            var startSpot = GetInterpolatedRate(startTenor);
            var endSpot = GetInterpolatedRate(endTenor);

            var fwdFactor = Math.Pow(Math.Pow(1 + endSpot, endTenor) / Math.Pow(1 + startSpot, startTenor), 1 / (endTenor - startTenor));
            var fwdRate = fwdFactor - 1;
            return fwdRate;
        }

        #endregion
    }
}