using System;
using System.Diagnostics;
using Periodic.Ts;

namespace Periodic.Algo
{
    public class Average : IUnaryAggregateOperator
    {
        public Tvq Apply(DateTime t0, DateTime t1, Timeseries ts)
        {
            double v0 = 0; 
            Tvq current = null;
            int i;
            int currentIndex = 0;
            for (i = 0; i < ts.Count; i++)
            {
                currentIndex = i;
                current = ts[currentIndex];
                v0 = current.V;
                if (current.Time >= t0)
                {
                    break;
                }
            }

            Debug.Assert(ts[currentIndex].Time >= t0);

            if (currentIndex > 0)
            {
                v0 = Tvq.CalculateValueAt(t0, ts[currentIndex - 1], ts[currentIndex]).V;
            }

            double area = 0;

            var tsHasValueWithinPeriod = current.Time <= t1;
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

            Tvq previous = null;
            for (i++; i < ts.Count; i++)
            {
                previous = current;
                current = ts[i];
                if (current.Time > t1)
                {
                    break;
                }

                var dt = (current.Time - previous.Time).TotalSeconds;
                var averageValue = (previous.V + current.V) / 2;
                area += averageValue * dt;
            }

            if (current.Time < t1)
            {
                var isStepwise = true;
                var valueAtT1 = isStepwise
                    ? current.V                                      // Take current value
                    : Tvq.CalculateValueAt(t1, previous, current).V; // Extrapolate linearly

                var deltaT =
                    (t1 - current.Time)
                    .TotalSeconds;
                var averageValue = (current.V + valueAtT1) / 2;
                var lastArea = averageValue * deltaT;
                area += lastArea;
            }

            var average = area/(t1 - t0).TotalSeconds;
            return new Tvq(t0, average, Quality.Interpolated);
        }
    }
}
