using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend
{
    public class StorageManager
    {
        private readonly CloudStorageAccount _storageAccount;

        public StorageManager(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
        }

        public async Task InsertAsync(DishEntity dish)
        {
            Stopwatch sw = Stopwatch.StartNew();
            await CreateTableIfNotExists();
            long t1 = sw.ElapsedMilliseconds;
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("dish");
            long t2 = sw.ElapsedMilliseconds;

            var insertOperation = TableOperation.Insert(dish);
            var tableResult = await table.ExecuteAsync(insertOperation);
            long t3 = sw.ElapsedMilliseconds;
        }

        public async Task<IEnumerable<DishEntity>> SearchAsync(string search)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("dish");
            // Construct the query operation for all dish entities where PartitionKey="<search>".
            var query = new TableQuery<DishEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, search));

            var entities = new List<DishEntity>();

            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            do
            {
                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<DishEntity> tableQueryResult =
                    await table.ExecuteQuerySegmentedAsync(query, continuationToken);

                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                entities.AddRange(tableQueryResult.Results);

                // Loop until a null continuation token is received, indicating the end of the table.
            } while (continuationToken != null);

            return entities;
        }

        public async Task<IEnumerable<DishEntity>> GetAllAsync()
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("dish");
            var tableQuery = new TableQuery<DishEntity>();
            var entities = new List<DishEntity>();

            // Initialize the continuation token to null to start from the beginning of the table.
            TableContinuationToken continuationToken = null;

            do
            {
                // Retrieve a segment (up to 1,000 entities).
                TableQuerySegment<DishEntity> tableQueryResult =
                    await table.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);

                // Assign the new continuation token to tell the service where to
                // continue on the next iteration (or null if it has reached the end).
                continuationToken = tableQueryResult.ContinuationToken;

                entities.AddRange(tableQueryResult.Results);

                // Loop until a null continuation token is received, indicating the end of the table.
            } while (continuationToken != null);

            return entities;
        }

        public async Task<DishEntity> GetAsync(string partitionkey, string rowkey)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("dish");
            var retrieveOperation = TableOperation.Retrieve<DishEntity>(partitionkey, rowkey);
            var retrievedResult = await table.ExecuteAsync(retrieveOperation);
            if (retrievedResult.Result != null)
                return (DishEntity)retrievedResult.Result;
            else
                return null;
        }

        private async Task CreateTableIfNotExists()
        {
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("dish");
            await table.CreateIfNotExistsAsync();
        }
    }
}