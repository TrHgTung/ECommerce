using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    [Authorize] // ✅ Bắt buộc đăng nhập
    // [Route("Cart")]
    public class CartController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .ThenInclude(ci => ci.Product)
                                     .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
                return View(new List<CartItem>());

            return View(cart.CartItems.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Khuyến khích bảo vệ form
        public async Task<IActionResult> AddToCart(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                                    .Include(c => c.CartItems)
                                    .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                cart = new Cart { UserId = user.Id };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.CartItems.Add(new CartItem
                {
                    ProductId = id,
                    Quantity = 1,
                    Price = product.Price,
                    ProductName = product.Name
                });
            }

            await _context.SaveChangesAsync();
            // return RedirectToAction("Index", "Cart");
            return RedirectToAction("Index", "Product");
        }


        public async Task<IActionResult> Remove(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var cart = await _context.Carts
                                     .Include(c => c.CartItems)
                                     .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null) return RedirectToAction("Index");

            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == id);
            if (item != null)
            {
                cart.CartItems.Remove(item);
                _context.CartItems.Remove(item); // Xóa khỏi DB
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
