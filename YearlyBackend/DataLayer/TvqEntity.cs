using System;
using Azure;
using Azure.Data.Tables;
using Periodic.Ts;

namespace YearlyBackend.DataLayer
{
    public class TvqEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

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

        public DateTime T { get { return new DateTime(long.Parse(RowKey));} }

        public double V { get; set; }

        public Quality Q { get; set; }

        public Tvq ToTvq()
        {
            return new Tvq(T, V, Q);
        }

    }
}
