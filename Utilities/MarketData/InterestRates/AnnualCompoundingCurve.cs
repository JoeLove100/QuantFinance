using System;
using System.Collections.Generic;
using Utilities.Calculations.Interpolations;
using Utilities.ExtenstionMethods;

/// <summary>
/// Basic interest rate curve implemenation with simple 
/// annual compounding
/// </summary>

namespace Utilities.MarketData.InterestRates
{
    public class AnnualCompoundingCurve : IInterestRateCurve
    {
        #region constructor

        public AnnualCompoundingCurve(SortedDictionary<double, double> ratesByTenor,
                                      IInterpolationBehaviour interpolator)
        {
            _ratesByTenor = ratesByTenor;
            _tenors = new List<double>(_ratesByTenor.Keys);
            Interpolator = interpolator;
        }

        #endregion

        #region private properties

        private readonly SortedDictionary<double, double> _ratesByTenor;
        private readonly List<double> _tenors;
        private readonly IInterpolationBehaviour Interpolator;

        #endregion

        #region public methods

        public double GetAnnualisedForwardRate(double startTenor, double endTenor)
        {
            var startSpot = GetInterpolatedRate(startTenor);
            var endSpot = GetInterpolatedRate(endTenor);

            var fwdFactor = Math.Pow(Math.Pow(1 + endSpot, endTenor) / Math.Pow(1 + startSpot, startTenor), 1 / (endTenor - startTenor));
            var fwdRate = fwdFactor - 1;
            return fwdRate;
        }

        public double GetSpotRate(double tenor)
        {
            var spotRate = GetInterpolatedRate(tenor);
            return spotRate;
        }

        #endregion

        #region private methods

        private double GetInterpolatedRate(double tenor)
        {
            var tenorIndices = _tenors.GetPrevAndNextIndex(tenor);
            var selectedTenors = new List<double> { _tenors[tenorIndices.Item1], _tenors[tenorIndices.Item2] };
            var selectedRates = new List<double> { _ratesByTenor[selectedTenors[0]], _ratesByTenor[selectedTenors[1]]};

            var interpolatedRate = Interpolator.Interpolate2D(selectedTenors, selectedRates, tenor);
            return interpolatedRate;
        }

        #endregion
    }
}
