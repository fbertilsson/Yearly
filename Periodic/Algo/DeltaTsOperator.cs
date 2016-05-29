using System;
using Periodic.Ts;

namespace Periodic.Algo
{
    /// <summary>
    /// Calculates a delta series given a series, where each result TVQ is the
    /// difference from the previous TVQ value. For example, calculates
    /// consumption from register entries.
    /// </summary>
    public class DeltaTsOperator : IUnaryTsOperator
    {
        private static readonly DeltaTvqOperator DeltaOp = new DeltaTvqOperator();

        /// <summary>
        /// Applies the delta function to each pair of consecutive values.
        /// </summary>
        /// <param name="ts">A time series</param>
        /// <returns></returns>
        public Timeseries Apply(Timeseries ts)
        {
            const string paramNameTs = "ts";
            if (ts == null) throw new ArgumentException("Must be non-null", paramNameTs);

            if (ts.Count < 2) throw new ArgumentException("ts must contain at least two elements", paramNameTs);

            var result = new Timeseries();

            var enumerator = ts.GetEnumerator();
            enumerator.MoveNext();
            var previous = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                var delta = CalculateDelta(current, previous);
                result.Add(delta);
                previous = current;
            }

            return result;
        }

        private static Tvq CalculateDelta(Tvq current, Tvq previous)
        {
            return DeltaOp.Apply(previous, current);
        }
    }
}
