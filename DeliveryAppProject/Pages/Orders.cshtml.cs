using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeliveryAppProject.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly INavigationService _navigationService;
        private readonly IProductService _productService;

        public OrdersModel(INavigationService navigationService, IProductService productService)
        {
            _navigationService = navigationService;
            _productService = productService;
        }

        public List<OrderDTO> Orders { get; set; }

        public async Task OnGet()
        {
            var products = _productService.GetProducts();
            var ordersJson = await _navigationService.ReceiveMassagesAsync();
            var orders = ordersJson.Select(x => JsonSerializer.Deserialize<Order>(x)!).ToList();
            Orders = orders.Select(x => {
                var product = products.First(y => y.RowKey.Equals(x.ProductRowKey));
                return new OrderDTO()
                {
                    Name = product.Name,
                    Email = x.Email,
                    Price = product.Price,
                    Image = product.Url,
                };
            }).ToList();
        }
    }
}
