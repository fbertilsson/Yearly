using System.Collections.Generic;

namespace Periodic
{
    public class Timeseries : List<Tvq>
    {
        //public IList<Tvq<T>> m_Tvq = new List<Tvq<T>>();

        public new Timeseries Add(Tvq tvq)
        {
            base.Add(tvq);
            return this;
        }
    }
}
