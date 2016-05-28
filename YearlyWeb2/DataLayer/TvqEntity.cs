using System;
using Microsoft.WindowsAzure.Storage.Table;
using Periodic;
using Periodic.Ts;

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

        public DateTime T { get { return new DateTime(long.Parse(RowKey));} }

        public double V { get; set; }

        public Quality Q { get; set; }

        public Tvq ToTvq()
        {
            return new Tvq(T, V, Q);
        }
    }
}