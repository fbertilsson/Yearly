using System;
using System.Diagnostics;
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
                t = t0.AddMonths(1);
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
            int currentIndex = 0;
            for (i = 0; i < ts.Count; i++)
            {
                currentIndex = i;
                current = ts[currentIndex];
                if (current.Time > t0)
                {
                    break;
                }
                v0 = current.V;
            }

            Debug.Assert(ts[currentIndex].Time >= t0);

            double area = 0;

            bool tsHasValueWithinPeriod = current.Time <= t1;
            if (!tsHasValueWithinPeriod)
            {
                double v1 = 0;
                if (i > 0)
                {
                    v1 = Tvq.CalculateValueAt(t1, ts[currentIndex - 1], ts[currentIndex]).V;
                }
                double avg = (v0 + v1)/ 2;
                return new Tvq(t0, avg, Quality.Interpolated);
            }

            if (current != null)
            {
                var dt = (current.Time - t0).TotalSeconds;
                area = v0 * dt;
            }

            var previous = current;
            var valueAtT1 = current.V;      // a hypothesis

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

            var deltaT = (t1 - previous.Time).TotalSeconds;
            area += deltaT * (previous.V + valueAtT1) / 2;

            var average = area/(t1 - t0).TotalSeconds;
            return new Tvq(t0, average, Quality.Interpolated);
        }
    }
}
