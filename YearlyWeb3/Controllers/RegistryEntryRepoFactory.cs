using System.Threading;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using YearlyWeb3.DataLayer;

namespace YearlyWeb3.Controllers
{
    public class RegistryEntryRepoFactory
    {
        public RegistryEntryRepo GetRegistryEntryRepo()
        {
            var partitionKey = Thread.CurrentPrincipal.Identity.Name;

            return GetRegistryEntryRepo(partitionKey);
        }

        public RegistryEntryRepo GetRegistryEntryRepo(string partitionKey)
        {
            var storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));

            var repo = new RegistryEntryRepo(storageAccount, partitionKey);
            return repo;
        }
    }
}