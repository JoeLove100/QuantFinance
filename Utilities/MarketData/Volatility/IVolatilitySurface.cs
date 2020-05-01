using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Calculations.Interpolations;

namespace Utilities.MarketData.Volatility
{
    public interface IVolatilitySurface
    {
        double GetVolatility(double tenor, double moneyness);
    }
}
