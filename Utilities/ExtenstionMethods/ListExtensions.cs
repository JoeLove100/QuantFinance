using System;
using System.Collections.Generic;
using Utilities.MarketData;

/// <summary>
/// Set of extension methods for IList 
/// </summary>

namespace Utilities.ExtenstionMethods
{
    public static class ListExtensions
    {

        #region generic list extensions

        public static Tuple<int, int> GetPrevAndNextIndex(this List<double> sortedList,
                                                          double value)
        ///<summary>
        /// binary search a sorted list and return the index of the items
        /// preceding and following the search value.  If value is greater 
        /// than all elements of sortedList, return the last index (and 
        /// similarly the first if it is smaller than all elements
        ///</summary>
        {
            var position = sortedList.BinarySearch(value);
            if (position >= 0)
            {
                // exact match - no interpolation needed
                return new Tuple<int, int>(position, position);
            }
            else
            {
                // not found exactly - interpolate
                var nextItemIndex = ~position;
                if (nextItemIndex == 0)
                {
                    // smaller than first tenor 
                    return new Tuple<int, int>(0, 0);
                }
                else if (nextItemIndex == sortedList.Count)
                {
                    // greater than last tenor
                    return new Tuple<int, int>(sortedList.Count - 1, sortedList.Count - 1);
                }
                else
                {
                    return new Tuple<int, int>(nextItemIndex - 1, nextItemIndex);
                }
            }
        }

        public static bool IsAlmostEqual(this List<double> listOne,
                                         List<double> listTwo,
                                         double tol)
        {
            if (listOne.Count != listTwo.Count) return false;
            else
            {
                for (int i = 0; i < listOne.Count; i++)
                {
                    if (Math.Abs(listOne[i] - listTwo[i]) > tol) return false;
                }

                return true;
            }
        }

        #endregion

        #region sorted list extensions

        public static bool IsAlmostEqual(this SortedList<DateTime, double> listOne,
                                         SortedList<DateTime, double> listTwo,
                                         double tol)
        {
            if (listOne.Count != listTwo.Count) return false;
            else
            {
                for (int i = 0; i < listOne.Count; i++)
                {

                    var dateOne = listOne.Keys[i];
                    var dateTwo = listTwo.Keys[i];
                    if (dateOne != dateTwo) return false;

                    var valOne = listOne[dateOne];
                    var valTwo = listOne[dateTwo];
                    if (Math.Abs(valOne - valTwo) > tol) return false;
                }
            }

            return true;
        }

        public static SortedList<DateTime, double> AsBDTimeSeries(this List<double> currentList,
                                                                  DateTime startDate)
        ///<summary>
        /// wrap a series as a business day time series
        ///</summary>
        {
            var businessDaySeries = new SortedList<DateTime, double>();
            var counter = 0;
            var currentDate = startDate;

            while (counter < currentList.Count)
            {
                businessDaySeries.Add(currentDate, currentList[counter]);
                currentDate = currentDate.AddWorkingDay();
                counter += 1;
            }

            return businessDaySeries;

        }

        public static SortedList<DateTime, double> GetPriceSeries(this SortedList<DateTime, OptionPricingData> currentList)
        {
            var prices = new SortedList<DateTime, double>();
            foreach(KeyValuePair<DateTime, OptionPricingData> kvp in currentList)
            {
                prices.Add(kvp.Key, kvp.Value.CurrentPrice);
            }

            return prices;
        }
    }

    #endregion 

}
