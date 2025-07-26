using System.Text.RegularExpressions;
using ECommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Helpers
{
    public static class UniqueSlugGenerator
    {
        // Tạo slug chuẩn SEO từ chuỗi
        private static string GenerateSlug(string phrase)
        {
            string str = phrase.ToLowerInvariant().Trim();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); // loại ký tự đặc biệt
            str = Regex.Replace(str, @"\s+", "-");         // thay khoảng trắng bằng gạch ngang
            str = Regex.Replace(str, @"-+", "-");          // bỏ gạch ngang thừa
            return str;
        }

        // Tạo slug duy nhất cho Category
        public static async Task<string> GenerateUniqueCategorySlugAsync(string name, DataContext context)
        {
            var baseSlug = GenerateSlug(name);
            var slug = baseSlug;
            int count = 1;

            while (await context.Categories.AnyAsync(c => c.Slug == slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            return slug;
        }

        // Tạo slug duy nhất cho Product
        public static async Task<string> GenerateUniqueProductSlugAsync(string name, DataContext context)
        {
            var baseSlug = GenerateSlug(name);
            var slug = baseSlug;
            int count = 1;

            while (await context.Products.AnyAsync(p => p.Slug == slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            return slug;
        }
    }
}
