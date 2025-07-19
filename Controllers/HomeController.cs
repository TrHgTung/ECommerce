using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models;
using ECommerce.Data;
using ECommerce.ViewModel;

namespace ECommerce.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DataContext _context;
    public HomeController(ILogger<HomeController> logger, DataContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public IActionResult Index()
    {
        var products = _context.Products.ToList();
        var categories = _context.Categories.ToList();

        // ko thể hiển thị các model riêng lẻ, mà chỉ có thể hiển thị nhiều loại dưới dạng viewModel
        var viewModelTrangChu = new ViewModelTrangChu
        {
            Products = products,
            Categories = categories
        };

        return View(viewModelTrangChu);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
