﻿using Azure.Data.Tables;

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
            var tableServiceClient = new TableServiceClient(_connectionString);

            var repo = new RegistryEntryRepo(tableServiceClient, partitionKey);
            return repo;
        }
    }
}
