using System;
using System.Linq;

namespace Periodic
{
    public enum Interval
    {
        Year,
    }

    public class Periodizer
    {
        /// <summary>
        /// Inserts point at the specified interval by attempting to interpolate and/or 
        /// extrapolate values. Useful when splitting up a period into several series
        /// and displaying calculated start/end values in a graph.
        /// </summary>
        /// <param name="ts"></param>
        /// <param name="intervalLength"></param>
        /// <returns></returns>
        public Timeseries InsertPoints(Timeseries ts, Interval intervalLength)
        {
            if (ReferenceEquals(ts, null)) throw new ArgumentException("ts");

            var result = new Timeseries();

            if (ts.Any())
            {
                var firstTvq = ts.First();
                var tFirst = firstTvq.Time;
                var firstSecond = new DateTime(tFirst.Year, 01, 01, 0, 0, 0);
                var lastSecond = new DateTime(tFirst.Year, 12, 31, 23, 59, 59);
                Tvq newTvq;
                if (ts.Count == 1 && tFirst > firstSecond)
                {
                    newTvq = new Tvq(firstSecond, firstTvq.V, Quality.Interpolated);
                    result.Add(newTvq);
                }
                result.AddRange(ts); // Adding one or more points
                
                var iLast = ts.Count - 1;
                var lastTvq = ts[iLast];

                if (ts.Count < 2 && lastTvq.Time < lastSecond)
                {
                    newTvq = new Tvq(lastSecond, lastTvq.V, Quality.Interpolated);
                    result.Add(newTvq);
                }
                else if (ts.Count >= 2)
                { 
                    var nextLast = ts[iLast - 1];

                    var dx = lastTvq.Time - nextLast.Time;
                    var dy = lastTvq.V - nextLast.V;
                    var k = dy/dx.TotalSeconds;

                    var x = (lastSecond - lastTvq.Time).TotalSeconds;
                    var extrapolatedValue = k * x + lastTvq.V;

                    newTvq = new Tvq(lastSecond, extrapolatedValue, Quality.Interpolated);
                    result.Add(newTvq);
                }
            }

            return result;
        }

    }
}
