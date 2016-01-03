using System;
using System.Collections.Generic;
using System.Linq;

namespace Periodic
{
    public class Splitter<T>
    {
        /// <summary>
        /// Splits a timeseries into possibly several with one timeseries per year.
        /// </summary>
        /// <param name="timeseries"></param>
        /// <returns></returns>
        public IEnumerable<Timeseries<T>> SplitPerYear(Timeseries<T> timeseries) {

            if (! timeseries.Any())
            {
                return new List<Timeseries<T>>();
            }

            var result = new List<Timeseries<T>>();
            Tvq<T> previous = null;
            Timeseries<T> currentTs = null;

            foreach (var tvq in timeseries)
            {
                if (previous == null)
                {
                    currentTs = new Timeseries<T>();
                    currentTs.Add(tvq);
                    result.Add(currentTs);
                }
                else
                {
                    if (previous.Time.Year != tvq.Time.Year)
                    {
                        currentTs = new Timeseries<T>();
                        result.Add(currentTs);
                    }
                    currentTs.Add(tvq);
                }
                previous = tvq;
            }

            return result;
        }

    }
}
