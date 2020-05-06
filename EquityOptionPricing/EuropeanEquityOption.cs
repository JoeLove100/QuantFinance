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

        public double GetPriceBSModel(double timePeriod,
                                      double currentPrice,
                                      double interestRate,
                                      double divYield,
                                      double vol)
        {
            var d1 = (Math.Log(currentPrice / _strike) + (interestRate - divYield + 0.5 * Math.Pow(vol, 2)) * timePeriod) / 
                (vol * Math.Pow(timePeriod, 0.5));
            var d2 = d1 - vol * Math.Pow(timePeriod, 0.5);
            var discountedStrike = Math.Exp(-interestRate * timePeriod) * _strike;
            var divAdjustedCurrentPrice = Math.Exp(-divYield * timePeriod) * currentPrice;

            if (_isCall)
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

        #endregion

        #region overrides
        
        public override double GetPayoff(Dictionary<string, SortedList<DateTime, double>> underlyingValues)
        {
            var indexPrices = underlyingValues[_underlying];
            var priceAtExpiry = indexPrices[_expiryDate];

            if (_isCall)
            {
                return Math.Max(0, priceAtExpiry - _strike);
            }
            else
            {
                return Math.Max(0, _strike - priceAtExpiry);
            }
            
        }

        #endregion

    }
}
