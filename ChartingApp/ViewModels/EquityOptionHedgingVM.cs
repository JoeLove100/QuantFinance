using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Series;
using OptionPricing;
using Utilities.MarketData;
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;
using Stochastics;
using Stochastics.Strategies;
using Utilities.ExtenstionMethods;

namespace ChartingApp.ViewModels
{
    public class EquityOptionHedgingVM: INotifyPropertyChanged
    {

        #region constructor

        public EquityOptionHedgingVM(EquityOption option,
                                     StochasticEngine stochasticEngine,
                                     HedgingStrategy hedgingStrategy)
        {
            Option = option;
            StochasticEngine = stochasticEngine;
            HedgingStrategy = hedgingStrategy;
            PlotModel = new PlotModel();
        }

        public EquityOptionHedgingVM(string inputDataCsv)
        {
            PlotModel = new PlotModel();
            PricingData = ReadOptionPricingData(inputDataCsv);
        }

        #endregion

        #region events

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        #endregion 

        #region properties

        private readonly SortedList<DateTime, OptionPricingData> PricingData;
        private EquityOption Option { get; }
        private StochasticEngine StochasticEngine { get; }
        private HedgingStrategy HedgingStrategy { get; }
        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { _plotModel = value; OnPropertyChanged("PlotModel"); }
        }
        #endregion

        #region public methods

        public void PlotDailyPnL(DateTime startDate,
                                 double initialStockPrice,
                                 double constantVol,
                                 double constantInterest,
                                 double constantDivYield)
        {
            var gbmParameters = new GbmParameters(constantInterest - constantDivYield, constantVol, initialStockPrice);
            var pricingData = GenerateOptionPricingData(startDate, gbmParameters, constantVol, constantInterest, constantDivYield);
            var pnl = HedgingStrategy.GetDailyPnl(pricingData);

            var optionSeries = new ColumnSeries() { IsStacked=true};
            var hedgeSeries = new ColumnSeries() { IsStacked = true };
            var cashSeries = new ColumnSeries() { IsStacked = true };
            foreach (KeyValuePair<DateTime, (double, double, double)> dailyPnl in pnl)
            {
                // TODO: plot different PnL elements seperately
                (double optionPnl, double hedgePnl, double cashPnl) = dailyPnl.Value;
                optionSeries.Items.Add(new ColumnItem(optionPnl));
                hedgeSeries.Items.Add(new ColumnItem(hedgePnl));
                cashSeries.Items.Add(new ColumnItem(cashPnl));
            }

            PlotModel.Series.Clear();
            PlotModel.Series.Add(optionSeries);
            PlotModel.Series.Add(hedgeSeries);
            PlotModel.Series.Add(cashSeries);
        }

        #endregion 


        #region private methods

        private SortedList<DateTime, OptionPricingData> ReadOptionPricingData(string inputDataCsv)
        {
            var pricingData = new SortedList<DateTime, OptionPricingData>();
            var counter = 0;

            using (TextFieldParser csvParser = new TextFieldParser(inputDataCsv))
            {
                csvParser.SetDelimiters(new string[] { "," });
                csvParser.ReadLine();  // skip line with headers
                while (!csvParser.EndOfData)
                {
                    string[] fields = csvParser.ReadFields();
                    var date = DateTime.Parse(fields[0]);
                    var currentPrice = double.Parse(fields[1]);
                    var vol = double.Parse(fields[2]);
                    var interestRate = double.Parse(fields[3]);
                    var divYield = double.Parse(fields[4]);

                    pricingData.Add(date, new OptionPricingData(currentPrice, vol, interestRate, divYield));
                    counter++;
                }
            }

            return pricingData;
        }

        private SortedList<DateTime, OptionPricingData> GenerateOptionPricingData(DateTime startDate,
                                                                                  GbmParameters gbmParams,
                                                                                  double vol,
                                                                                  double interestRate,
                                                                                  double divYield)
        {
            int days = startDate.GetWorkingDaysTo(Option.ExpiryDate);
            var stockPrices = StochasticEngine.GetGeometricBrownianSeries(gbmParams, 1.0 / TimePeriods.BusinessDaysInYear, days);
            var pricingData = new SortedList<DateTime, OptionPricingData>();
            var date = startDate;

            if (!date.IsWorkingDay())
            {
                date = date.AddWorkingDay();
            }

            foreach(double price in stockPrices)
            {
                pricingData.Add(date, new OptionPricingData(price, vol, interestRate, divYield));
                date = date.AddWorkingDay();
            }

            return pricingData;
        }
        #endregion

    }
}
