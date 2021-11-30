using Azure.Data.Tables;
using Backend.Models;
using Microsoft.Extensions.Options;
using System.Diagnostics;

// https://github.com/Azure/azure-sdk-for-net/tree/main/sdk/tables/Azure.Data.Tables
// https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/storage/Azure.Storage.Blobs/AzureStorageNetMigrationV12.md

// https://elcamino.cloud/articles/2020-03-30-azure-storage-blobs-net-sdk-v12-upgrade-guide-and-tips.html

namespace Backend
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

        //public async Task<IEnumerable<DishEntity>> SearchAsync(string search)
        //{
        //    var tableClient = _storageAccount.CreateCloudTableClient();
        //    var table = tableClient.GetTableReference("dish");
        //    // Construct the query operation for all dish entities where PartitionKey="<search>".
        //    var query = new TableQuery<DishEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, search));

        //    var entities = new List<DishEntity>();

        //    // Initialize the continuation token to null to start from the beginning of the table.
        //    TableContinuationToken continuationToken = null;

        //    do
        //    {
        //        // Retrieve a segment (up to 1,000 entities).
        //        TableQuerySegment<DishEntity> tableQueryResult =
        //            await table.ExecuteQuerySegmentedAsync(query, continuationToken);

        //        // Assign the new continuation token to tell the service where to
        //        // continue on the next iteration (or null if it has reached the end).
        //        continuationToken = tableQueryResult.ContinuationToken;

        //        entities.AddRange(tableQueryResult.Results);

        //        // Loop until a null continuation token is received, indicating the end of the table.
        //    } while (continuationToken != null);

        //    return entities;
        //}

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