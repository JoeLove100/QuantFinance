using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.MarketData.InterestRates
{
    public interface IInterestRateCurve
    {
        double GetSpotRate(double tenor);
        double GetAnnualisedForwardRate(double startTenor, double endTenor);
    }
}
