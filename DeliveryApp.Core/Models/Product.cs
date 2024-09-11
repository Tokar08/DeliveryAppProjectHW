using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeliveryApp.Core.Models;

public class Product : ITableEntity
{
    public Product()
    {
        PartitionKey = nameof(Product);
        RowKey = Guid.NewGuid().ToString();
        Timestamp = DateTimeOffset.UtcNow;
        ETag = new ETag();
    }

    public string Name { get; set; }
    public int Price { get; set; }
    public string Description { get; set; }
    public string Image { get; set; }

    [NotMapped]
    public string Url { get; set; }


    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}
