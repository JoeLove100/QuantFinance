using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

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

        public override List<double> GetBrownianMotionSeries(double mean, 
                                                             double vol, 
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
                var brownian = prev + mean * timeStep + vol * x * Math.Sqrt(timeStep);
                prev = brownian;
                brownianMotion.Add(prev);
            }

            return brownianMotion;
            
        }

        #endregion

        #region overrides

        public override List<double> GetGeometricBrownianSeries(double mean, 
                                                                double vol,
                                                                double initialVal, 
                                                                double timestep,
                                                                int length)
            ///<summary>
            /// generates a series of observations of a geometric brownian 
            /// variable for projecting investable assets
            /// <paramref name="mean"/> annual drift of the process
            /// <paramref name="vol"/> annual standard deviation of modelled returns
            /// <paramref name="initialVal"/> starting value for the asset 
            /// <paramref name="timeStep"/> period in years between each observation
            /// <paramref name="length"/> number of observations to be generated
            ///</summary>
        {
            var gbSeries = new List<double>();
            var brownianSeries = GetBrownianMotionSeries(mean - 0.5 * Math.Pow(vol, 2), vol, timestep, length);

            foreach(double brownianVal in brownianSeries)
            {
                var geomBrownianVal = initialVal * Math.Exp(brownianVal);
                gbSeries.Add(geomBrownianVal);
            }

            return gbSeries;
        }

        public override List<double> GetStandardBrownianMotionSeries(double timestep, 
                                                                     int length)
        {
            return GetBrownianMotionSeries(0, 1, timestep, length);
        }

        #endregion
    }

}
