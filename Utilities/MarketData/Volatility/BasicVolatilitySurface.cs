using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Calculations.Interpolations;
using Utilities.ExtenstionMethods;

/// <summary>
/// naive equity vol surface implementation
/// </summary>

namespace Utilities.MarketData.Volatility
{
    public class BasicVolatilitySurface : IVolatilitySurface
    {

        #region constructors

        public BasicVolatilitySurface(double[,] volatilities,
                                 List<double> tenors,
                                 List<double> moneyness,
                                 IInterpolationBehaviour interpolator)
        {
            _volatilities = volatilities;
            _tenors = tenors;
            _moneyness = moneyness;
            Interpolator = interpolator;
        }

        #endregion

        #region private fields

        private readonly double[,] _volatilities;
        private readonly List<double> _tenors;
        private readonly List<double> _moneyness;
        private readonly IInterpolationBehaviour Interpolator;

        #endregion

        #region public methods


        public double GetVolatility(double tenor, 
                                    double moneyness)
            ///<summary>
            /// interpolate the vols surface to get the vol level (where
            /// we assume the surface is constant at points beyond its defined
            /// bounds
            ///</summary>
        {
            var tenorIndices =  _tenors.GetPrevAndNextIndex(tenor);
            var selectedTenors = new List<double> { _tenors[tenorIndices.Item1], _tenors[tenorIndices.Item2],
                                                    _tenors[tenorIndices.Item1], _tenors[tenorIndices.Item2]};

            var moneynessIndices = _moneyness.GetPrevAndNextIndex(moneyness);
            var selectedMoneyness = new List<double> { _moneyness[moneynessIndices.Item1], _moneyness[moneynessIndices.Item1],
                                                       _moneyness[moneynessIndices.Item2], _moneyness[moneynessIndices.Item2]};

            var selectedVols = new List<double> { _volatilities[tenorIndices.Item1, moneynessIndices.Item1],
                                                  _volatilities[tenorIndices.Item2, moneynessIndices.Item1],
                                                  _volatilities[tenorIndices.Item1, moneynessIndices.Item2],
                                                  _volatilities[tenorIndices.Item2, moneynessIndices.Item2]};

            var pointToInterpolate = new List<double> { tenor, moneyness };
            var interpolatedVol = Interpolator.Interpolate3D(selectedTenors, selectedMoneyness, selectedVols, pointToInterpolate);
            return interpolatedVol;

        }

        #endregion 
    }
}
