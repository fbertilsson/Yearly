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
                result.Add(ts.First());
                var lastTvq = ts.Last();
                var t0 = lastTvq.Time;
                var lastSecond = new DateTime(t0.Year, 12, 31, 23, 59, 59);
                Tvq newTvq;
                if (ts.Count == 1)
                {
                    newTvq = new Tvq(lastSecond, lastTvq.V, Quality.Interpolated);
                }
                else
                {
                    result.Add(ts[1]);
                    int j = 1;
                    var nextLast = ts[j - 1];
                    var last = ts[j];

                    var dx = last.Time - nextLast.Time;
                    var dy = last.V - nextLast.V;
                    var k = dy/dx.TotalSeconds;

                    var x = (lastSecond - last.Time).TotalSeconds;
                    var extrapolatedValue = k * x + last.V;

                    newTvq = new Tvq(lastSecond, extrapolatedValue, Quality.Interpolated);
                }
                result.Add(newTvq);
            }

            return result;
        }

    }
}
