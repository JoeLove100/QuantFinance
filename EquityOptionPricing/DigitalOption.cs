﻿using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using Utilities.MarketData;
using Utilities.ExtenstionMethods;

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

        public double GetPriceBSModel(DateTime currentDate,
                                      OptionPricingData pricingData)
        {
            var timePeriod = GetTimePeriodToExpiry(currentDate);
            double optionPrice;

            var currentPrice = pricingData.CurrentPrice;
            var interestRate = pricingData.InterestRate;
            var divYield = pricingData.DivYield;
            var vol = pricingData.Vol;

            if (_isAssetSettled)
            {
                var divFactor = Math.Exp(-divYield * timePeriod);
                var d1 = GetD1(timePeriod, pricingData);
                if (IsCall)
                {
                    optionPrice = currentPrice * Normal.CDF(0, 1, d1) * divFactor;
                }
                else
                {
                    optionPrice = currentPrice * Normal.CDF(0, 1, -d1) * divFactor;
                }
            }
            else
            {
                var discountFactor = Math.Exp(-interestRate * timePeriod);
                var d2 = GetD2(timePeriod, pricingData);
                if (IsCall)
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

        public override double GetPayoff(SortedList<DateTime, double> prices)
        {
            var priceAtExpiry = prices[ExpiryDate];

            if (IsCall)
            {
                if (priceAtExpiry > Strike)
                {
                    return _isAssetSettled ? priceAtExpiry : 1;
                }
                else return 0;
            }
            else
            {
                if (priceAtExpiry < Strike)
                {
                    return _isAssetSettled ? priceAtExpiry : 1;
                }
                else return 0;
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

        #endregion 
    }
}
