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

        public override string ToString()
        {
            return $"{Time}\t{V}\t{Q}";
        }
    }
}
