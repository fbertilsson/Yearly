using Periodic.Ts;

namespace Periodic.Algo
{
    public interface IAggregateAlgorithm
    {
        Timeseries Apply(IUnaryAggregateOperator op, Timeseries ts);
    }
}