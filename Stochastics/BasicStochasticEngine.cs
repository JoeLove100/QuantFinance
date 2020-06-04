using System;
using System.Collections.Generic;
using System.Linq;
using OptionPricing;
using Utilities.ExtenstionMethods;
using Utilities.MarketData;

/// <summary>
/// naive, synchronous implementation of an engine
/// class for stochastic projections
/// </summary>

namespace Stochastics
{
    public class BasicStochasticEngine: StochasticEngine
    {
        #region constructor

        public BasicStochasticEngine(Sampler sampler):
            base(sampler)
        {

        }

        #endregion

        #region overrides

        public override List<double> GetBrownianMotionSeries(GbmParameters parameters,
                                                             double timeStep, 
                                                             int length,
                                                             int? seed = null)
            ///<summary>
            /// Returns a list of size length consisting of observations of 
            /// a brownian motion 
            /// <paramref name="mean"/> annual drift of the process
            /// <paramref name="vol"/> annual standard deviation of modelled returns
            /// <paramref name="timeStep"/> period in years between each observation
            /// <paramref name="length"/> number of observations to be generated
            ///</summary>
        {
            var brownianMotion = new List<double> { 0 };
            var standardNormalNumbers = Sampler.GetRandStandardNormal(length - 1, seed);
            double prev = 0;
            
            foreach(double x in standardNormalNumbers)
            {
                var brownian = prev + parameters.Mean * timeStep + parameters.Vol * x * Math.Sqrt(timeStep);
                prev = brownian;
                brownianMotion.Add(prev);
            }

            return brownianMotion;
            
        }

        public override List<double> GetGeometricBrownianSeries(GbmParameters parameters,
                                                                double timestep,
                                                                int length,
                                                                int? seed = null)
            ///<summary>
            /// generates a series of observations of a geometric brownian 
            /// variable for projecting investable assets
            /// <paramref name="parameters"/> parameters for brownian motion
            /// <paramref name="timeStep"/> period in years between each observation
            /// <paramref name="length"/> number of observations to be generated
            ///</summary>
        {
            var gbSeries = new List<double>();
            var geomParameters = new GbmParameters(parameters.Mean - 0.5 * Math.Pow(parameters.Vol, 2), parameters.Vol);
            var brownianSeries = GetBrownianMotionSeries(geomParameters, timestep, length, seed);

            foreach(double brownianVal in brownianSeries)
            {
                var geomBrownianVal = parameters.InitialVal * Math.Exp(brownianVal);
                gbSeries.Add(geomBrownianVal);
            }

            return gbSeries;
        }

        public override List<double> GetStandardBrownianMotionSeries(double timestep, 
                                                                     int length,
                                                                     int? seed = null)
        {
            var standardParameters = new GbmParameters(0, 1);
            return GetBrownianMotionSeries(standardParameters, timestep, length, seed);
        }

        public override double GetOptionValue(EquityOption option,
                                              DateTime currentDate,
                                              GbmParameters gbmParams,
                                              double discountFactor,
                                              int numberSims)
            ///<summary>
            /// simulate the payoff of the given equity option and 
            /// take the average discounted value as the option value
            ///</summary>
        {
            var simulatedValues = new List<double>();
            var simulationLengthDays = currentDate.GetWorkingDaysTo(option.ExpiryDate) + 1;
            var dailyTimePeriod = 1.0 / TimePeriods.BusinessDaysInYear;

            for (int i = 0; i < numberSims; i++)
            {
                var rawStockPrices = GetGeometricBrownianSeries(gbmParams, dailyTimePeriod, simulationLengthDays);
                var rawStockTimeSeries = rawStockPrices.AsBDTimeSeries(currentDate);
                var optionPayoff = option.GetPayoff(rawStockTimeSeries);
                simulatedValues.Add(optionPayoff);
            }

            var avgPayoff = simulatedValues.Sum() / numberSims;
            var discountedAvgPayoff = avgPayoff * discountFactor;
            return discountedAvgPayoff;
        }

        #endregion
    }

}
