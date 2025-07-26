using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Controllers
{
    // [Route("{product}")]
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        // Hiển thị tất cả sản phẩm
        [Route("product")]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // Chi tiết sản phẩm
        // [Route("product/chi-tiet/{slug}")]
        [Route("{slug}")]
        [HttpGet]
        public async Task<IActionResult> Details(string slug)
        {
            var product = await _context.Products
                                        .Include(p => p.Category)
                                        .FirstOrDefaultAsync(p => p.Slug == slug);

            if (product == null)
                return NotFound();

            return View(product);
        }

    }
}
