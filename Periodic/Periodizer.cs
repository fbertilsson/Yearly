using System;
using System.Diagnostics;
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
            for (var i = 0; i < ts.Count; i++)
            {
                var current = ts[i];
                var isNewInterval = 
                    i == 0 || 
                    previous != null && IsNewInterval(previous.Time, current.Time, intervalLength);

                //
                // Previous interval end?
                //
                var doInsertPreviousEnd = isNewInterval && previous != null ; // && not last second
                if (doInsertPreviousEnd)
                {
                    var lastSecondOfPrevious = LastSecondOfInterval(previous.Time, intervalLength);
                    var tvq = Tvq.CalculateValueAt(lastSecondOfPrevious, previous, current);
                    result.Add(tvq);
                }

                if (previous != null)
                {
                    var hasEmptyIntervalInBetween = 
                        HasEmptyIntervalInBetween(previous.Time, current.Time, intervalLength);
                    var t = previous.Time;
                    while (hasEmptyIntervalInBetween)
                    {
                        t = FirstSecondOfNextInterval(t, intervalLength);
                        var tvq = Tvq.CalculateValueAt(t, previous, current);
                        result.Add(tvq);

                        var tLastSecond = LastSecondOfInterval(t, intervalLength);
                        tvq = Tvq.CalculateValueAt(tLastSecond, previous, current);
                        result.Add(tvq);
                        
                        hasEmptyIntervalInBetween = HasEmptyIntervalInBetween(t, current.Time, intervalLength);
                    }
                }

                //
                // Current interval start?
                //
                var firstSecond = FirstSecond(current.Time, intervalLength);
                if (isNewInterval && current.Time > firstSecond)
                {
                    var newTvq = previous == null ?
                        new Tvq(firstSecond, current.V, Quality.Interpolated) : Tvq.CalculateValueAt(firstSecond, previous, current);
                    result.Add(newTvq);
                }

                //
                // Add point
                //
                result.Add(current);

                previous = current;
            }

            if (ts.Any())
            {
                if (previous != null)
                {
                    var lastSecond = LastSecondOfInterval(previous.Time, intervalLength);

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

        private DateTime FirstSecondOfNextInterval(DateTime t, Interval intervalLength)
        {
            switch (intervalLength)
            {
                case Interval.Year:
                    return new DateTime(t.Year + 1, 1, 1, 0, 0, 0, 0);
                case Interval.Month:
                    return new DateTime(t.Year, t.Month, 1, 0, 0, 0, 0).AddMonths(1);
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));                    
            }
            
        }

        /// <summary>
        /// Returns true if the points in time are separated by one or more intervals.
        /// </summary>
        /// <param name="t0"></param>
        /// <param name="t1"></param>
        /// <param name="intervalLength"></param>
        /// <returns></returns>
        /// <example>
        /// The following return true:
        /// 2016-01-01, 2016-03-01
        /// 2017-12-31, 2018-02-01
        /// </example>
        public bool HasEmptyIntervalInBetween(DateTime t0, DateTime t1, Interval intervalLength)
        {
            if (t0 > t1)
            {
                throw new ArgumentException("t0 > t1", nameof(t0));
            }

            switch (intervalLength)
            {
                case Interval.Year:
                    return t1.Year - t0.Year > 1;
                case Interval.Month:
                    var firstInMonth = new DateTime(t0.Year, t0.Month, 1, 0, 0, 0, 0);
                    return firstInMonth.AddMonths(2) <= t1;
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));                    
            }
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

        public static DateTime LastSecondOfInterval(DateTime t, Interval intervalLength)
        {
            switch (intervalLength)
            {
                case Interval.Year:
                    return new DateTime(t.Year, 12, 31, 23, 59, 59);
                case Interval.Month:
                    return new DateTime(t.Year, t.Month, 
                        DateTime.DaysInMonth(t.Year, t.Month), 
                        23, 59, 59);
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));
            }
        }

        public static bool IsNewInterval(DateTime previous, DateTime current, Interval intervalLength)
        {
            switch (intervalLength)
            {
                case Interval.Year:
                    return previous.Year != current.Year;
                case Interval.Month:
                    return
                        previous.Year != current.Year ||
                        previous.Month != current.Month;                        
                default:
                    throw new ArgumentException("Interval not supported", nameof(intervalLength));
            }
        }

        /// <summary>
        /// Return a time series with one entry per month
        /// where the time is the first second of the month
        /// and the value is the average.
        /// 
        /// Handles roll over and meter change.
        /// </summary>
        /// <param name="ts">A time series with meter values, possibly with roll over and meter change.</param>
        /// <returns></returns>
        public Timeseries MonthlyAverage(Timeseries ts)
        {
            if (ts == null || !ts.Any()) return new Timeseries();

            var deltaOperator = new DeltaTsOperator();
            var consumptionTs = deltaOperator.Apply(ts);

            var result = Periodize(new Average(), consumptionTs, Interval.Month);
            return result;
        }

        /// <summary>
        /// Periodizes a time series with relative consumption
        /// </summary>
        /// <param name="op"></param>
        /// <param name="ts">A time series with relative consumption, without roll over or meter change</param>
        /// <param name="interval"></param>
        /// <returns></returns>
        private Timeseries Periodize(IUnaryAggregateOperator op, Timeseries ts, Interval interval)
        {
            var result = new Timeseries();

            using (var enumerator = ts.GetEnumerator())
            {
                DateTime? intervalStart = null;
                Tvq nextTvq;
                while (enumerator.MoveNext())
                {
                    var currentTvq = enumerator.Current;
                    Debug.Assert(currentTvq != null);

                    if (intervalStart == null)
                    {
                        intervalStart = FirstSecond(currentTvq.Time, interval);
                    }

                    var nextIntervalStart = FirstSecondOfNextInterval(currentTvq.Time, interval);

                    Debug.Assert(currentTvq.Time >= intervalStart);
                    Debug.Assert(currentTvq.Time <= nextIntervalStart);

                    enumerator.MoveNext();
                    nextTvq = enumerator.Current;

                    //
                    // Create a new timeseries to pass to the operator
                    //

                    var tsForPeriod = new Timeseries();
                    if (currentTvq.Time > intervalStart)
                    {
                        var tentativeValueAtIntervalStart = Tvq.CalculateValueAt(intervalStart.Value, currentTvq, nextTvq);

                        //
                        // A negative consumption is probably a start of the time series. Set it to 0.
                        // E.g. 0 at the 10:th and 90 at month end.
                        //
                        var valueAtIntervalStart = new Tvq(
                            tentativeValueAtIntervalStart.Time, 
                            Math.Max(0d, tentativeValueAtIntervalStart.V),
                            tentativeValueAtIntervalStart.Q);
                        tsForPeriod.Add(valueAtIntervalStart);
                    }

                    tsForPeriod.Add(currentTvq);
                    Tvq previousTvq = null;
                    var isDone = false;
                    do
                    {
                        if (nextTvq == null)
                        {
                            isDone = true;
                        }
                        else
                        {
                            if (nextTvq.Time > nextIntervalStart)
                            {
                                tsForPeriod.Add(Tvq.CalculateValueAt(nextIntervalStart, currentTvq, nextTvq));
                            }

                            previousTvq = currentTvq;
                            currentTvq = nextTvq;
                            enumerator.MoveNext();
                            nextTvq = enumerator.Current;

                            if (IsNewInterval(intervalStart.Value, currentTvq.Time, interval))
                            {
                                isDone = true;
                            }
                            else
                            {
                                tsForPeriod.Add(currentTvq);
                            }
                        }
                    } while (! isDone);

                    if (tsForPeriod.Last().Time < nextIntervalStart)
                    {
                        Tvq valueAtNext;
                        if (previousTvq == null)
                        {
                            valueAtNext = new Tvq(nextIntervalStart, 0, Quality.Interpolated);
                        }
                        else
                        {
                            valueAtNext = Tvq.CalculateValueAt(nextIntervalStart, previousTvq, currentTvq);
                        }

                        tsForPeriod.Add(valueAtNext);
                    }

                    if (tsForPeriod.Any())
                    {
                        var v0 = tsForPeriod.First().V;
                        var v1 = tsForPeriod.Last().V;
                        var consumptionInPeriod = v1 - v0;
                        result.Add(new Tvq(intervalStart.Value, consumptionInPeriod, Quality.Ok));
                        // result.Add(op.Apply(intervalStart.Value, nextIntervalStart, tsForPeriod));
                    }
                    else
                    {
                        result.Add(new Tvq(intervalStart.Value, 0, Quality.Suspect));
                    }
                }
            }
            return result;
        }
    }
}
