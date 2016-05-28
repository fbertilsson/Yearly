using System;
using Periodic.Ts;

namespace Periodic.Algo
{
    public interface IUnaryOperator
    {
        Tvq Apply(DateTime t0, DateTime t1, Timeseries ts);
    }
}