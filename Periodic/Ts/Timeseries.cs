using System.Collections.Generic;
using System.Linq;

namespace Periodic.Ts
{
    public class Timeseries : List<Tvq>
    {
        public new Timeseries Add(Tvq tvq)
        {
            base.Add(tvq);
            return this;
        }

        public override string ToString()
        {
            return this.Aggregate(
                string.Empty,
                (s, x) => s + $"\r\n{x.Time}\t{x.V}\t{x.Q}");
        }
    }
}
