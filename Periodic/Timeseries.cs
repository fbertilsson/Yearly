using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periodic
{
    public class Timeseries<T> : List<Tvq<T>>
    {
        //public IList<Tvq<T>> m_Tvq = new List<Tvq<T>>();

        public new Timeseries<T> Add(Tvq<T> tvq)
        {
            base.Add(tvq);
            return this;
        }
    }
}
