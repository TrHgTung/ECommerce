using ECommerce.Data;
using ECommerce.Models;
using ECommerce.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECommerce.Helpers;
using Microsoft.EntityFrameworkCore;

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
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());

                if (existingCategory != null)
                {
                    ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
                    return View(category);
                }

                // Tạo slug duy nhất
                category.Slug = await UniqueSlugGenerator.GenerateUniqueCategorySlugAsync(category.Name, _context);

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
                var sameNameCount = await _context.Products
                    .CountAsync(p => p.Name.ToLower() == model.Name.ToLower());

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
                        CategoryId = model.CategoryId,
                        Slug = await UniqueSlugGenerator.GenerateUniqueProductSlugAsync(model.Name, _context)
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

        // Hiển thị danh sách danh mục
        [HttpGet("categories")]
        public async Task<IActionResult> ListCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // Sửa danh mục
        [HttpGet("edit-category/{id}")]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost("edit-category/{id}")]
        public async Task<IActionResult> EditCategory(int id, Category updated)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FindAsync(id);
                if (category == null) return NotFound();

                category.Name = updated.Name;
                category.Slug = await UniqueSlugGenerator.GenerateUniqueCategorySlugAsync(updated.Name, _context);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật danh mục thành công.";
                return RedirectToAction("ListCategories");
            }

            return View(updated);
        }

        // Xóa danh mục
        [HttpPost("delete-category/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa danh mục thành công.";
            }
            return RedirectToAction("ListCategories");
        }

        // Quản ly Sản phẩm
        // Hiển thị danh sách sản phẩm
        [HttpGet("products")]
        public async Task<IActionResult> ListProducts()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
        }

        // Sửa sản phẩm
        [HttpGet("edit-product/{id}")]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            var viewModel = new ProductCreateViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                Categories = await _context.Categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync()
            };

            return View(viewModel);
        }

        [HttpPost("edit-product/{id}")]
        public async Task<IActionResult> EditProduct(int id, ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null) return NotFound();

                product.Name = model.Name;
                product.Description = model.Description;
                product.Price = model.Price;
                product.ImageUrl = model.ImageUrl;
                product.CategoryId = model.CategoryId;
                product.Slug = await UniqueSlugGenerator.GenerateUniqueProductSlugAsync(model.Name, _context);

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật sản phẩm thành công.";
                return RedirectToAction("ListProducts");
            }

            model.Categories = await _context.Categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToListAsync();

            return View(model);
        }

        // Xóa sản phẩm
        [HttpPost("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa sản phẩm thành công.";
            }
            return RedirectToAction("ListProducts");
        }


    }
}
