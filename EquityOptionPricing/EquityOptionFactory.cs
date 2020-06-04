using System;

namespace OptionPricing
{
    public static class EquityOptionFactory
    {
        public static EquityOption GetOption(OptionType optionType, 
                                             string underlying,
                                             DateTime expiryDate,
                                             double strike,
                                             string discountCurve)
        {
            switch (optionType)
            {
                case OptionType.EuropeanCall:
                    return new EuropeanEquityOption(underlying, expiryDate, strike, true, discountCurve);
                case OptionType.EuropeanPut:
                    return new EuropeanEquityOption(underlying, expiryDate, strike, true, discountCurve);
                case OptionType.DigitalCall:
                    return new DigitalOption(underlying, expiryDate, strike, true, discountCurve, false);
                case OptionType.DigitalPut:
                    return new DigitalOption(underlying, expiryDate, strike, true, discountCurve, false);
                default:
                    throw new NotImplementedException($"No option defined for {optionType.ToString()}");
            }
        }
    }
}
