using Utilities.MarketData.Curves;

namespace Utilities.MarketData
{
    public class Index
    {
        #region constructor

        public Index(double level,
                     YieldCurve divCurve)
        {
            Level = level;
            DividendCurve = divCurve;
        }

        #endregion

        #region properties

        public readonly double Level;
        public readonly YieldCurve DividendCurve;

        #endregion
    }
}
