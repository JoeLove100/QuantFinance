using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Set of extension methods for IList 
/// </summary>

namespace Utilities.ExtenstionMethods
{
    public static class ListExtensions
    {
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
    }
}
