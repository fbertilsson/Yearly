using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Periodic.Ts;

namespace YearlyBackend.DataLayer
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

        public async Task AddRegistryEntries(IEnumerable<Tvq> tvqs)
        {
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            await table.CreateIfNotExistsAsync();

            foreach (var tvq in tvqs)
            {
                var tvqEntity = new TvqEntity(m_PartitionKey, tvq);
                var insertOperation = TableOperation.Insert(tvqEntity);

                // Execute the insert operation.
                await table.ExecuteAsync(insertOperation);
            }
        }

        public async Task<IEnumerable<Tvq>> GetRegistryEntries()
        {
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            await table.CreateIfNotExistsAsync();

            // Construct the query operation for all entities where PartitionKey limits the search.
            var query = new TableQuery<TvqEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                    "PartitionKey", QueryComparisons.Equal, m_PartitionKey));

            var result = await table.ExecuteQueryAsync(query);
            return result.Select(x => x.ToTvq());
        }

        /// <summary>
        /// Deletes an entry
        /// </summary>
        /// <param name="t"></param>
        /// <returns>True if successful</returns>
        public async Task<bool> DeleteRegistryEntry(DateTime t)
        {
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            await table.CreateIfNotExistsAsync();

            var entity = new TvqEntity(m_PartitionKey, new Tvq(t, 0, Quality.Ok))
            {
                ETag = "*"
            };
            var deleteOperation = TableOperation.Delete(entity);
            var result = await table.ExecuteAsync(deleteOperation);
            var ok = result.HttpStatusCode == (int) HttpStatusCode.NoContent;
            return ok;
        }
    }
}
