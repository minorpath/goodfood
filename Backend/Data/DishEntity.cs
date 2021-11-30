using Azure;
using Azure.Data.Tables;

namespace Backend
{
    public class DishEntity : ITableEntity
    {
        public DishEntity(string owner, string? rowkey)
        {
            this.PartitionKey = owner;
            this.RowKey = rowkey ?? Guid.NewGuid().ToString();
        }

        public DishEntity() { }
        public string? Tags { get; set; }
        public string? ImageUrl { get; set; }
        public string? Urls { get; set; }
        public string? Description { get; set; }
        public string? Comment { get; set; }

        // ITableEntity
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}