using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using DeliveryApp.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace DeliveryAppClient.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly INavigationService _navigationService;
        private readonly IProductService _productService;

        public IndexModel(ILogger<IndexModel> logger, INavigationService navigationService, IProductService productService)
        {
            _logger = logger;
            _navigationService = navigationService;
            _productService = productService;
            Products = new List<Product>(_productService.GetProducts());
        }


        [BindProperty]
        public string Message { get; set; }

        public List<Product> Products { get; set; }

        public async Task OnPostAsync(string key)
        {
            Order order = new Order()
            {
                ProductRowKey = key,
                Email = ""
            };

            await _navigationService.SendMessageAsync(JsonSerializer.Serialize(order));
        }
    }
}
