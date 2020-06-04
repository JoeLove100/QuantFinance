using System;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Annotations;
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

        public EquityOptionHedgingVM(StochasticEngine stochasticEngine)
        {
            StochasticEngine = stochasticEngine;

            PlotStockPrice = new PlotModel();
            PlotModelDaily = new PlotModel();
            PlotModelCumulative = new PlotModel();
        }

        public EquityOptionHedgingVM(string inputDataCsv)
        {
            PlotModelDaily = new PlotModel();
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
        private StochasticEngine StochasticEngine { get; }

        private PlotModel _plotStockPrice;
        public PlotModel PlotStockPrice
        {
            get { return _plotStockPrice; }
            set { _plotStockPrice = value; OnPropertyChanged("PlotStockPrice"); }
        }

        private PlotModel _plotModelDaily;
        public PlotModel PlotModelDaily
        {
            get { return _plotModelDaily; }
            set { _plotModelDaily = value; OnPropertyChanged("PlotModelDaily"); }
        }

        private PlotModel _plotModelCumulative;
        public PlotModel PlotModelCumulative
        {
            get { return _plotModelCumulative; }
            set { _plotModelCumulative = value; OnPropertyChanged("PlotModelCumulative"); }
        }
        #endregion

        #region public methods

        public void RefreshCharts()
        {
            PlotStockPrice.InvalidatePlot(true);
            PlotModelDaily.InvalidatePlot(true);
            PlotModelCumulative.InvalidatePlot(true);
        }

        public void ClearCharts()
        {
            PlotStockPrice.Series.Clear();
            PlotModelDaily.Series.Clear();
            PlotModelCumulative.Series.Clear();
        }

        public void PlotDailyPnL(DateTime startDate,
                                 OptionPricingData basicPricingData,
                                 List<HedgingStrategy> hedgingStrategies)
        {
            ClearCharts();

            var gbmParameters = new GbmParameters(basicPricingData);
            var pricingData = GenerateOptionPricingData(startDate, hedgingStrategies[0].ExpiryDate, gbmParameters, basicPricingData);

            var stockSeries = new LineSeries() { Title = @"Stock Price" };
            for(int i = 0;  i < pricingData.Count; i ++)
            {
                stockSeries.Points.Add(new DataPoint(i, pricingData.Values[i].CurrentPrice));
            }

            PlotStockPrice.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Minimum = 0
            });

            PlotStockPrice.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = 55,
                Maximum = 65
            });

            PlotStockPrice.Series.Add(stockSeries);

            var strikeAnnotation = new LineAnnotation() { Y = 60, MinimumX=0, MaximumX=65, Color=OxyColors.Red ,
            LineStyle=LineStyle.Solid, Type=LineAnnotationType.Horizontal};
            PlotStockPrice.Annotations.Add(strikeAnnotation);

            foreach (HedgingStrategy strategy in hedgingStrategies)
            {

                var pnl = strategy.GetDailyPnl(pricingData);

                var dailyPnlSeries = new ColumnSeries() { Title = $"{strategy.Name} - Daily PnL" };
                var cumulativeSeries = new ColumnSeries() { Title = $"{strategy.Name} - Cumulative PnL" };
                var cumulativePnl = new List<double>();

                foreach (KeyValuePair<DateTime, (double, double, double)> dailyPnl in pnl)
                {
                    // TODO: plot different PnL elements seperately
                    (double optionPnl, double hedgePnl, double cashPnl) = dailyPnl.Value;
                    var totalPnl = optionPnl + hedgePnl + cashPnl;
                    dailyPnlSeries.Items.Add(new ColumnItem(totalPnl));
                    if (cumulativePnl.Count == 0)
                    {
                        cumulativePnl.Add(totalPnl);
                    }
                    else
                    {
                        cumulativePnl.Add(cumulativePnl[cumulativePnl.Count - 1] + totalPnl);
                    }
                }

                foreach (double dailyVal in cumulativePnl)
                {
                    cumulativeSeries.Items.Add(new ColumnItem(dailyVal));
                }

                PlotModelDaily.Series.Add(dailyPnlSeries);
                PlotModelCumulative.Series.Add(cumulativeSeries);

                RefreshCharts();
            }
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
                                                                                  DateTime expiryDate,
                                                                                  GbmParameters gbmParams,
                                                                                  OptionPricingData basicPricingData)
        {
            int days = startDate.GetWorkingDaysTo(expiryDate);
            var stockPrices = StochasticEngine.GetGeometricBrownianSeries(gbmParams, 1.0 / TimePeriods.BusinessDaysInYear, days);
            var pricingData = new SortedList<DateTime, OptionPricingData>();
            var date = startDate;

            if (!date.IsWorkingDay())
            {
                date = date.AddWorkingDay();
            }

            foreach(double price in stockPrices)
            {
                pricingData.Add(date, new OptionPricingData(price, basicPricingData.Vol, basicPricingData.InterestRate, basicPricingData.DivYield));
                date = date.AddWorkingDay();
            }

            return pricingData;
        }
        #endregion

    }
}
