using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerce.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Checkout()
        {
            var cart = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cart)) return RedirectToAction("Index", "Cart");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cartJson = HttpContext.Session.GetString("Cart");
            if (string.IsNullOrEmpty(cartJson)) return RedirectToAction("Index", "Cart");

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            var user = await _userManager.GetUserAsync(User);

            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                OrderItems = cartItems.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    Price = x.Price
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Cart");
            return RedirectToAction("Success");
        }

        public IActionResult Success() => View();
    }
}
