using System.Collections.Generic;
using Utilities.MarketData.InterestRates;
using Utilities.MarketData.Volatility;

namespace Utilities.MarketData
{
    public class DailyMarketData
    {
        #region constructors

        public DailyMarketData(Dictionary<string, double> indexLevels,
                               Dictionary<string, IVolatilitySurface> volSurfaces,
                               Dictionary<string, IInterestRateCurve> riskFreeCurves)
        {
            IndexLevels = indexLevels;
            VolSurfaces = volSurfaces;
            RiskFreeCurves = riskFreeCurves;
        }

        #endregion

        #region public properties

        public readonly Dictionary<string, double> IndexLevels;
        public readonly Dictionary<string, IVolatilitySurface> VolSurfaces;
        public readonly Dictionary<string, IInterestRateCurve> RiskFreeCurves;

        #endregion
    }
}
