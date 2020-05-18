using System;
using System.Collections.Generic;
using OptionPricing;

namespace Stochastics
{
    public abstract class StochasticEngine
    {

        #region constructor 

        public StochasticEngine(Sampler sampler)
        {
            Sampler = sampler;
        }

        #endregion

        #region properties

        protected readonly Sampler Sampler;

        #endregion

        #region abstract methods

        public abstract List<double> GetGeometricBrownianSeries(GbmParameters parameters, double timestep, int length);
        public abstract List<double> GetBrownianMotionSeries(GbmParameters parameters, double timestep, int length);
        public abstract List<double> GetStandardBrownianMotionSeries(double timestep, int length);
        public abstract double GetOptionValue(EquityOption option, DateTime currentDate, 
                                              GbmParameters gbmParams, double interestRate,
                                              int numberSims);

        #endregion

    }

    public class GbmParameters
    {
        #region properties

        public readonly double Mean;
        public readonly double Vol;
        public readonly double InitialVal;

        #endregion

        public GbmParameters(double mean,
                             double vol,
                             double initialVal = 0)
        {
            Mean = mean;
            Vol = vol;
            InitialVal = initialVal;
        }
    }
}
