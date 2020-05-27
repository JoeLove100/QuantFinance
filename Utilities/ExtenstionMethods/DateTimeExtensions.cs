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
                var frac = (double) (futureDate - currentDate).Days /  daysInYear;
                return frac;
            }

        }

        public static DateTime AddWorkingDay(this DateTime currentDate)
        {
            //TODO: should make this handle bank holidays etc.

            if (currentDate.DayOfWeek is DayOfWeek.Friday)
            {
                return currentDate.AddDays(3);
            }
            else if (currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                return currentDate.AddDays(2);
            }
            else
            {
                return currentDate.AddDays(1);
            }

        }

        public static int GetWorkingDaysTo(this DateTime startDate,
                                           DateTime futureDate)
        {
            var dayCount = 0;
            var currentDate = startDate;

            if (!startDate.IsWorkingDay())
            {
                currentDate = currentDate.AddWorkingDay();
            }

            while(currentDate < futureDate)
            {
                dayCount += 1;
                currentDate = currentDate.AddWorkingDay();
            }

            return dayCount;
        }

        public static bool IsWorkingDay(this DateTime currentDate)
        {
            if (currentDate.DayOfWeek is DayOfWeek.Saturday || currentDate.DayOfWeek is DayOfWeek.Sunday)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static DateTime PrevWorkingDay(this DateTime currentDate)
        {
            if (currentDate.DayOfWeek is DayOfWeek.Saturday)
            {
                return currentDate.AddDays(-1);
            }
            else if (currentDate.DayOfWeek is DayOfWeek.Sunday)
            {
                return currentDate.AddDays(-2);
            }
            else
            {
                return currentDate;
            }
        }
    }
}
