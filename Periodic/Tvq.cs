using System;

namespace Periodic
{
    public class Tvq
    {
        public DateTime Time { get; private set; }

        public double V { get; private set; }

        public Quality Q { get; private set; }

        public Tvq(DateTime t, double v, Quality q)
        {
            Time = t;
            V = v;
            Q = q;
        }


        /// <summary>
        /// Linearly interpolates or extrapolates the value at t
        /// </summary>
        /// <param name="t"></param>
        /// <param name="tvq1"></param>
        /// <param name="tvq2"></param>
        /// <returns></returns>
        public static Tvq CalculateValueAt(DateTime t, Tvq tvq1, Tvq tvq2)
        {
            var dx = tvq2.Time - tvq1.Time;
            var dy = tvq2.V - tvq1.V;
            var k = dy / dx.TotalSeconds;

            var x = (t - tvq1.Time).TotalSeconds;
            var y = k * x + tvq1.V;

            var tvq = new Tvq(t, y, Quality.Interpolated);
            return tvq;
        }

        public override string ToString()
        {
            return $"{Time}\t{V}\t{Q}";
        }
    }
}
