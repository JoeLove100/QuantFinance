using System;
using System.Collections.Generic;
using Utilities.ExtenstionMethods;

namespace Stochastics
{
    public abstract class StochasticEngine
    {
        #region constructor 

        public StochasticEngine(Sampler sampler)
        {
            Sampler = sampler;
        }

        #endregion

        #region properties

        protected readonly Sampler Sampler;

        #endregion

        #region concrete methods

        public SortedList<DateTime, double> GetDailyGBM(double mean,
                                                        double vol,
                                                        double initialVal,
                                                        DateTime startDate, 
                                                        DateTime endDate)
            ///<summary>
            /// utility method to get daily geometric brownian motion of 
            /// an asset between two dates (inclusive)
            ///</summary>
        {
            var workingDates = new List<DateTime>();
            var currentDate = startDate;
            while (currentDate <= endDate)
            {
                workingDates.Add(currentDate);
                currentDate = currentDate.AddWorkingDay();
            }

            var timePeriod = 1.0 / 250.0;
            var gbm = GetGeometricBrownianSeries(mean, vol, initialVal, timePeriod, workingDates.Count);

            var dailyGBM = new SortedList<DateTime, double>();
            for(int i = 0; i < workingDates.Count; i++)
            {
                dailyGBM.Add(workingDates[i], gbm[i]);
            }

            return dailyGBM;
        }

        #endregion

        #region abstract methods

        public abstract List<double> GetGeometricBrownianSeries(double mean, double vol, double initialVal, double timestep, int length);
        public abstract List<double> GetBrownianMotionSeries(double mean, double vol, double timestep, int length);
        public abstract List<double> GetStandardBrownianMotionSeries(double timestep, int length);

        #endregion



    }
}
