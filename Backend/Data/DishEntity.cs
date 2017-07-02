using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Backend
{
    public class DishEntity : TableEntity
    {
        public DishEntity(string owner, string rowkey)
        {
            this.PartitionKey = owner;
            this.RowKey = rowkey ?? Guid.NewGuid().ToString();
        }

        public DishEntity() { }
        public string Tags { get; set; }
        public string ImageUrl { get; set; }
        public string Urls { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
    }
}