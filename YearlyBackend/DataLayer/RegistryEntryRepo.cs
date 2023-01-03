using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Periodic.Ts;

namespace YearlyBackend.DataLayer
{
    public class RegistryEntryRepo
    {
        private const string TableName = "registryentries";

        private readonly TableServiceClient m_StorageAccount;
        private readonly string m_PartitionKey;
        private TableClient m_TableClient;

        public RegistryEntryRepo(TableServiceClient storageAccount, string partitionKey)
        {
            m_StorageAccount = storageAccount;
            m_PartitionKey = EnforceNamingRules(partitionKey);
            m_TableClient = m_StorageAccount.GetTableClient(TableName);
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
            // Create the table if it doesn't exist.
            var table = await m_StorageAccount.CreateTableIfNotExistsAsync(TableName);
            
            foreach (var tvq in tvqs)
            {
                var tvqEntity = new TvqEntity(m_PartitionKey, tvq);

                // Execute the insert operation.
                await m_TableClient.AddEntityAsync(tvqEntity);
            }
        }

        public async Task<IEnumerable<Tvq>> GetRegistryEntries()
        {
            // Create the table if it doesn't exist.
            await m_StorageAccount.CreateTableIfNotExistsAsync(TableName);

            // Construct the query operation for all entities where PartitionKey limits the search.
            var queryAsync = m_TableClient.QueryAsync<TvqEntity>(
                tvq => tvq.PartitionKey == m_PartitionKey);
            
            var result = new List<Tvq>();
            await foreach (var page in queryAsync.AsPages())
            {
                result.AddRange(page.Values.Select(value => value.ToTvq()));
            }
            return result;
        }

        /// <summary>
        /// Deletes an entry
        /// </summary>
        /// <param name="t"></param>
        /// <returns>True if successful</returns>
        public async Task<bool> DeleteRegistryEntry(DateTime t)
        {
            // Create the table if it doesn't exist.
            await m_StorageAccount.CreateTableIfNotExistsAsync(TableName);

            var entity = new TvqEntity(m_PartitionKey, new Tvq(t, 0, Quality.Ok))
            {
                ETag = ETag.All
            };
            var result = await m_TableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);
            var ok = ! result.IsError; // TODO handle error
            return ok;
        }
    }
}
