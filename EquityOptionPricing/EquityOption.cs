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
            Underlying = underlying;
            ExpiryDate = expiryDate;
            Strike = strike;
            IsCall = isCall;
            DiscountCurve = discountCurve;
        }

        #endregion

        #region fields

        public readonly string Underlying;
        public readonly DateTime ExpiryDate;
        public readonly double Strike;
        public readonly bool IsCall;
        public readonly string DiscountCurve;

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
            var currentIndexPrice = marketData.IndexData[Underlying].Level;
            var dividendYield = marketData.IndexData[Underlying].DividendCurve.GetSpotRate(timePeriod);
            var interestRate = marketData.IndexData[Underlying].DividendCurve.GetSpotRate(timePeriod);

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

        public double GetTimePeriodToExpiry(DateTime currentDate)
        {
            var timePeriod = (double) currentDate.GetWorkingDaysTo(ExpiryDate) / TimePeriods.BusinessDaysInYear;
            return timePeriod;

        }

        protected double GetD1(double timePeriod,
                               OptionPricingData pricingData)
        {

            var d1 = (Math.Log(pricingData.CurrentPrice / Strike) +
                (pricingData.InterestRate - pricingData.DivYield + 0.5 * Math.Pow(pricingData.Vol, 2)) * timePeriod) /
                (pricingData.Vol * Math.Pow(timePeriod, 0.5));
            return d1;
        }

        protected double GetD2(double timePeriod,
                               OptionPricingData pricingData)
        {
            var d1 = GetD1(timePeriod, pricingData);
            var d2 = d1 - pricingData.Vol * Math.Sqrt(timePeriod);
            return d2;
        }

        #endregion

        #region abstract methods

        public abstract double GetPayoff(SortedList<DateTime, double> prices);
        public abstract double GetCurrentPrice(DateTime currentDate, SortedList<DateTime, OptionPricingData> pricingData);

        #endregion 

    }
}
