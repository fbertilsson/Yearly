﻿using System.Collections.Generic;
using System.Linq;
using Periodic.Ts;

namespace Periodic
{
    public class Splitter
    {
        /// <summary>
        /// Splits a timeseries into possibly several with one timeseries per year.
        /// </summary>
        /// <param name="timeseries"></param>
        /// <returns></returns>
        public IList<Timeseries> SplitPerYear(Timeseries timeseries) {

            if (! timeseries.Any())
            {
                return new List<Timeseries>();
            }

            var result = new List<Timeseries>();
            Tvq previous = null;
            Timeseries currentTs = null;

            foreach (var tvq in timeseries)
            {
                if (previous == null)
                {
                    currentTs = new Timeseries {tvq};
                    result.Add(currentTs);
                }
                else
                {
                    if (previous.Time.Year != tvq.Time.Year)
                    {
                        currentTs = new Timeseries();
                        result.Add(currentTs);
                    }
                    currentTs.Add(tvq);
                }
                previous = tvq;
            }

            return result;
        }

        public IList<Timeseries> SplitPerWeek(Timeseries timeseries)
        {
            var result = new List<Timeseries>();
            if (timeseries.Any())
            {
                Tvq previous = null;
                Timeseries currentTs = null;
                timeseries.Reverse();

                foreach (var tvq in timeseries)
                {
                    if (previous == null)
                    {
                        currentTs = new Timeseries {tvq};
                        result.Add(currentTs);
                    }
                    else
                    {
                        if ((previous.Time - tvq.Time).TotalDays >= 7)
                        {
                            currentTs = new Timeseries();
                            result.Add(currentTs);
                        }

                        currentTs.Add(tvq);
                    }

                    previous = tvq;
                }

                foreach (var ts in result)
                {
                    ts.Reverse();
                }
            }

            return result;
        }
    }
}
