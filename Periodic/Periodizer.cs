using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <param name="intervalLength"></param>
        /// <returns></returns>
        public Timeseries<T> InsertPoints<T>(Timeseries<T> ts, Interval intervalLength)
        {
            if (ReferenceEquals(ts, null)) throw new ArgumentException("ts");

            var result = new Timeseries<T>();

            if (ts.Any())
            {
                result.Add(ts.First());
                var lastTvq = ts.Last();
                var t0 = lastTvq.Time;
                var lastSecond = new DateTime(t0.Year, 12, 31, 23, 59, 59);
                var newTvq = new Tvq<T>(lastSecond, lastTvq.V, Quality.Interpolated);
                result.Add(newTvq);
            }

            return result;
        }

    }
}
