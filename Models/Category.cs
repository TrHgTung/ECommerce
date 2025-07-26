namespace ECommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Slug { get; set; }  // <-- Slug SEO
        public List<Product> Products { get; set; } = new();
    }
}

