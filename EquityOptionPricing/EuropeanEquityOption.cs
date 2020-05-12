using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

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
                                      double currentPrice,
                                      double interestRate,
                                      double divYield,
                                      double vol)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = (Math.Log(currentPrice / Strike) + (interestRate - divYield + 0.5 * Math.Pow(vol, 2)) * timePeriod) / 
                (vol * Math.Pow(timePeriod, 0.5));
            var d2 = d1 - vol * Math.Pow(timePeriod, 0.5);
            var discountedStrike = Math.Exp(-interestRate * timePeriod) * Strike;
            var divAdjustedCurrentPrice = Math.Exp(-divYield * timePeriod) * currentPrice;

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
                               double currentPrice,
                               double interestRate,
                               double divYield,
                               double vol)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, currentPrice, interestRate, divYield, vol);

            if (IsCall)
            {
                return Math.Exp(-divYield * timePeriod) * Normal.CDF(0, 1, d1);
            }
            else
            {
                return Math.Exp(-divYield * timePeriod) * (Normal.CDF(0, 1, d1) - 1);
            }
        }

        public double GetGamma(DateTime currentDate,
                               double currentPrice,
                               double interestRate,
                               double divYield,
                               double vol)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, currentPrice, interestRate, divYield, vol);
            var gamma = Math.Exp(-divYield * timePeriod) * Normal.PDF(0, 1, d1) / (currentPrice * vol * Math.Sqrt(timePeriod));
            return gamma;
        }

        public double GetVega(DateTime currentDate,
                              double currentPrice,
                              double interestRate,
                              double divYield,
                              double vol)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, currentPrice, interestRate, divYield, vol);
            var vega = Math.Exp(-divYield * timePeriod) * currentPrice * Math.Sqrt(timePeriod) * Normal.PDF(0, 1, d1);
            return vega;
        }

        public double GetRho(DateTime currentDate,
                             double currentPrice,
                             double interestRate,
                             double divYield,
                             double vol)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d2 = GetD2(timePeriod, currentPrice, interestRate, divYield, vol);
            if (IsCall)
            {
                return Math.Exp(-interestRate * timePeriod) * timePeriod * Strike * Normal.CDF(0, 1, d2);
            }
            else
            {
                return Math.Exp(-interestRate * timePeriod) * timePeriod * Strike * (Normal.CDF(0, 1, d2) - 1);
            }
        }

        public double GetTheta(DateTime currentDate,
                               double currentPrice,
                               double interestRate,
                               double divYield,
                               double vol)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            var d1 = GetD1(timePeriod, currentPrice, interestRate, divYield, vol);
            var d2 = GetD2(timePeriod, currentPrice, interestRate, divYield, vol);

            if (IsCall)
            {
                var theta = -Math.Exp(-divYield * timePeriod) * currentPrice * Normal.PDF(0, 1, d1) * vol / (2 * Math.Sqrt(timePeriod))
                    - interestRate * Strike * Math.Exp(-interestRate * timePeriod) * Normal.CDF(0, 1, d2)
                    + divYield * currentPrice * Math.Exp(-divYield * timePeriod) * Normal.CDF(0, 1, d1);
                return theta;
            }
            else
            {
                var theta = -Math.Exp(-divYield * timePeriod) * currentPrice * Normal.PDF(0, 1, -d1) * vol / (2 * Math.Sqrt(timePeriod))
                    - interestRate * Strike * Math.Exp(-interestRate * timePeriod) * (Normal.CDF(0, 1, d2) - 1)
                    + divYield * currentPrice * Math.Exp(-divYield * timePeriod) * (Normal.CDF(0, 1, d1) - 1);
                return theta;
            }
        }

        #region private methods

        private double GetD1(double timePeriod, 
                             double currentPrice, 
                             double interestRate,
                             double divYield,
                             double vol)
        {
            var d1 = (Math.Log(currentPrice / Strike) + (interestRate - divYield + 0.5 * Math.Pow(vol, 2)) * timePeriod) /
                (vol * Math.Pow(timePeriod, 0.5));
            return d1;
        }

        private double GetD2(double timePeriod,
                             double currentPrice,
                             double interestRate,
                             double divYield,
                             double vol)
        {
            var d1 = GetD1(timePeriod, currentPrice, interestRate, divYield, vol);
            var d2 = d1 - vol * Math.Sqrt(timePeriod);
            return d2;
        }

        #endregion



        #endregion

        #region overrides

        public override double GetPayoff(Dictionary<string, SortedList<DateTime, double>> underlyingValues)
        {
            var indexPrices = underlyingValues[Underlying];
            var priceAtExpiry = indexPrices[ExpiryDate];

            if (IsCall)
            {
                return Math.Max(0, priceAtExpiry - Strike);
            }
            else
            {
                return Math.Max(0, Strike - priceAtExpiry);
            }
            
        }

        #endregion

    }
}
