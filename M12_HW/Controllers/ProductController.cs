using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using M12_HW.Models;
using M12_HW.Services;
using System.Threading.Tasks;

namespace M12_HW.Controllers
{
    public class ProductController : Controller
    {
        private readonly IServiceProducts _serviceProducts;
        public ProductController(IServiceProducts serviceProducts)
        {
            _serviceProducts = serviceProducts;
        }

        [HttpGet]
        public async Task<ViewResult> Index()
        {
            var products = await _serviceProducts.ReadAsync();
            return View(products);
        }
        [HttpGet] //https://localhost:port/product/create
        public ViewResult Create() => View();

        [Authorize(Roles = "admin,moderator,employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,Description,ImageData")] Product product)
        {
            if (ModelState.IsValid)
            {
                using (var ms = new MemoryStream())
                {
                    await product.ImageData.CopyToAsync(ms);
                    product.ImageFile = ms.ToArray();
                }
                product.ImageType = product.ImageData.ContentType;
                await _serviceProducts.CreateAsync(product);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin,moderator,employee")]
        [HttpGet]
        public async Task<ViewResult> Update(int id)
        {
            var product = await _serviceProducts.GetByIdAsync(id);
            return View(product);
        }

        [Authorize(Roles = "moderator,admin,employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("Id,Name,Price,Description,ImageData")] Product product)
        {
            Console.WriteLine(product);
            if (ModelState.IsValid)
            {
                using (var ms = new MemoryStream())
                {
                    await product.ImageData.CopyToAsync(ms);
                    product.ImageFile = ms.ToArray();
                }
                product.ImageType = product.ImageData.ContentType;

                await _serviceProducts.UpdateAsync(id, product);
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [Authorize(Roles = "admin,moderator,employee")]
        [HttpGet]
        // GET: http://localhost:[port]/products/delete
        // Display the product delete confirmation form
        public async Task<ViewResult> Delete()
        {
            return View();
        }

        [Authorize(Roles = "admin,moderator,employee")]
        [HttpPost]
        // POST: http://localhost:[port]/products/delete/{id}
        // Handle product deletion
        public async Task<IActionResult> Delete(int id)
        {
            bool result = await _serviceProducts.DeleteAsync(id); // Delete the product asynchronously
            if(result)
            {
                return RedirectToAction(nameof(Index)); // Redirect to the product list
            }
            return NotFound();
        }
        // GET: http://localhost:[port]/products/details/{id}
        // Displays details of a single product by ID
        public async Task<ViewResult> Details(int id)
        {
            var product = await _serviceProducts.GetByIdAsync(id); // Retrieve product by ID asynchronously
            return View(product); // Return the product details to the view
        }
    }
}
