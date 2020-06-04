using System;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Annotations;
using OxyPlot.Axes;
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
            PlotStockPrice.Axes.Clear();

            PlotModelDaily.Series.Clear();
            PlotModelDaily.Axes.Clear();

            PlotModelCumulative.Series.Clear();
            PlotModelCumulative.Axes.Clear();
        }

        public void PlotDailyPnL(DateTime startDate,
                                 OptionPricingData basicPricingData,
                                 List<HedgingStrategy> hedgingStrategies,
                                 int? seed = null)
        {
            ClearCharts();

            var gbmParameters = new GbmParameters(basicPricingData);
            var pricingData = GenerateOptionPricingData(startDate, hedgingStrategies[0].ExpiryDate, gbmParameters, basicPricingData, seed);
            var minPrice = double.PositiveInfinity;
            var maxPrice = double.NegativeInfinity;

            var stockSeries = new LineSeries() { Title = @"Stock Price" };
            for(int i = 0;  i < pricingData.Count; i ++)
            {
                var stockPrice = pricingData.Values[i].CurrentPrice;
                stockSeries.Points.Add(new DataPoint(i, stockPrice));
                minPrice = Math.Min(minPrice, stockPrice);
                maxPrice = Math.Max(maxPrice, stockPrice);
            }

            PlotStockPrice.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                TextColor = OxyColors.Transparent
            });

            PlotStockPrice.Axes.Add(new OxyPlot.Axes.LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Minimum = (Math.Floor(minPrice / 5) - 1) * 5,
                Maximum = (Math.Floor(maxPrice / 5) + 1) * 5
            });

            PlotStockPrice.Series.Add(stockSeries);

            var strike = hedgingStrategies[0].Portfolio.Option.Strike;
            var strikeAnnotation = new LineAnnotation() { Y = strike, MinimumX=0, MaximumX=stockSeries.Points.Count, Color=OxyColors.Red ,
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

                PlotModelDaily.Axes.Add(new CategoryAxis()
                {
                    Position = AxisPosition.Bottom,
                    IsAxisVisible = false
                });

                PlotModelCumulative.Axes.Add(new CategoryAxis()
                {
                    Position = AxisPosition.Bottom,
                    IsAxisVisible = false
                });

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
                                                                                  OptionPricingData basicPricingData,
                                                                                  int? seed)
        {
            int days = startDate.GetWorkingDaysTo(expiryDate);
            var stockPrices = StochasticEngine.GetGeometricBrownianSeries(gbmParams, 1.0 / TimePeriods.BusinessDaysInYear, days, seed);
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
