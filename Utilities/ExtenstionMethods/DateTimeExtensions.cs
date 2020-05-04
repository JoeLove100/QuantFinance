using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.ExtenstionMethods
{
    public static class DateTimeExtensions
    {
        public static double GetYearFractionTo(this DateTime currentDate, 
                                              DateTime futureDate,
                                              int daysInYear = 365)
        {
            if (currentDate >= futureDate)
            {
                return 0;
            }
            else
            {
                var frac = (double) (futureDate - currentDate).Days / (double) daysInYear;
                return frac;
            }

        }
    }
}
