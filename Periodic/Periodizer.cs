using System;
using System.Linq;
using Periodic.Algo;
using Periodic.Ts;

namespace Periodic
{
    public class Periodizer
    {
        /// <summary>
        /// Inserts point at the specified interval by attempting to interpolate and/or 
        /// extrapolate values. Useful when splitting up a period into several series
        /// and displaying calculated start/end values in a graph.
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="intervalLength"></param>
        /// <returns>A new timeseries that has values at the start and end of the interval</returns>
        /// <example>
        /// TODO FB  
        /// </example>
        public Timeseries InsertPoints(Timeseries ts, Interval intervalLength)
        {
            if (ReferenceEquals(ts, null)) throw new ArgumentException("ts");

            var result = new Timeseries();
            Tvq previous = null;
            // Maybe try a while loop instead that moves the time forwards by the smaller of
            // * the next TVQ
            // * the next interval
            for (var i = 0; i < ts.Count; i++)
            {
                var current = ts[i];
                var isNewInterval = 
                    i == 0 || 
                    IsNewInterval(previous, current, intervalLength);

                //
                // Previous interval end?
                //
                var doInsertPreviousEnd = isNewInterval && previous != null ; // && not last second
                if (doInsertPreviousEnd)
                {
                    var lastSecondOfPrevious = LastSecondOfInterval(previous, intervalLength);
                    var tvq = Tvq.CalculateValueAt(lastSecondOfPrevious, previous, current);
                    result.Add(tvq);
                }

                //
                // Current interval start?
                //
                var t = current.Time;
                var firstSecond = FirstSecond(t, intervalLength);
                if (isNewInterval && t > firstSecond)
                {
                    var newTvq = previous == null ?
                        new Tvq(firstSecond, current.V, Quality.Interpolated) : Tvq.CalculateValueAt(firstSecond, previous, current);
                    result.Add(newTvq);
                }

                result.Add(current); 
                
                previous = current;
            }

            if (ts.Any())
            {
                if (previous != null)
                {
                    var lastSecond = LastSecondOfInterval(previous, intervalLength);

                    var iLast = ts.Count - 1;
                    var lastTvq = ts.Last();

                    if (ts.Count <= 1 && lastTvq.Time < lastSecond)
                    {
                        var newTvq = new Tvq(lastSecond, lastTvq.V, Quality.Interpolated);
                        result.Add(newTvq);
                    }
                    else if (lastTvq.Time < lastSecond)
                    {
                        var newTvq = Tvq.CalculateValueAt(lastSecond, ts[iLast - 1], lastTvq);
                        result.Add(newTvq);
                    }
                }
            }
            return result;
        }

        public static DateTime FirstSecond(DateTime t, Interval intervalLength)
        {
            switch (intervalLength)
            {
                case Interval.Year:
                    return new DateTime(t.Year, 01, 01, 0, 0, 0);
                case Interval.Month:
                    return new DateTime(t.Year, t.Month, 01, 0, 0, 0);
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));
            }
            
        }

        public static DateTime LastSecondOfInterval(Tvq tvq, Interval intervalLength)
        {
            switch (intervalLength)
            {
                case Interval.Year:
                    return new DateTime(tvq.Time.Year, 12, 31, 23, 59, 59);
                case Interval.Month:
                    return new DateTime(tvq.Time.Year, tvq.Time.Month, 
                        DateTime.DaysInMonth(tvq.Time.Year, tvq.Time.Month), 
                        23, 59, 59);
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));
            }
        }

        public static bool IsNewInterval(Tvq previous, Tvq current, Interval intervalLength)
        {
            switch (intervalLength)
            {
                case Interval.Year:
                    return previous != null && previous.Time.Year != current.Time.Year;
                case Interval.Month:
                    return previous != null && 
                           (
                               previous.Time.Year != current.Time.Year  ||
                               previous.Time.Month != current.Time.Month
                            );
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));
            }
        }

        /// <summary>
        /// Return a time series with one entry per month
        /// where the time is the first second of the month
        /// and the value is the average.
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public Timeseries MonthlyAverage(Timeseries ts)
        {
            var interpolatedTs = InsertPoints(ts, Interval.Month);
            var deltaOperator = new DeltaTsOperator();
            var consumptionTs = deltaOperator.Apply(interpolatedTs);

            var result = new Aggregate().Apply(new Average(), consumptionTs);
            return result;
        }
    }
}
