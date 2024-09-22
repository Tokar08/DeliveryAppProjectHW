using Azure.Data.Tables;
using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.Extensions.Logging;

namespace DeliveryApp.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly TableClient _tableClient;
        private readonly BlobService _blobService;
        private readonly ILogger<ProductService> _logger;
        private const string TableName = "products";

        public ProductService(string connectionString, ILogger<ProductService> logger, BlobService blobService)
        {
            var tableServiceClient = new TableServiceClient(connectionString);
            _tableClient = tableServiceClient.GetTableClient(TableName);
            _tableClient.CreateIfNotExists();
            _blobService = blobService;
            _logger = logger;
        }

        public async Task AddProduct(Product product, IEnumerable<byte> bytes)
        {
            try
            {
                var blobUrl = await _blobService.AddBlob(product.Image, bytes);
                product.Image = blobUrl;
                product.Url = blobUrl;
                await _tableClient.AddEntityAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding product {ProductName}", product.Name);
                throw;
            }
        }

        public IEnumerable<Product> GetProducts()
        {
            try
            {
                return _tableClient.Query<Product>(x => x.PartitionKey.Equals(nameof(Product))).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products");
                throw;
            }
        }

        public Task OrderProduct(string rowKey, string email)
        {
            throw new NotImplementedException();
        }
    }
}
