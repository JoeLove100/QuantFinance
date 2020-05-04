using System.Collections.Generic;
using Utilities.Calculations.Interpolations;
using Utilities.ExtenstionMethods;

namespace Utilities.MarketData.Curves
{
    public abstract class YieldCurve
    {
        #region constructor

        public YieldCurve(SortedDictionary<double, double> ratesByTenor,
                          IInterpolationBehaviour interpolator)
        {
            _ratesByTenor = ratesByTenor;
            _tenors = new List<double>(_ratesByTenor.Keys);
            _interpolator = interpolator;
        }

        #endregion

        #region private fields

        private readonly SortedDictionary<double, double> _ratesByTenor;
        private readonly IInterpolationBehaviour _interpolator;
        private readonly List<double> _tenors;

        #endregion

        #region concrete methods

        public double GetSpotRate(double tenor)
        {
            var spotRate = GetInterpolatedRate(tenor);
            return spotRate;
        }

        protected double GetInterpolatedRate(double tenor)
        {
            var tenorIndices = _tenors.GetPrevAndNextIndex(tenor);
            var selectedTenors = new List<double> { _tenors[tenorIndices.Item1], _tenors[tenorIndices.Item2] };
            var selectedRates = new List<double> { _ratesByTenor[selectedTenors[0]], _ratesByTenor[selectedTenors[1]] };

            var interpolatedRate = _interpolator.Interpolate2D(selectedTenors, selectedRates, tenor);
            return interpolatedRate;
        }

        #endregion

        #region abstract methods

        public abstract double GetAnnualisedForwardRate(double startTenor, double endTenor);

        #endregion

    }
}
