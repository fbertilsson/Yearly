﻿using System;
using System.Linq;

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
        public Timeseries InsertPoints(Timeseries ts, Interval intervalLength)
        {
            if (ReferenceEquals(ts, null)) throw new ArgumentException("ts");

            var result = new Timeseries();
            Tvq previous = null;
            for (var i = 0; i < ts.Count; i++)
            {
                var current = ts[i];
                var isNewInterval = i == 0 || 
                    (previous != null && previous.Time.Year != current.Time.Year);

                //
                // Previous interval end?
                //
                var doInsertPreviousEnd = isNewInterval && previous != null ; // && not last second
                if (doInsertPreviousEnd)
                {
                    var lastSecondOfPrevious = new DateTime(previous.Time.Year, 12, 31, 23, 59, 59);
                    var tvq = CalculateValueAt(lastSecondOfPrevious, previous, current);
                    result.Add(tvq);
                }

                //
                // Current interval start?
                //
                var t = current.Time;
                var firstSecond = new DateTime(t.Year, 01, 01, 0, 0, 0);
                if (isNewInterval && t > firstSecond)
                {
                    var newTvq = previous == null ?
                        new Tvq(firstSecond, current.V, Quality.Interpolated) : 
                        CalculateValueAt(firstSecond, previous, current);
                    result.Add(newTvq);
                }

                result.Add(current); 
                
                previous = current;
            }

            if (ts.Any())
            {
                if (previous != null)
                {
                    var lastSecond = new DateTime(previous.Time.Year, 12, 31, 23, 59, 59);

                    var iLast = ts.Count - 1;
                    var lastTvq = ts.Last();

                    if (ts.Count <= 1 && lastTvq.Time < lastSecond)
                    {
                        var newTvq = new Tvq(lastSecond, lastTvq.V, Quality.Interpolated);
                        result.Add(newTvq);
                    }
                    else if (lastTvq.Time < lastSecond)
                    {
                        var newTvq = CalculateValueAt(lastSecond, ts[iLast - 1], lastTvq);
                        result.Add(newTvq);
                    }
                }
            }
            return result;
        }

        private static Tvq CalculateValueAt(DateTime t, Tvq tvq1, Tvq tvq2)
        {
            var dx = tvq2.Time - tvq1.Time;
            var dy = tvq2.V - tvq1.V;
            var k = dy/dx.TotalSeconds;

            var x = (t - tvq1.Time).TotalSeconds;
            var y = k*x + tvq1.V;

            var tvq = new Tvq(t, y, Quality.Interpolated);
            return tvq;
        }
    }
}
