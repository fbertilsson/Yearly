using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Periodic;

namespace YearlyWeb2.DataLayer
{
    public class RegistryEntryRepo
    {
        private readonly CloudStorageAccount m_StorageAccount;
        private string m_PartitionKey;

        public RegistryEntryRepo(CloudStorageAccount storageAccount)
        {
            m_StorageAccount = storageAccount;
            m_PartitionKey = "fredrik.bertilsson";
        }

        public void AddRegistryEntry(Tvq tvq)
        { 
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            table.CreateIfNotExists();

            var tvqEntity = new TvqEntity(m_PartitionKey, tvq);
            var insertOperation = TableOperation.Insert(tvqEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        public IEnumerable<Tvq> GetRegistryEntries()
        {
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            table.CreateIfNotExists();

            // Construct the query operation for all entities where PartitionKey limits the search.
            var query = new TableQuery<TvqEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey", QueryComparisons.Equal, m_PartitionKey));

            return table.ExecuteQuery(query).Select(x => x.ToTvq());
        }

        /// <summary>
        /// Deletes an entry
        /// </summary>
        /// <param name="t"></param>
        /// <returns>True if successful</returns>
        public bool DeleteRegistryEntry(DateTime t)
        {
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            table.CreateIfNotExists();

            var entity = new TvqEntity(m_PartitionKey, new Tvq(t, 0, Quality.Ok))
            {
                ETag = "*"
            };
            var deleteOperation = TableOperation.Delete(entity);
            var result = table.Execute(deleteOperation);
            var ok = result.HttpStatusCode == (int)HttpStatusCode.NoContent;
            return ok;
        }
    }
}