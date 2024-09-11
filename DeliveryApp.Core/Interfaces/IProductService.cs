using DeliveryApp.Core.Models;

namespace DeliveryApp.Core.Interfaces;

public interface IProductService
{
    Task AddProduct(Product product, IEnumerable<byte> bytes);
    Task OrderProduct(string rowKey, string email);

    IEnumerable<Product> GetProducts();
}
