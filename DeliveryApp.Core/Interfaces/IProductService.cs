using DeliveryApp.Core.Models;

namespace DeliveryApp.Core.Interfaces;

public interface IProductService
{
    Task<Product> AddProduct(Product product);
    Task<Order> OrderProduct(string rowKey, string email);

    IEnumerable<Product> GetProducts();
}
