using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using Utilities.MarketData;
using Utilities.ExtenstionMethods;

namespace OptionPricing
{
    public class EuropeanEquityOption: EquityOption
    {

        #region constructors

        public EuropeanEquityOption(string underlying,
                            DateTime expiryDate,
                            double strike,
                            bool isCall,
                            string discountCurve) : base(underlying, expiryDate, strike, isCall, discountCurve)
        {

        }

        #endregion

        #region public methods

        public double GetPriceBSModel(DateTime currentDate,
                                      OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, pricingData);
            var d2 = GetD2(timePeriod, pricingData);

            var discountedStrike = Math.Exp(-pricingData.InterestRate * timePeriod) * Strike;
            var divAdjustedCurrentPrice = Math.Exp(-pricingData.DivYield * timePeriod) * pricingData.CurrentPrice;

            if (IsCall)
            {
                var price = divAdjustedCurrentPrice * Normal.CDF(0, 1, d1) - discountedStrike * Normal.CDF(0, 1, d2);
                return price;
            }
            else
            {
                var price = discountedStrike * Normal.CDF(0, 1, -d2) - divAdjustedCurrentPrice * Normal.CDF(0, 1, -d1);
                return price;
            }
            
        }

        public double GetDelta(DateTime currentDate,
                               OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, pricingData);

            if (IsCall)
            {
                return Math.Exp(-pricingData.DivYield * timePeriod) * Normal.CDF(0, 1, d1);
            }
            else
            {
                return Math.Exp(-pricingData.DivYield * timePeriod) * (Normal.CDF(0, 1, d1) - 1);
            }
        }

        public double GetGamma(DateTime currentDate,
                               OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, pricingData);
            var gamma = Math.Exp(-pricingData.DivYield * timePeriod) * Normal.PDF(0, 1, d1) / 
                (pricingData.CurrentPrice * pricingData.Vol * Math.Sqrt(timePeriod));
            return gamma;
        }

        public double GetVega(DateTime currentDate,
                              OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, pricingData);
            var vega = Math.Exp(-pricingData.DivYield * timePeriod) * pricingData.CurrentPrice 
                * Math.Sqrt(timePeriod) * Normal.PDF(0, 1, d1) / 100;
            return vega;
        }

        public double GetRho(DateTime currentDate,
                             OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d2 = GetD2(timePeriod, pricingData);
            if (IsCall)
            {
                return Math.Exp(-pricingData.InterestRate * timePeriod) * timePeriod * Strike * Normal.CDF(0, 1, d2) / 100;
            }
            else
            {
                return Math.Exp(-pricingData.InterestRate * timePeriod) * timePeriod * Strike * (Normal.CDF(0, 1, d2) - 1) / 100;
            }
        }

        public double GetTheta(DateTime currentDate,
                               OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, pricingData);
            var d2 = GetD2(timePeriod, pricingData);

            if (IsCall)
            {
                var term1 = -Math.Exp(-pricingData.DivYield * timePeriod) * pricingData.CurrentPrice * Normal.PDF(0, 1, d1) 
                    * pricingData.Vol / (2 * Math.Sqrt(timePeriod));
                var term2 = pricingData.InterestRate * Strike * Math.Exp(-pricingData.InterestRate * timePeriod)
                    * Normal.CDF(0, 1, d2);
                var term3 = pricingData.DivYield * pricingData.CurrentPrice * Math.Exp(-pricingData.DivYield * timePeriod) 
                    * Normal.CDF(0, 1, d1);
                var theta = term1 - term2 + term3;

                return theta / TimePeriods.BusinessDaysInYear;
            }
            else
            {
                var term1 = -Math.Exp(-pricingData.DivYield * timePeriod) * pricingData.CurrentPrice * Normal.PDF(0, 1, -d1)
                    * pricingData.Vol / (2 * Math.Sqrt(timePeriod));
                var term2 = pricingData.InterestRate * Strike * Math.Exp(-pricingData.InterestRate * timePeriod)
                    * (Normal.CDF(0, 1, d2) - 1);
                var term3 = pricingData.DivYield * pricingData.CurrentPrice * Math.Exp(-pricingData.DivYield * timePeriod) 
                    * (Normal.CDF(0, 1, d1) - 1);
                var theta = term1 - term2 + term3;

                return theta / TimePeriods.BusinessDaysInYear;
            }
        }

        #region private methods



        #endregion



        #endregion

        #region overrides

        public override double GetPayoff(SortedList<DateTime, double> prices)
        {
            var priceAtExpiry = prices[ExpiryDate];

            if (IsCall)
            {
                return Math.Max(0, priceAtExpiry - Strike);
            }
            else
            {
                return Math.Max(0, Strike - priceAtExpiry);
            }
            
        }
        
        public override double GetCurrentPrice(DateTime currentDate, SortedList<DateTime, OptionPricingData> pricingData)
        {
            if (currentDate == ExpiryDate)
            {
                return GetPayoff(pricingData.GetPriceSeries());
            }
            else
            {
                return GetPriceBSModel(currentDate, pricingData[currentDate]);
            }
        }

        public override double GetCurrentDelta(DateTime currentDate, SortedList<DateTime, OptionPricingData> pricingData)
        {
            return GetDelta(currentDate, pricingData[currentDate]);
        }

        public override double GetCurrentGamma(DateTime currentDate, SortedList<DateTime, OptionPricingData> pricingData)
        {
            return GetGamma(currentDate, pricingData[currentDate]);
        }

        #endregion

    }
}
