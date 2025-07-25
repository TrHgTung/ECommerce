using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.ViewModel
{
    public class ProductCreateViewModel
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = "";
        public int CategoryId { get; set; }

        public List<SelectListItem> Categories { get; set; } = new();
    }
}
