using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryAppProject.Pages;

public class AddProductModel : PageModel
{
    private readonly IProductService _productService;
    private readonly ILogger<AddProductModel> _logger;

    public AddProductModel(IProductService productService, ILogger<AddProductModel> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [BindProperty]
    public Product Product { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(IFormFile Picture)
    {
        if (Picture == null)
        {
            ModelState.AddModelError(string.Empty, "Please upload a valid image.");
            return Page();
        }

        try
        {
            Product.Image = Picture.FileName;
            using var stream = Picture.OpenReadStream();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            await _productService.AddProduct(Product, ms.ToArray());

            _logger.LogInformation("Product {ProductName} added successfully.", Product.Name);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding the product {ProductName}.", Product.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while adding the product. Please try again.");
            return Page();
        }
    }
}


