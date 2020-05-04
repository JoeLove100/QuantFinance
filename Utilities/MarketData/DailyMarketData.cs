using System;
using System.Collections.Generic;
using Utilities.MarketData.Curves;
using Utilities.MarketData.Volatility;

namespace Utilities.MarketData
{
    public class DailyMarketData
    {
        #region constructors

        public DailyMarketData(DateTime date,
                               Dictionary<string, Index> indexLevels,
                               Dictionary<string, IVolatilitySurface> volSurfaces,
                               Dictionary<string, YieldCurve> riskFreeCurves)
        {
            IndexData = indexLevels;
            VolSurfaces = volSurfaces;
            RiskFreeCurves = riskFreeCurves;
        }

        #endregion

        #region public properties

        public readonly DateTime Date;
        public readonly Dictionary<string, Index> IndexData;
        public readonly Dictionary<string, IVolatilitySurface> VolSurfaces;
        public readonly Dictionary<string, YieldCurve> RiskFreeCurves;

        #endregion
    }
}
