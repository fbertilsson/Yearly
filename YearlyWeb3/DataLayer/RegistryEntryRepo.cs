using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Periodic.Ts;

namespace YearlyWeb3.DataLayer
{
    public class RegistryEntryRepo
    {
        private readonly CloudStorageAccount m_StorageAccount;
        private readonly string m_PartitionKey;

        public RegistryEntryRepo(CloudStorageAccount storageAccount, string partitionKey)
        {
            m_StorageAccount = storageAccount;
            m_PartitionKey = EnforceNamingRules(partitionKey);
        }

        /// <summary>
        /// Transforms a partition key candidate string to a valid string to use for Azure table storage.
        /// N.B. The number sign '#' is not allowed. 
        /// <seealso cref="https://msdn.microsoft.com/library/azure/dd179338.aspx"/>
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <returns>A valid partition key</returns>
        private string EnforceNamingRules(string partitionKey)
        {
            return partitionKey
                .Replace("#", "-n-")
                .Replace("?", "-q-");
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

        public IEnumerable<Tvq> GetRegistryEntries(IPrincipal principal)
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