using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract List<double> GetGeometricBrownianSeries(double mean, double vol, double initialVal, double timestep, int length);
        public abstract List<double> GetBrownianMotionSeries(double mean, double vol, double timestep, int length);
        public abstract List<double> GetStandardBrownianMotionSeries(double timestep, int length);

        #endregion



    }
}
