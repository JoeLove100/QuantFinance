using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace OptionPricing
{
    public class DigitalOption : EquityOption
    {
        #region constructor

        public DigitalOption(string underlying, 
                             DateTime expiryDate, 
                             double strike, 
                             bool isCall, 
                             string discountCurve,
                             bool isAssetSettled) : base(underlying, expiryDate, strike, isCall, discountCurve)
        {
            _isAssetSettled = isAssetSettled;
        }

        #region fields

        private readonly bool _isAssetSettled;

        #endregion

        #endregion

        #region public methods

        public double GetPriceBSModel(double timePeriod,
                                      double currentPrice,
                                      double interestRate,
                                      double divYield,
                                      double vol)
        {
            double optionPrice;
            var discountFactor = Math.Exp(-interestRate * timePeriod);

            if (_isAssetSettled)
            {
                var d1 = (Math.Log(currentPrice / _strike) + (interestRate - divYield + 0.5 * Math.Pow(vol, 2)) * timePeriod) /
                    (vol * Math.Sqrt(timePeriod));
                if (_isCall)
                {
                    optionPrice = currentPrice * Normal.CDF(0, 1, d1) * discountFactor;
                }
                else
                {
                    optionPrice = currentPrice * Normal.CDF(0, 1, -d1) * discountFactor;
                }
            }
            else
            {

                var d2 = (Math.Log(currentPrice / _strike) + (interestRate - divYield - 0.5 * Math.Pow(vol, 2)) * timePeriod) /
                    (vol * Math.Sqrt(timePeriod));
                if (_isCall)
                {
                    optionPrice = Normal.CDF(0, 1, d2) * discountFactor;
                }
                else
                {
                    optionPrice = Normal.CDF(0, 1, -d2) * discountFactor;
                }
            }

            return optionPrice;
        }

        #endregion

        #region overrides

        public override double GetPayoff(Dictionary<string, SortedList<DateTime, double>> underlyingValues)
        {
            var indexPrices = underlyingValues[_underlying];
            var priceAtExpiry = indexPrices[_expiryDate];

            if (_isCall)
            {
                if (priceAtExpiry > _strike)
                {
                    return _isAssetSettled ? priceAtExpiry : 1;
                }
                else return 0;
            }
            else
            {
                if (priceAtExpiry < _strike)
                {
                    return _isAssetSettled ? priceAtExpiry : 1;
                }
                else return 0;
            }
        }

        #endregion 
    }
}
