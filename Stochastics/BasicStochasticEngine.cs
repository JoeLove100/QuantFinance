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

        public BasicStochasticEngine(IRandomNumberGenerator randomNumberGenerator):
            base(randomNumberGenerator)
        {

        }

        public override List<double> GetBrownianMotionSeries(double mean, double vol, int length)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region overrides

        public override List<double> GetGeometricBrownianSeries(double mean, 
                                                                double vol, 
                                                                int length)
        {
            var gbSeries = new List<double>();



            return gbSeries;
        }

        public override List<double> GetStandardBrownianMotionSeries(int length)
        {
            var sbmSeries = new List<double>();
            var prev = 0;

            for(int i = 0; i < length; i++)
            {
                var current = 0; 
            }
            

            return sbmSeries;
        }

        #endregion
    }

}
