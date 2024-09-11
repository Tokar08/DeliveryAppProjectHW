using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryAppProject.Pages
{
    public class AddProductModel : PageModel
    {
        private readonly IProductService _productService;

        public AddProductModel(IProductService productService)
        {
            _productService = productService;
        }

        [BindProperty]
        public Product Product { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(IFormFile Picture)
        {
            /*if (!ModelState.IsValid)
            {
                throw new ArgumentException();
            }*/

            Product.Image = Picture.FileName;
            using var stream = Picture.OpenReadStream();
            using var ms = new MemoryStream();
            stream.CopyTo(ms);

            await _productService.AddProduct(Product, ms.ToArray());
            return RedirectToPage("./Index");
        }
    }
}
