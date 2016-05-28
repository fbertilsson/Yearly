using System;
using System.Linq;
using Periodic.Ts;

namespace Periodic.Algo
{
    public class Aggregate : IAlgorithm
    {
        public Timeseries Apply(IUnaryOperator op, Timeseries ts)
        {
            if (ts == null || ! ts.Any()) return new Timeseries();

            var result = new Timeseries();

            // Default to monthly
            var t = ts.First().Time;
            do
            {
                var t0 = new DateTime(t.Year, t.Month, 1, 0, 0, 0);
                var t1 = new DateTime(t.Year, t.Month, DateTime.DaysInMonth(t.Year, t.Month), 23, 59, 59);
                result.Add(op.Apply(t0, t1, ts));
                t = t0.AddMonths(1);
            } while (t <= ts.Last().Time);

            return result;
        }
    }
}