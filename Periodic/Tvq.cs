using System;

namespace Periodic
{
    public class Tvq<T>
    {
        public DateTime Time { get; private set; }

        public T V { get; private set; }

        public Quality Q { get; private set; }

        public Tvq(DateTime t, T v, Quality q)
        {
            Time = t;
            V = v;
            Q = q;
        }
    }
}
