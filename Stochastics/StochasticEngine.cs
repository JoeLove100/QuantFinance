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

        public StochasticEngine(IRandomNumberGenerator randGenerator)
        {
            RandomNumerGenerator = randGenerator;
        }

        #endregion

        #region properties

        protected readonly IRandomNumberGenerator RandomNumerGenerator;

        #endregion

        #region abstract methods

        public abstract List<double> GetGeometricBrownianSeries(double mean, double vol, int length);
        public abstract List<double> GetBrownianMotionSeries(double mean, double vol, int length);
        public abstract List<double> GetStandardBrownianMotionSeries(int length);

        #endregion



    }
}
