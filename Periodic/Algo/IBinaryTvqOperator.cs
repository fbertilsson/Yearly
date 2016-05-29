using Periodic.Ts;

namespace Periodic.Algo
{
    public interface IBinaryTvqOperator
    {
        Tvq Apply(Tvq o1, Tvq o2);
    }
}