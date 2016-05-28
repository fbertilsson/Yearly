using Periodic.Ts;

namespace Periodic.Algo
{
    public interface IAlgorithm
    {
        Timeseries Apply(IUnaryOperator op, Timeseries ts);
    }
}