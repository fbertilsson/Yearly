using Periodic.Ts;

namespace Periodic.Algo
{
    public class DeltaTvqOperator : IBinaryTvqOperator
    {

        public double RolloverThresholdUpper { get; set; } = 9e5;
        public double RolloverThresholdLower { get; set; } = 1e3;
        public double Rollover { get; set; } = 1e6;

        public Tvq Apply(Tvq o1, Tvq o2)
        {
            var v = o2.V - o1.V;

            var isRollover = v < 0 
                    && o1.V > RolloverThresholdUpper
                    && o2.V < RolloverThresholdLower;
            if (isRollover)
            {
                v = Rollover - o1.V + o2.V;
            } else if (v < 0
                       && o1.V > RolloverThresholdLower
                       && o2.V < RolloverThresholdLower)
            {
                v = 0;
            }

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