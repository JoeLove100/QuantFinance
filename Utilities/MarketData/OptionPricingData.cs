namespace Utilities.MarketData
{
    public class OptionPricingData
    {
        #region constructor

        public OptionPricingData(double currentPrice,
                                 double vol,
                                 double interestRate,
                                 double divYield)
        {
            CurrentPrice = currentPrice;
            Vol = vol;
            InterestRate = interestRate;
            DivYield = divYield;
        }

        #endregion

        #region properties

        public readonly double CurrentPrice;
        public readonly double Vol;
        public readonly double InterestRate;
        public readonly double DivYield;

        #endregion
    }
}
