using Microsoft.WindowsAzure.Storage;

namespace YearlyBackend.DataLayer
{
    public class RegisterEntryRepoFactory
    {
        private readonly string _connectionString;

        public RegisterEntryRepoFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public RegistryEntryRepo GetRegistryEntryRepo(string partitionKey)
        {
            var storageAccount = CloudStorageAccount.Parse(_connectionString);

            var repo = new RegistryEntryRepo(storageAccount, partitionKey);
            return repo;
        }
    }
}
