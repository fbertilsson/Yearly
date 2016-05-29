using Periodic.Ts;

namespace Periodic.Algo
{
    public class DeltaTvqOperator : IBinaryTvqOperator
    {
        public Tvq Apply(Tvq o1, Tvq o2)
        {
            var v = o2.V - o1.V;
            var q = Quality.Ok;

            if (o1.Q == Quality.Suspect || o2.Q == Quality.Suspect)
            {
                q = Quality.Suspect;
            }
            else if (o1.Q == Quality.Interpolated || o2.Q == Quality.Interpolated)
            {
                q = Quality.Interpolated;
            }

            return new Tvq(o2.Time, v, q);
        }
    }
}