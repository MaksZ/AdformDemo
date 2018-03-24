using AdformDemo.Data.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdformDemo.Data
{
    public class WeekImpressions
    {
        public int Week { get; set; }
        public int Total { get; set; }
    }

    public static class Aggregators
    {
        /// <summary>
        /// Aggregates retrieved data by week and provide result how much impressions was each week this year.
        /// </summary>
        public static WeekImpressions[] GroupByWeek(this IEnumerable<ImpressionsRaw> source)
            =>
                source
                    .GroupBy(obj => obj.Date.GetWeekOfYear())
                    .Select(g => new WeekImpressions { Week = g.Key, Total = g.Sum(x => x.Value) })
                    .OrderBy(x => x.Week)
                    .ToArray();

        /// <summary>
        /// Finds data anomalies when bidrequests increased or decreased given in <paramref name="anomalyFactor"/> or more times compared to previous day
        /// </summary>
        /// <param name="source"></param>
        /// <param name="anomalyFactor"></param>
        /// <returns></returns>
        public static DateTime[] GetAnomalies(this IEnumerable<BidRequestsRaw> source, int anomalyFactor)
        {
            Func<BidRequestsRaw, BidRequestsRaw, bool> hasAnomaly =
                (previousBid, currentBid) =>
                {
                    var previous = previousBid.Value;
                    var current = currentBid.Value;

                    if (previous == current) return false;

                    if (previous > current)
                        return current == 0 || previous / current >= anomalyFactor;

                    return previous == 0 || current / previous >= anomalyFactor;
                };

            return source
                .FilterByPrevious(hasAnomaly)
                .Select(x => x.Date)
                .ToArray();
        }
                
        private static IEnumerable<T> FilterByPrevious<T>(this IEnumerable<T> source, Func<T, T, bool> predicate)
        {
            using (var e = source.GetEnumerator())
            {
                if (!e.MoveNext())
                    yield break;

                var previous = e.Current;

                while (e.MoveNext())
                {
                    var current = e.Current;

                    if (predicate(previous, current))
                        yield return current;

                    previous = current;
                }
            }
        }
    }

    public static class CalendarHelper
    {
        public static Calendar CurrentCalendar { get; set; } = CultureInfo.InvariantCulture.Calendar;

        public static CalendarWeekRule CurrentCalendarWeekRule { get; set; } = CalendarWeekRule.FirstDay;

        public static DayOfWeek CurrentDayOfWeek { get; set; } = DayOfWeek.Monday;

        public static int GetWeekOfYear(this DateTime date)
            =>
                CurrentCalendar.GetWeekOfYear(date, CurrentCalendarWeekRule, CurrentDayOfWeek);
    }
}
