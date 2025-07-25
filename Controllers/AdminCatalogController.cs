using ECommerce.Data;
using ECommerce.Models;
using ECommerce.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class AdminCatalogController : Controller
    {
        private readonly DataContext _context;

        public AdminCatalogController(DataContext context)
        {
            _context = context;
        }

        // ========== 1. Thêm danh mục ==========
        [HttpGet("create-category")]
        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost("create-category")]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng tên danh mục (không phân biệt hoa thường)
                var existingCategory = _context.Categories
                    .FirstOrDefault(c => c.Name.ToLower() == category.Name.ToLower());

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
                    return View(category);
                }

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo danh mục thành công.";
                return RedirectToAction("CreateCategory");
            }

            return View(category);
        }

        // ========== 2. Thêm sản phẩm ==========
        [HttpGet("create-product")]
        public IActionResult CreateProduct()
        {
            var viewModel = new ProductCreateViewModel
            {
                Categories = _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost("create-product")]
        public async Task<IActionResult> CreateProduct(ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra số lượng sản phẩm trùng tên (không phân biệt hoa thường)
                var sameNameCount = _context.Products
                    .Count(p => p.Name.ToLower() == model.Name.ToLower());

                if (sameNameCount >= 2)
                {
                    ModelState.AddModelError("Name", "Đã đạt giới hạn 2 sản phẩm cùng tên.");
                }
                else
                {
                    var product = new Product
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Price = model.Price,
                        ImageUrl = model.ImageUrl,
                        CategoryId = model.CategoryId
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Tạo sản phẩm thành công.";
                    return RedirectToAction("CreateProduct");
                }
            }

            // Nếu có lỗi, load lại danh sách danh mục
            model.Categories = _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            return View(model);
        }
    }
}
