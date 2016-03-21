using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Periodic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace YearlyWeb2.DataLayer
{
    public class RegistryEntryRepo
    {
        private readonly CloudStorageAccount m_StorageAccount;

        public RegistryEntryRepo(CloudStorageAccount storageAccount)
        {
            m_StorageAccount = storageAccount;
        }

        public void AddRegistryEntry(Tvq tvq)
        { 
            // Create the table client.
            var tableClient = m_StorageAccount.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            var table = tableClient.GetTableReference("registryentries");
            table.CreateIfNotExists();

            var tvqEntity = new TvqEntity("fredrik.bertilsson", tvq);
            var insertOperation = TableOperation.Insert(tvqEntity);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }
    }
}