using System.ComponentModel;

namespace OptionPricing
{
    public enum OptionType
    {
        [Description("European Call")]
        EuropeanCall = 1,
        [Description("European Put")]
        EuropeanPut = 2,
        [Description("Binary Call")]
        DigitalCall = 3,
        [Description("Binary Put")]
        DigitalPut = 4
    }
}
