using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.MarketData;
using Utilities.ExtenstionMethods;

namespace OptionPricing
{
    public abstract class EquityOption
    {
        #region constructor

        public EquityOption(string underlying,
                            DateTime expiryDate,
                            double strike,
                            bool isCall,
                            string discountCurve)
        {
            _underlying = underlying;
            _expiryDate = expiryDate;
            _strike = strike;
            _isCall = isCall;
            _discountCurve = discountCurve;
        }

        #endregion

        #region fields

        protected readonly string _underlying;
        protected readonly DateTime _expiryDate;
        protected readonly double _strike;
        protected readonly bool _isCall;
        protected readonly string _discountCurve;

        #endregion

        #region concrete methods

        public double GetUnderlyingForwardPrice(DailyMarketData marketData,
                                                DateTime forwardDate)
        {
            if (forwardDate <= marketData.Date)
            {
                return 0;
            }

            var timePeriod = marketData.Date.GetYearFractionTo(forwardDate);
            var currentIndexPrice = marketData.IndexData[_underlying].Level;
            var dividendYield = marketData.IndexData[_underlying].DividendCurve.GetSpotRate(timePeriod);
            var interestRate = marketData.IndexData[_underlying].DividendCurve.GetSpotRate(timePeriod);

            var fwdPrice = GetUnderlyingForwardPrice(currentIndexPrice, dividendYield, interestRate, timePeriod);
            return fwdPrice;
        }

        public double GetUnderlyingForwardPrice(double currentPrice,
                                                   double dividendYield,
                                                   double interestRate,
                                                   double timePeriod)
        {
            var fwdFactor = Math.Exp((interestRate - dividendYield) * timePeriod);
            var fwdPrice = currentPrice * fwdFactor;
            return fwdPrice;
        }

        #endregion

        #region abstract methods

        public abstract double GetPayoff(Dictionary<string, SortedList<DateTime, double>> underlyingValues);

        #endregion 

    }
}
