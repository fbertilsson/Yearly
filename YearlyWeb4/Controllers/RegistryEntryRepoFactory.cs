using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using YearlyWeb4.DataLayer;

namespace YearlyWeb4.Controllers
{
    public class RegistryEntryRepoFactory
    {
        public RegistryEntryRepo GetRegistryEntryRepo(IConfiguration configuration)
        {
            var connectionString = configuration["AppSettings:StorageConnectionString"];
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var partitionKey = Thread.CurrentPrincipal.Identity.Name;

            var repo = new RegistryEntryRepo(storageAccount, partitionKey);
            return repo;
        }
    }
}
