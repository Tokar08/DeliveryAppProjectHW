using Azure.Data.Tables;
using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using System.Net.Http.Headers;

namespace DeliveryApp.Core.Services;

public class ProductService : IProductService
{
    private readonly TableServiceClient _tableServiceClient;
    private readonly TableClient _tableClient;
    private readonly string _tableName = "products";
    private readonly BlobService _blobService;

    public ProductService(string connectionString)
    {
        _tableServiceClient = new TableServiceClient(connectionString);
        _tableClient = _tableServiceClient.GetTableClient(_tableName);
        _tableClient.CreateIfNotExists();
        _blobService = new BlobService(connectionString);
    }

    public async Task AddProduct(Product product, IEnumerable<byte> bytes)
    {
        var (uniqueName, sas) = await _blobService.AddBlob(product.Image, bytes);
        product.Image = uniqueName;
        product.Url = sas;
        await _tableClient.AddEntityAsync(product);
    }

    public IEnumerable<Product> GetProducts()
    {
        var response = _tableClient.Query<Product>(x => x.PartitionKey.Equals(nameof(Product)));
        return response.ToList();
    }

    public Task OrderProduct(string rowKey, string email)
    {
        throw new NotImplementedException();
    }
}
