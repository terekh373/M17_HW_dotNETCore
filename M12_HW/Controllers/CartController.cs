using M12_HW.Services;
using Microsoft.AspNetCore.Mvc;

namespace M12_HW.Controllers
{
    public class CartController : Controller
    {
        private readonly IServiceCart _cartService;

        public CartController(IServiceCart cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetCartAsync();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            await _cartService.AddToCartAsync(productId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int id)
        {
            await _cartService.RemoveFromCartAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearCartAsync();
            return RedirectToAction("Index");
        }
    }
}
