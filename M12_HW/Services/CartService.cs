using M12_HW.Models;
using Microsoft.EntityFrameworkCore;

namespace M12_HW.Services
{
    public interface IServiceCart
    {
        Task AddToCartAsync(int productId, int quantity = 1);
        Task<List<Cart>> GetCartAsync();
        Task RemoveFromCartAsync(int cartItemId);
        Task ClearCartAsync();
    }

    public class CartService : IServiceCart
    {
        private readonly ProductStoreContext _context;

        public CartService(ProductStoreContext context)
        {
            _context = context;
        }

        public async Task AddToCartAsync(int productId, int quantity = 1)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                cartItem = new Cart
                {
                    ProductId = productId,
                    Quantity = quantity
                };
                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Cart>> GetCartAsync()
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .ToListAsync();
        }

        public async Task RemoveFromCartAsync(int cartItemId)
        {
            var item = await _context.CartItems.FirstOrDefaultAsync(c => c.Id == cartItemId);
            if (item != null)
            {
                _context.CartItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync()
        {
            var items = _context.CartItems;
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
