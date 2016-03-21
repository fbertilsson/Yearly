using Microsoft.WindowsAzure.Storage.Table;
using Periodic;

namespace YearlyWeb2.DataLayer
{
    public class TvqEntity : TableEntity
    {
        public TvqEntity(string partitionKey, Tvq tvq)
        {
            PartitionKey = partitionKey;
            RowKey = tvq.Time.Ticks.ToString();

            V = tvq.V;
            Q = tvq.Q;
        }

        public TvqEntity()
        {
        }

        public double V { get; set; }

        public Quality Q { get; set; }
    }
}