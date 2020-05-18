using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OptionPricing;
using Utilities.MarketData;

namespace Stochastics.Strategies
{
    public abstract class OptionHedgingStrategy
    {
        #region contructors

        public OptionHedgingStrategy(EquityOption option,
                                     int contracts)
        {
            _option = option;
            _contractNumbers = contracts;
        }

        #endregion

        #region properties

        protected readonly EquityOption _option;
        protected readonly int _contractNumbers;

        #endregion

        #region abstract methods

        public abstract SortedList<DateTime,double> GetDailyPnl(SortedList<DateTime, OptionPricingData> marketData);

        #endregion
    }
}
