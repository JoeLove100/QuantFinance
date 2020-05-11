using System;
using System.Collections.Generic;
using System.Linq;
using OptionPricing;
using Utilities.ExtenstionMethods;

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

        public override List<double> GetBrownianMotionSeries(GbmParameters parameters,
                                                             double timeStep, 
                                                             int length)
            ///<summary>
            /// Returns a list of size length consisting of observations of 
            /// a brownian motion 
            /// <paramref name="mean"/> annual drift of the process
            /// <paramref name="vol"/> annual standard deviation of modelled returns
            /// <paramref name="timeStep"/> period in years between each observation
            /// <paramref name="length"/> number of observations to be generated
            ///</summary>
        {
            var brownianMotion = new List<double>();
            var standardNormalNumbers = Sampler.GetRandStandardNormal(length);
            double prev = 0;
            
            foreach(double x in standardNormalNumbers)
            {
                var brownian = prev + parameters.Mean * timeStep + parameters.Vol * x * Math.Sqrt(timeStep);
                prev = brownian;
                brownianMotion.Add(prev);
            }

            return brownianMotion;
            
        }

        #endregion

        #region overrides

        public override List<double> GetGeometricBrownianSeries(GbmParameters parameters,
                                                                double timestep,
                                                                int length)
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
            var brownianSeries = GetBrownianMotionSeries(geomParameters, timestep, length);

            foreach(double brownianVal in brownianSeries)
            {
                var geomBrownianVal = parameters.InitialVal * Math.Exp(brownianVal);
                gbSeries.Add(geomBrownianVal);
            }

            return gbSeries;
        }

        public override List<double> GetStandardBrownianMotionSeries(double timestep, 
                                                                     int length)
        {
            var standardParameters = new GbmParameters(0, 1);
            return GetBrownianMotionSeries(standardParameters, timestep, length);
        }

        public override double GetOptionValue(EquityOption option,
                                              DateTime currentDate,
                                              Dictionary<string, GbmParameters> parametersByUnderlying,
                                              double discountFactor,
                                              int numberSims)
            ///<summary>
            /// simulate the payoff of the 
            ///</summary>
        {
            var simulatedValues = new List<double>();
            var simulationLengtDays = currentDate.GetWorkingDaysTo(option.ExpiryDate);

            for (int i = 0; i < numberSims; i++)
            {
                var underlyingProjections = new Dictionary<string, SortedList<DateTime, double>>();
                foreach (KeyValuePair<string, GbmParameters> parameters in parametersByUnderlying)
                {
                    var rawStockPrices = GetGeometricBrownianSeries(parameters.Value, DailyTimePeriod, simulationLengtDays);
                    underlyingProjections.Add(parameters.Key, rawStockPrices.AsBDTimeSeries(currentDate));
                }

                var optionPayoff = option.GetPayoff(underlyingProjections);
                simulatedValues.Add(optionPayoff);
            }

            var avgPayoff = simulatedValues.Sum() / numberSims;
            var discountedAvgPayoff = avgPayoff * discountFactor;
            return discountedAvgPayoff;
        }

        #endregion
    }

}
