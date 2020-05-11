using System;
using System.Collections.Generic;
using Utilities.Calculations.Interpolations;

namespace Utilities.MarketData.Curves
{
    public class ContinuousCompoundingCurve : YieldCurve
    {
        public ContinuousCompoundingCurve(SortedDictionary<double, double> ratesByTenor,
                                          IInterpolationBehaviour interpolator): base(ratesByTenor, interpolator)
        {

        }


        public override double GetAnnualisedForwardRate(double startTenor, double endTenor)
        {
            var startRate = GetInterpolatedSpotRate(startTenor);
            var endRate = GetInterpolatedSpotRate(endTenor);
            var fwdRate = (endRate * endTenor - startRate * startTenor) / (endTenor - startTenor);
            return fwdRate;
        }

        public override double GetDiscountFactor(double tenor)
        {
            var spot = GetInterpolatedSpotRate(tenor);
            var discountFactor = Math.Exp(-tenor * spot);
            return discountFactor;
        }

        public override double GetDiscountFactor(double tenor, 
                                                 double interestRate)
        {
            var discountFactor = Math.Exp(-tenor * interestRate);
            return discountFactor;
        }
    }
}
