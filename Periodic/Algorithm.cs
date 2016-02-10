using System;
using System.Linq;

namespace Periodic
{
    public interface IAlgorithm
    {
        Timeseries Apply(IUnaryOperator op, Timeseries ts);
    }

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
                t = t.AddMonths(1);
            } while (t <= ts.Last().Time);

            return result;
        }
    }

    public interface IUnaryOperator
    {
        Tvq Apply(DateTime t0, DateTime t1, Timeseries ts);
    }

    public class Average : IUnaryOperator
    {
        public Tvq Apply(DateTime t0, DateTime t1, Timeseries ts)
        {
            double v0 = 0; // Value at start of period. Zero if first tvq.Time > t0
            Tvq current = null;
            int i;
            for (i = 0; i < ts.Count; i++)
            {
                current = ts[i];
                if (current.Time > t0)
                {
                    break;
                }
                v0 = current.V;
            }

            // Invariant ts[i].Time >= t0

            double area = 0;
            if (current != null)
            {
                var dt = (current.Time - t0).TotalSeconds;
                area = v0 * dt;
            }

            var previous = current;
            var valueAtT1 = current.V;

            for (i++; i < ts.Count; i++)
            {
                current = ts[i];
                if (current.Time == t1)
                {
                    valueAtT1 = current.V;
                }
                else if (current.Time > t1)
                {
                    valueAtT1 = Tvq.CalculateValueAt(t1, previous, current).V;
                    break;
                }

                var dt = (current.Time - previous.Time).TotalSeconds;
                area += (previous.V + current.V) / 2 * dt;
                previous = current;
            }

            if (current.Time < t1)
            {
                valueAtT1 = current.V; // Extrapolate current value to end of period
            }

            area += ((previous.V + valueAtT1) / 2) * (t1 - previous.Time).TotalSeconds;

            var average = area/(t1 - t0).TotalSeconds;
            return new Tvq(t0, average, Quality.Interpolated);
        }
    }
}
