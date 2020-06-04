using System;
using System.Windows.Controls;
using OptionPricing;


namespace ChartingApp
{
    /// <summary>
    /// Interaction logic for OptionControl.xaml
    /// </summary>
    public partial class OptionControl : UserControl
    {
        #region constructor

        public OptionControl()
        {
            InitializeComponent();
            InitialiseControls();
        }

        #endregion

        #region methods

        private void InitialiseControls()
        {
            OptionTypeCombo.ItemsSource = Enum.GetValues(typeof(OptionType));
        }

        public void SetOption(OptionType optionType,
                              double optionStrike,
                              DateTime expiryDate)
        {
            OptionTypeCombo.SelectedItem = optionType;
            StrikeDoubleBox.Value = optionStrike;
            ExpiryDatePicker.SelectedDate = expiryDate;
        }

        public EquityOption GetOption()
        {
            var optionType = (OptionType) OptionTypeCombo.SelectedValue;
            var strike = (double) StrikeDoubleBox.Value;
            var expiryDate = (DateTime) ExpiryDatePicker.SelectedDate;
            return EquityOptionFactory.GetOption(optionType, "Underlying", expiryDate, strike, "Discount curve");
        }

        #endregion 
    }
}
