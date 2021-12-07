using Azure.Data.Tables;
using HeinjoFood.Api.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;

// https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/tables/Azure.Data.Tables
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/storage/Azure.Storage.Blobs/AzureStorageNetMigrationV12.md

// https://elcamino.cloud/articles/2020-03-30-azure-storage-blobs-net-sdk-v12-upgrade-guide-and-tips.html

namespace HeinjoFood.Api
{
    public class StorageManager
    {
        private readonly string _connectionString;
        private readonly ILogger<StorageManager> _logger;
        private readonly TableServiceClient _serviceClient;
        private readonly TableClient _dishTable;
        private readonly StorageOptions _options;

        public StorageManager(IOptions<StorageOptions> options, ILogger<StorageManager> logger)
        {
            _options = options.Value;
            _connectionString = _options.StorageAccount;
            _logger = logger;
            _serviceClient = new TableServiceClient(_connectionString);
            _dishTable = _serviceClient.GetTableClient("dish");
            _dishTable.CreateIfNotExists();
        }

        public async Task InsertAsync(DishEntity dish)
        {
            if (dish.PartitionKey == null)
                dish.PartitionKey = "lye";
            if (dish.RowKey == null)
                dish.RowKey = Guid.NewGuid().ToString();

            var response = await _dishTable.AddEntityAsync(dish);
            _logger.LogInformation("Inserted new dish with RowKey {RowKey} - Status: {HttpStatus}", dish.RowKey, response.Status);
        }

        public async Task DeleteAsync(DishEntity dish)
        {
            ArgumentNullException.ThrowIfNull(dish.RowKey);
            var partitionKey = "lye";
            var response = await _dishTable.DeleteEntityAsync(partitionKey, dish.RowKey);
            _logger.LogInformation("Deleted dish with RowKey {RowKey} - Status: {HttpStatus}", dish.RowKey, response.Status);
        }

        public async Task<IEnumerable<DishEntity>> SearchAsync(string tagSearch)
        {
            var sw = Stopwatch.StartNew();
            var partitionKey = "lye";
            var queryResultsFilter = _dishTable.QueryAsync<DishEntity>(filter: $"Tags eq '{tagSearch}'");
            var entities = new List<DishEntity>();
            var pageCount = 0;
            await foreach (var page in queryResultsFilter.AsPages())
            {
                pageCount++;
                foreach (DishEntity qEntity in page.Values)
                    entities.Add(qEntity);
            }
            sw.Stop();
            return entities;
        }

        public async Task<IEnumerable<DishEntity>> GetAllAsync()
        {
            var sw = Stopwatch.StartNew();
            var partitionKey = "lye";
            var queryResultsFilter = _dishTable.QueryAsync<DishEntity>(filter: $"PartitionKey eq '{partitionKey}'");
            var entities = new List<DishEntity>();
            var pageCount = 0;
            await foreach (var page in queryResultsFilter.AsPages())
            {
                pageCount++;
                foreach (DishEntity qEntity in page.Values)
                    entities.Add(qEntity);
            }
            sw.Stop();
            _logger.LogDebug("Retrieved all dish entities in {ElapsedMs}ms. Total pages: {PageCount} Total entities: {TotalCount}", 
                sw.ElapsedMilliseconds, pageCount, entities.Count);
            return entities;
        }

        public async Task<DishEntity> GetAsync(string rowKey)
        {
            var partitionKey = "lye";
            var entity = await _dishTable.GetEntityAsync<DishEntity>(partitionKey, rowKey);
            return entity;
        }

    }
}