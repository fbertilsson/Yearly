using System;
using Periodic.Ts;

namespace Periodic.Algo
{
    public interface IUnaryAggregateOperator
    {
        Tvq Apply(DateTime t0, DateTime t1, Timeseries ts);
    }
}