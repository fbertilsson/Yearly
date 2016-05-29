using Periodic.Ts;

namespace Periodic.Algo
{
    public interface IUnaryTsOperator
    {
        Timeseries Apply(Timeseries ts);
    }
}