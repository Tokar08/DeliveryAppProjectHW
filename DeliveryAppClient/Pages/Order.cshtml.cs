using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeliveryAppClient.Pages;

public class OrderModel : PageModel
{
    private readonly ILogger<OrderModel> _logger;
    private readonly IProductService _productService;
    private readonly INavigationService _navigationService;

    public OrderModel(ILogger<OrderModel> logger, IProductService productService, INavigationService navigationService)
    {
        _logger = logger;
        _productService = productService;
        _navigationService = navigationService;
    }

    [BindProperty(SupportsGet = true)]
    public string Name { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Description { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal Price { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ProductKey { get; set; }

    public async Task<IActionResult> OnPostAsync(string key, string userEmail)
    {
        try
        {
            var product = _productService.GetProducts().FirstOrDefault(p => p.RowKey == key);

            if (product == null)
            {
                _logger.LogError("Product with key {Key} not found", key);
                ModelState.AddModelError("", "Product not found");
                return Page();
            }

            Order order = new Order()
            {
                ProductRowKey = product.RowKey,
                Email = userEmail,
            };

            var orderWithProduct = new
            {
                Order = order,
                Product = new
                {
                    product.Name,
                    product.Price,
                    product.Description,
                    product.Url
                }
            };

            string message = JsonSerializer.Serialize(orderWithProduct);

            await _navigationService.SendMessageAsync(message);

            _logger.LogInformation("Order for product {ProductName} with email {UserEmail} was successfully sent.", product.Name, userEmail);

            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while placing the order for product key {Key}", key);
            ModelState.AddModelError("", "An error occurred while processing your order.");
            return Page();
        }
    }
}