using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ECommerce.Controllers
{
    public class CartController : Controller
    {
        private const string CartSessionKey = "Cart";

        public IActionResult Index()
        {
            var cart = GetCartItems();
            return View(cart);
        }

        public IActionResult AddToCart(int id, string name, decimal price)
        {
            var cart = GetCartItems();
            var existingItem = cart.FirstOrDefault(x => x.ProductId == id);
            if (existingItem != null)
                existingItem.Quantity++;
            else
                cart.Add(new CartItem { ProductId = id, ProductName = name, Price = price, Quantity = 1 });

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }

        public IActionResult Remove(int id)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null) cart.Remove(item);

            SaveCartSession(cart);
            return RedirectToAction("Index");
        }

        private List<CartItem> GetCartItems()
        {
            var session = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(session) ? new List<CartItem>() :
                JsonConvert.DeserializeObject<List<CartItem>>(session);
        }

        private void SaveCartSession(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }
    }
}
