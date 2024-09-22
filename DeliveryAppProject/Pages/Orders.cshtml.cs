using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeliveryAppProject.Pages
{
    public class OrdersModel : PageModel
    {
        private readonly INavigationService _navService;
        private readonly IProductService _prodService;
        private readonly ILogger<OrdersModel> _logger;

        public OrdersModel(INavigationService navService, IProductService prodService, ILogger<OrdersModel> logger)
        {
            _navService = navService;
            _prodService = prodService;
            _logger = logger;
        }

        public List<OrderDTO> Orders { get; set; } = new();

        public async Task OnGet()
        {
            try
            {
                var products = _prodService.GetProducts();
                var ordersJson = await _navService.ReceiveMassagesAsync();
                var orders = ordersJson.Select(x => JsonSerializer.Deserialize<Order>(x)).ToList();

                if (orders == null || !orders.Any())
                {
                    return;
                }

                Orders = orders.Select(order =>
                {
                    var product = products.FirstOrDefault(p => p.RowKey.Equals(order?.ProductRowKey));
                    return new OrderDTO
                    {
                        Name = product?.Name ?? "Unknown Product",
                        Email = order?.Email ?? "Unknown Email",
                        Price = product?.Price ?? 0,
                        Image = product?.Url ?? ""
                    };
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching orders");
            }
        }
    }
}
